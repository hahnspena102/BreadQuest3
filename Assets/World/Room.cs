using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using NavMeshPlus.Components;
using System.Collections.Generic;

public class RoomSubCell
{
    public Vector2Int center;
    public List<Vector2Int> tiles = new List<Vector2Int>();
}

public struct RoomOpening
{
    public Vector2Int inside;
    public Vector2Int outside;
    public Vector2Int direction;

    public RoomOpening(Vector2Int inside, Vector2Int outside, Vector2Int direction)
    {
        this.inside = inside;
        this.outside = outside;
        this.direction = direction;
    }
}

public enum RoomShape
{
    Rectangle,      // Classic rectangle
    LShape,         // L-shaped room
    TShape,         // T-shaped room
    Organic         // Irregular organic shape
}

public class Room
{
    public int roomID;
    public RectInt area;
    public RoomShape roomShape = RoomShape.Rectangle;
    public List<Vector2Int> floorTiles = new List<Vector2Int>(); // All valid floor tiles in this room
    public List<RoomOpening> openings = new List<RoomOpening>();
    public List<RoomSubCell> subCells = new List<RoomSubCell>();

    public List<Vector3Int> barrierPositions = new List<Vector3Int>();
    public List<Wave> waves = new List<Wave>();
    ///public List<GameObject> enemiesInRoom = new List<GameObject>();
    public int nextBossWaveToSpawnIndex = 1;
    
    public bool isSealed = false;
    public bool isCleared = false;
    public bool isEntered = false;
    public int currentWaveIndex = 0;

    public Room(RectInt area, int roomID = -1)
    {
        this.roomID = roomID;
        this.area = area;
        this.subCells = new List<RoomSubCell>();
        this.waves = new List<Wave>();
        this.floorTiles = new List<Vector2Int>();
        this.openings = new List<RoomOpening>();
        this.nextBossWaveToSpawnIndex = 1;
    }


    public List<RoomSubCell> GenerateVoronoiInRoom(int seedCount)
    {
        List<RoomSubCell> cells = new List<RoomSubCell>();

        // Use floorTiles if available (for non-rectangular rooms), otherwise use the full area
        List<Vector2Int> tilesToAssign = floorTiles.Count > 0 ? new List<Vector2Int>(floorTiles) : GetAllAreaTiles();

        if (tilesToAssign.Count == 0 || seedCount <= 0)
        {
            subCells = cells;
            return subCells;
        }

        // Clamp seedCount to available tiles
        seedCount = Mathf.Min(seedCount, tilesToAssign.Count);

        // Pick unique seeds from tilesToAssign
        HashSet<int> chosenIndices = new HashSet<int>();
        for (int i = 0; i < seedCount; i++)
        {
            int idx;
            int attempts = 0;
            do
            {
                idx = Random.Range(0, tilesToAssign.Count);
                attempts++;
            } while (chosenIndices.Contains(idx) && attempts < 10);

            // Fallback to find first unused index
            if (chosenIndices.Contains(idx))
            {
                for (int j = 0; j < tilesToAssign.Count; j++)
                {
                    if (!chosenIndices.Contains(j))
                    {
                        idx = j;
                        break;
                    }
                }
            }

            chosenIndices.Add(idx);
            Vector2Int seed = tilesToAssign[idx];
            cells.Add(new RoomSubCell { center = seed });
        }

        // Assign tiles in room to nearest seed
        foreach (Vector2Int tile in tilesToAssign)
        {
            int closestIndex = 0;
            float closestDist = float.MaxValue;

            for (int i = 0; i < cells.Count; i++)
            {
                float dist = (tile - cells[i].center).sqrMagnitude;
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestIndex = i;
                }
            }

            cells[closestIndex].tiles.Add(tile);
        }

        subCells = cells;
        return subCells;
    }

    private List<Vector2Int> GetAllAreaTiles()
    {
        List<Vector2Int> tiles = new List<Vector2Int>();
        for (int x = area.xMin; x < area.xMax; x++)
        {
            for (int y = area.yMin; y < area.yMax; y++)
            {
                tiles.Add(new Vector2Int(x, y));
            }
        }
        return tiles;
    }

    public void GenerateRoomShape(RoomShape shape)
    {
        roomShape = shape;
        floorTiles.Clear();
        openings.Clear();

        switch (shape)
        {
            case RoomShape.Rectangle:
                GenerateRectangleShape();
                break;
            case RoomShape.LShape:
                GenerateLShape();
                break;
            case RoomShape.TShape:
                GenerateTShape();
                break;
            case RoomShape.Organic:
                GenerateOrganicShape();
                break;
        }
    }

    private void GenerateRectangleShape()
    {
        // Simple rectangular room (default behavior)
        for (int x = area.xMin; x < area.xMax; x++)
        {
            for (int y = area.yMin; y < area.yMax; y++)
            {
                floorTiles.Add(new Vector2Int(x, y));
            }
        }
    }

    private void GenerateLShape()
    {
        // L-shaped room: carves out a corner
        int width = area.width;
        int height = area.height;
        int carveX = Random.Range(Mathf.Max(1, width / 3), Mathf.Max(2, width * 2 / 3));
        int carveY = Random.Range(Mathf.Max(1, height / 3), Mathf.Max(2, height * 2 / 3));
        int cornerChoice = Random.Range(0, 4);

        for (int x = area.xMin; x < area.xMax; x++)
        {
            for (int y = area.yMin; y < area.yMax; y++)
            {
                bool isValid = false;

                if (cornerChoice == 0) // Remove top-right corner
                    isValid = !(x >= area.xMin + carveX && y >= area.yMin + carveY);
                else if (cornerChoice == 1) // Remove top-left corner
                    isValid = !(x < area.xMin + carveX && y >= area.yMin + carveY);
                else if (cornerChoice == 2) // Remove bottom-right corner
                    isValid = !(x >= area.xMin + carveX && y < area.yMin + carveY);
                else // Remove bottom-left corner
                    isValid = !(x < area.xMin + carveX && y < area.yMin + carveY);

                if (isValid)
                    floorTiles.Add(new Vector2Int(x, y));
            }
        }
    }

    private void GenerateTShape()
    {
        // T-shaped room with a horizontal bar and vertical stem
        int width = area.width;
        int height = area.height;
        int barHeight = Mathf.Max(2, height / 3);
        int stemWidth = Mathf.Max(2, width / 3);
        bool isHorizontalT = Random.value > 0.5f;

        for (int x = area.xMin; x < area.xMax; x++)
        {
            for (int y = area.yMin; y < area.yMax; y++)
            {
                bool isValid = false;

                if (isHorizontalT)
                {
                    // Horizontal bar at top, vertical stem at bottom-center
                    bool inTopBar = y >= area.yMax - barHeight;
                    bool inBottomStem = y < area.yMax - barHeight && x >= area.xMin + (width - stemWidth) / 2 && x < area.xMin + (width + stemWidth) / 2;
                    isValid = inTopBar || inBottomStem;
                }
                else
                {
                    // Vertical bar on left, horizontal stem on right
                    bool inLeftBar = x < area.xMin + stemWidth;
                    bool inRightStem = x >= area.xMin + stemWidth && y >= area.yMin + (height - stemWidth) / 2 && y < area.yMin + (height + stemWidth) / 2;
                    isValid = inLeftBar || inRightStem;
                }

                if (isValid)
                    floorTiles.Add(new Vector2Int(x, y));
            }
        }
    }

    private void GenerateOrganicShape()
    {
        // Organic irregular shape using cellular automata-like approach
        // Start with a filled rectangle
        HashSet<Vector2Int> activeTiles = new HashSet<Vector2Int>();
        for (int x = area.xMin; x < area.xMax; x++)
        {
            for (int y = area.yMin; y < area.yMax; y++)
            {
                activeTiles.Add(new Vector2Int(x, y));
            }
        }

        // Random carving iterations
        int iterations = Random.Range(3, 6);
        for (int iter = 0; iter < iterations; iter++)
        {
            HashSet<Vector2Int> toRemove = new HashSet<Vector2Int>();

            // Randomly carve out small pockets
            foreach (var tile in activeTiles)
            {
                if (Random.value < 0.15f) // 15% chance to remove
                {
                    // Check if it's not on the border
                    if (tile.x > area.xMin + 1 && tile.x < area.xMax - 2 &&
                        tile.y > area.yMin + 1 && tile.y < area.yMax - 2)
                    {
                        toRemove.Add(tile);
                    }
                }
            }

            foreach (var tile in toRemove)
            {
                activeTiles.Remove(tile);
            }
        }

        // Add random alcoves/protrusions
        int alcoveCount = Random.Range(1, 4);
        for (int i = 0; i < alcoveCount; i++)
        {
            Vector2Int alcoveCenter = new Vector2Int(
                Random.Range(area.xMin + 2, area.xMax - 2),
                Random.Range(area.yMin + 2, area.yMax - 2)
            );

            // Add tiles around the alcove center
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    Vector2Int pos = alcoveCenter + new Vector2Int(dx, dy);
                    if (pos.x >= area.xMin + 1 && pos.x < area.xMax - 1 &&
                        pos.y >= area.yMin + 1 && pos.y < area.yMax - 1)
                    {
                        activeTiles.Add(pos);
                    }
                }
            }
        }

        floorTiles.AddRange(activeTiles);
    }

    public bool IsPointInRoom(Vector2Int point)
    {
        if (floorTiles.Count > 0)
        {
            // For non-rectangular rooms, check if point is in floorTiles
            return floorTiles.Contains(point);
        }
        else
        {
            // For rectangular rooms, use the area bounds
            return area.Contains(point);
        }
    }

    public Vector2Int GetRoomCenter()
    {
        if (floorTiles.Count > 0)
        {
            // For non-rectangular rooms, calculate center from floorTiles
            int sumX = 0, sumY = 0;
            foreach (var tile in floorTiles)
            {
                sumX += tile.x;
                sumY += tile.y;
            }
            return new Vector2Int(sumX / floorTiles.Count, sumY / floorTiles.Count);
        }
        else
        {
            // For rectangular rooms
            return new Vector2Int(
                area.xMin + area.width / 2,
                area.yMin + area.height / 2
            );
        }
    }
    
}

