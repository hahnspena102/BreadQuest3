using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public partial class WorldManager
{
    /* Generate the world map. */
    private void GenerateMap()
    {
        /* Clear world */
        floorTilemap.ClearAllTiles();
        rooms.Clear();


        /* Get world area */ 
        RectInt worldArea = GetWorldArea();


        /* Generate BSP tree and rooms */
        if (isBossFloor)
        {
            worldArea = new RectInt(
                -(worldWidth * 3 / 4) + 10,
                -(worldHeight * 3 / 4) + 10,
                (worldWidth * 3 / 2) - 20,
                (worldHeight * 3 / 2) - 20
            );
            GenerateBossFloor(worldArea);
        }
        else
        {
            BSPNode root = new BSPNode(worldArea);
            root.GenerateBSP(minPartitionSize);

            CreateRooms(root);
        }

        ConnectRooms();
        CreateWalls();
    }

    private RectInt GetWorldArea()
    {
        if (isBossFloor)
        {
            return new RectInt(
                -(worldWidth * 3 / 4) + 10,
                -(worldHeight * 3 / 4) + 10,
                (worldWidth * 3 / 2) - 20,
                (worldHeight * 3 / 2) - 20
            );
        }

        return new RectInt(
            -worldWidth / 2,
            -worldHeight / 2,
            worldWidth,
            worldHeight
        );
    }

    private void GenerateBossFloor(RectInt worldArea)
    {
        int roomInset = 0;
        int corridorGap = Random.Range(4, 7);

        if (worldArea.width < minRoomWidth * 2 + corridorGap + roomInset * 2 || worldArea.height < minRoomHeight + roomInset * 2)
        {
            BSPNode root = new BSPNode(worldArea);
            root.GenerateBSP(minPartitionSize);
            CreateRooms(root);
            return;
        }

        int layoutRoll = Random.Range(0, 4);

        int smallRoomWidth = Mathf.Clamp(
            minRoomWidth + Mathf.Max(1, worldArea.width / 36),
            minRoomWidth,
            Mathf.Max(minRoomWidth + 1, worldArea.width / 4)
        );
        int smallRoomHeight = Mathf.Clamp(
            minRoomHeight + Mathf.Max(1, worldArea.height / 36),
            minRoomHeight,
            Mathf.Max(minRoomHeight + 1, worldArea.height / 4)
        );
        int bossRoomWidth = Mathf.Clamp(worldArea.width / 3 - 2, smallRoomWidth + 4, Mathf.Max(smallRoomWidth + 4, worldArea.width - smallRoomWidth - corridorGap - roomInset * 2));
        int bossRoomHeight = Mathf.Clamp(worldArea.height / 3 - 2, smallRoomHeight + 4, Mathf.Max(smallRoomHeight + 4, worldArea.height - smallRoomHeight - corridorGap - roomInset * 2));
        int bossRoomCellCount = Mathf.CeilToInt((bossRoomWidth * bossRoomHeight) / 200f);
        switch (layoutRoll)
        {
            case 0:
            {
                int roomY = Random.Range(worldArea.yMin + roomInset, worldArea.yMax - roomInset - smallRoomHeight + 1);
                RectInt smallRoomArea = new RectInt(worldArea.xMin + roomInset, roomY, smallRoomWidth, smallRoomHeight);
                RectInt bossRoomArea = new RectInt(
                    smallRoomArea.xMax + corridorGap,
                    roomY,
                    bossRoomWidth,
                    bossRoomHeight
                );

                Room smallRoom = CreateRoom(smallRoomArea, RoomShape.Rectangle, 2);
                Room bossRoom = CreateRoom(bossRoomArea, RoomShape.Rectangle, bossRoomCellCount);
                ConnectRoomsThroughOpenings(smallRoom, bossRoom);
                break;
            }
            case 1:
            {
                int roomY = Random.Range(worldArea.yMin + roomInset, worldArea.yMax - roomInset - smallRoomHeight + 1);
                RectInt bossRoomArea = new RectInt(worldArea.xMin + roomInset, roomY, bossRoomWidth, bossRoomHeight);
                RectInt smallRoomArea = new RectInt(
                    bossRoomArea.xMax + corridorGap,
                    roomY,
                    smallRoomWidth,
                    smallRoomHeight
                );

                Room smallRoom = CreateRoom(smallRoomArea, RoomShape.Rectangle, 2);
                Room bossRoom = CreateRoom(bossRoomArea, RoomShape.Rectangle, bossRoomCellCount);
                ConnectRoomsThroughOpenings(smallRoom, bossRoom);
                break;
            }
            case 2:
            {
                int roomX = Random.Range(worldArea.xMin + roomInset, worldArea.xMax - roomInset - smallRoomWidth + 1);
                RectInt smallRoomArea = new RectInt(roomX, worldArea.yMin + roomInset, smallRoomWidth, smallRoomHeight);
                RectInt bossRoomArea = new RectInt(
                    roomX,
                    smallRoomArea.yMax + corridorGap,
                    bossRoomWidth,
                    bossRoomHeight
                );

                Room smallRoom = CreateRoom(smallRoomArea, RoomShape.Rectangle, 2);
                Room bossRoom = CreateRoom(bossRoomArea, RoomShape.Rectangle, bossRoomCellCount);
                ConnectRoomsThroughOpenings(smallRoom, bossRoom);
                break;
            }
            default:
            {
                int roomX = Random.Range(worldArea.xMin + roomInset, worldArea.xMax - roomInset - smallRoomWidth + 1);
                RectInt bossRoomArea = new RectInt(roomX, worldArea.yMin + roomInset, bossRoomWidth, bossRoomHeight);
                RectInt smallRoomArea = new RectInt(
                    roomX,
                    bossRoomArea.yMax + corridorGap,
                    smallRoomWidth,
                    smallRoomHeight
                );

                Room smallRoom = CreateRoom(smallRoomArea, RoomShape.Rectangle, 2);
                Room bossRoom = CreateRoom(bossRoomArea, RoomShape.Rectangle, bossRoomCellCount);
                ConnectRoomsThroughOpenings(smallRoom, bossRoom);
                break;
            }
        }
    }

    private Room CreateRoom(RectInt roomArea, RoomShape roomShape, int subCellCount)
    {
        Room newRoom = new Room(roomArea, rooms.Count);
        newRoom.GenerateRoomShape(roomShape);

        DrawRoom(newRoom);
        newRoom.GenerateVoronoiInRoom(subCellCount);
        Debug.Log($"Created room with sub-cells: {newRoom.subCells.Count}");

        rooms.Add(newRoom);
        return newRoom;
    }

    /* Recursively create rooms in the leaf nodes of the BSP tree. */
    private void CreateRooms(BSPNode node)
    {
        /* If this node is not a leaf, recursively create rooms in its children. */
        if (!node.IsLeaf)
        {
            CreateRooms(node.left);
            CreateRooms(node.right);
            return;
        }


        /* If this node is a leaf, create a room within its area. */
        int maxWidth = node.area.width - 2;
        int maxHeight = node.area.height - 2;

        if (maxWidth < minRoomWidth || maxHeight < minRoomHeight)
        {
            return;
        }

        int roomWidth = Random.Range(
            Mathf.Max(minRoomWidth, (int)(maxWidth * 0.7f)),
            Mathf.Min(maxWidth, maxRoomWidth)
        );

        int roomHeight = Random.Range(
            minRoomHeight,
            Mathf.Min(Mathf.Max(minRoomHeight + 1, (int)(maxHeight * 0.6f)), maxRoomHeight)
        );

        if (roomHeight >= roomWidth)
        {
            roomHeight = Mathf.Max(minRoomHeight, roomWidth - 2);
        }

        int roomX = Random.Range(
            node.area.x + 1,
            node.area.xMax - roomWidth - 1
        );

        int roomY = Random.Range(
            node.area.y + 1,
            node.area.yMax - roomHeight - 1
        );

        RectInt roomArea = new RectInt(roomX, roomY, roomWidth, roomHeight);
        Room newRoom;

        /* Randomly choose a shape for the room and generate its floor tiles. */
        int minDimensionForT = 5;
        bool allowTShape = roomWidth >= minDimensionForT && roomHeight >= minDimensionForT;

        float shapeRoll = Random.value;
        if (shapeRoll < 0.45f)
            newRoom = CreateRoom(roomArea, RoomShape.Rectangle, Random.Range(2, 5));
        else if (shapeRoll < 0.65f)
            newRoom = CreateRoom(roomArea, RoomShape.LShape, Random.Range(2, 5));
        else if (shapeRoll < 0.80f)
            newRoom = allowTShape
                ? CreateRoom(roomArea, RoomShape.TShape, Random.Range(2, 5))
                : CreateRoom(roomArea, RoomShape.Rectangle, Random.Range(2, 5));
        else
            newRoom = CreateRoom(roomArea, RoomShape.Rectangle, Random.Range(2, 5));
            //newRoom = CreateRoom(roomArea, RoomShape.Organic, Random.Range(2, 5));

        /* Assign the new room to the BSP node. */
        node.room = newRoom;
    }


    /* Iteratively connect all rooms in the BSP tree with corridors. */
    private void ConnectRooms()
    {
        if (rooms.Count < 2)
        {
            return;
        }

        for (int i = 0; i < rooms.Count - 1; i++)
        {
            ConnectRoomsThroughOpenings(rooms[i], rooms[i + 1]);
        }
    }

    private void ConnectRoomsThroughOpenings(Room roomA, Room roomB)
    {
        if (roomA == null || roomB == null)
        {
            return;
        }

        RoomOpening openingA = CreateOpeningTowards(roomA, roomB.GetRoomCenter());
        RoomOpening openingB = CreateOpeningTowards(roomB, roomA.GetRoomCenter());
        CreateCorridor(openingA.outside, openingB.outside);
    }

    private RoomOpening CreateOpeningTowards(Room room, Vector2Int target)
    {
        HashSet<Vector2Int> roomTiles = room.floorTiles.Count > 0
            ? new HashSet<Vector2Int>(room.floorTiles)
            : GetRectangleTiles(room.area);

        if (roomTiles.Count == 0)
        {
            Vector2Int center = room.GetRoomCenter();
            return new RoomOpening(center, center, Vector2Int.zero);
        }

        Vector2Int roomCenter = room.GetRoomCenter();
        Vector2Int toTarget = target - roomCenter;
        Vector2Int preferredDir = Mathf.Abs(toTarget.x) >= Mathf.Abs(toTarget.y)
            ? new Vector2Int(toTarget.x >= 0 ? 1 : -1, 0)
            : new Vector2Int(0, toTarget.y >= 0 ? 1 : -1);

        List<Vector2Int> boundaryCandidates = new List<Vector2Int>();
        foreach (Vector2Int tile in roomTiles)
        {
            if (IsBoundaryTile(tile, roomTiles) && IsExposedInDirection(tile, preferredDir, roomTiles))
            {
                boundaryCandidates.Add(tile);
            }
        }

        if (boundaryCandidates.Count == 0)
        {
            foreach (Vector2Int tile in roomTiles)
            {
                if (IsBoundaryTile(tile, roomTiles))
                {
                    boundaryCandidates.Add(tile);
                }
            }
        }

        Vector2Int opening = ChooseOpeningCandidate(boundaryCandidates, room.area, roomCenter, target, preferredDir);
        Vector2Int openingDirection = ResolveOpeningDirection(opening, preferredDir, roomTiles, target);
        Vector2Int outside = opening + openingDirection;

        if (roomTiles.Contains(outside))
        {
            outside = opening;
        }

        if (!room.floorTiles.Contains(opening))
        {
            room.floorTiles.Add(opening);
        }

        RoomOpening roomOpening = new RoomOpening(opening, outside, openingDirection);
        if (!HasMatchingOpening(room, roomOpening))
        {
            room.openings.Add(roomOpening);
        }

        DrawTile(opening.x, opening.y);
        DrawTile(outside.x, outside.y);
        return roomOpening;
    }

    private bool HasMatchingOpening(Room room, RoomOpening opening)
    {
        for (int i = 0; i < room.openings.Count; i++)
        {
            if (room.openings[i].inside == opening.inside && room.openings[i].outside == opening.outside)
            {
                return true;
            }
        }

        return false;
    }

    private Vector2Int ResolveOpeningDirection(Vector2Int opening, Vector2Int preferredDir, HashSet<Vector2Int> roomTiles, Vector2Int target)
    {
        Vector2Int[] dirs =
        {
            preferredDir,
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1)
        };

        Vector2Int bestDir = preferredDir;
        int bestScore = int.MinValue;

        for (int i = 0; i < dirs.Length; i++)
        {
            Vector2Int dir = dirs[i];
            if (dir == Vector2Int.zero)
            {
                continue;
            }

            Vector2Int outside = opening + dir;
            if (roomTiles.Contains(outside))
            {
                continue;
            }

            int score = -Mathf.Abs(outside.x - target.x) - Mathf.Abs(outside.y - target.y);
            if (dir == preferredDir)
            {
                score += 200;
            }

            if (score > bestScore)
            {
                bestScore = score;
                bestDir = dir;
            }
        }

        return bestDir;
    }

    private bool IsBoundaryTile(Vector2Int tile, HashSet<Vector2Int> roomTiles)
    {
        Vector2Int[] dirs =
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        for (int i = 0; i < dirs.Length; i++)
        {
            if (!roomTiles.Contains(tile + dirs[i]))
            {
                return true;
            }
        }

        return false;
    }

    private bool IsExposedInDirection(Vector2Int tile, Vector2Int direction, HashSet<Vector2Int> roomTiles)
    {
        return !roomTiles.Contains(tile + direction);
    }

    private Vector2Int ChooseOpeningCandidate(List<Vector2Int> candidates, RectInt roomArea, Vector2Int roomCenter, Vector2Int target, Vector2Int preferredDir)
    {
        if (candidates == null || candidates.Count == 0)
        {
            return roomCenter;
        }

        Vector2Int best = candidates[0];
        int bestScore = int.MinValue;

        for (int i = 0; i < candidates.Count; i++)
        {
            Vector2Int candidate = candidates[i];
            int score = 0;

            if (preferredDir.x > 0 && candidate.x == roomArea.xMax - 1)
            {
                score += 5000;
            }
            else if (preferredDir.x < 0 && candidate.x == roomArea.xMin)
            {
                score += 5000;
            }
            else if (preferredDir.y > 0 && candidate.y == roomArea.yMax - 1)
            {
                score += 5000;
            }
            else if (preferredDir.y < 0 && candidate.y == roomArea.yMin)
            {
                score += 5000;
            }

            score -= Mathf.Abs(candidate.x - target.x) + Mathf.Abs(candidate.y - target.y);

            if (preferredDir.x != 0)
            {
                score -= Mathf.Abs(candidate.y - roomCenter.y);
            }
            else
            {
                score -= Mathf.Abs(candidate.x - roomCenter.x);
            }

            if (score > bestScore)
            {
                bestScore = score;
                best = candidate;
            }
        }

        return best;
    }


    /* Creates a corridor between two points by drawing an L-shaped path. */
    private void CreateCorridor(Vector2Int a, Vector2Int b)
    {
        if (Random.value > 0.5f)
        {
            DrawCorridor(new RectInt(
                Mathf.Min(a.x, b.x),
                a.y,
                Mathf.Abs(a.x - b.x) + 1,
                1
            ));

            DrawCorridor(new RectInt(
                b.x,
                Mathf.Min(a.y, b.y),
                1,
                Mathf.Abs(a.y - b.y) + 1
            ));
        }
        else
        {
            DrawCorridor(new RectInt(
                a.x,
                Mathf.Min(a.y, b.y),
                1,
                Mathf.Abs(a.y - b.y) + 1
            ));

            DrawCorridor(new RectInt(
                Mathf.Min(a.x, b.x),
                b.y,
                Mathf.Abs(a.x - b.x) + 1,
                1
            ));
        }
    }

    /* Helper function to draw a tile at the given coordinates. */
    private void DrawTile(int x, int y)
    {
        Vector3Int position = new Vector3Int(x, y, 0);
        floorTilemap.SetTile(position, floorTile);
    }

    /* Helper function to draw a room in the tilemap. */
    private void DrawRoom(Room room)
    {
        if (room.floorTiles.Count > 0)
        {
            foreach (Vector2Int tile in room.floorTiles)
            {
                DrawTile(tile.x, tile.y);
            }
        }
        else
        {
            for (int x = room.area.xMin; x < room.area.xMax; x++)
            {
                for (int y = room.area.yMin; y < room.area.yMax; y++)
                {
                    DrawTile(x, y);
                }
            }
        }
    }

    /* Helper function to draw a corridor and its walls. */
    private void DrawCorridor(RectInt corridor)
    {
        /* Helper function to draw a corridor and its walls. */
        for (int x = corridor.xMin; x < corridor.xMax; x++)
        {
            for (int y = corridor.yMin; y < corridor.yMax; y++)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        DrawTile(x + dx, y + dy);
                    }
                }
            }
        }
    }

    /* Helper function to create walls around the world. */
    private void CreateWalls()
    {
        BoundsInt bounds = floorTilemap.cellBounds;
        bounds.yMax += 12;
        bounds.xMin -= 4;
        bounds.xMax += 4;
        bounds.yMin -= 4;

        foreach (var pos in bounds.allPositionsWithin)
        {
            if (floorTilemap.GetTile(pos) != null)
            {
                continue;
            }

            bool hasFloorBelow = floorTilemap.GetTile(pos + Vector3Int.down) != null;

            if (hasFloorBelow)
            {
                Vector3Int oneUp = pos + Vector3Int.up;
                Vector3Int twoUp = pos + Vector3Int.up * 2;

                bool canPlaceOne = floorTilemap.GetTile(oneUp) == null;
                bool canPlaceTwo = canPlaceOne && floorTilemap.GetTile(twoUp) == null;

                int startHeight = 0;

                if (canPlaceTwo)
                {
                    wallTilemap.SetTile(pos, wallTile);
                    wallTilemap.SetTile(oneUp, wallTile);
                    startHeight = 2;
                }
                else
                {
                    // If there is space above, a wall top will be painted there; keep the base as a normal wall.
                    wallTilemap.SetTile(pos, canPlaceOne ? wallTile : smallWallTile);
                    startHeight = 1;
                }

                for (int i = startHeight; i < 6; i++)
                {
                    Vector3Int abovePos = pos + Vector3Int.up * i;

                    if (floorTilemap.GetTile(abovePos) == null)
                    {
                        wallTilemap.SetTile(abovePos, wallTopTile);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        FillEmptyWithWalls();
    }

    private void FillEmptyWithWalls()
    {
        RectInt worldArea = GetWorldArea();
        worldArea.xMin -= 10;
        worldArea.xMax += 10;
        worldArea.yMin -= 10;
        worldArea.yMax += 10;

        // Fill walls first
        for (int x = worldArea.xMin; x < worldArea.xMax; x++)
        {
            for (int y = worldArea.yMin; y < worldArea.yMax; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);

                if (floorTilemap.GetTile(pos) == null &&
                    wallTilemap.GetTile(pos) == null)
                {
                    wallTilemap.SetTile(pos, wallTopTile);
                }
            }
        }

        // Place decor afterward
        List<Vector2Int> placedDecor = new();

        int decorCount = Mathf.Max(
            10,
            (worldArea.width * worldArea.height) / 10
        );

        int minSpacing = 6;
        int maxAttempts = decorCount * 20;

        for (int attempt = 0;
            attempt < maxAttempts && placedDecor.Count < decorCount;
            attempt++)
        {
            int x = Random.Range(worldArea.xMin, worldArea.xMax);
            int y = Random.Range(worldArea.yMin, worldArea.yMax);

            Vector3Int pos = new Vector3Int(x, y, 0);

            // Don't place inside dungeon
            if (NearFloor(pos))
            {
                continue;
            }

            Vector2Int candidate = new Vector2Int(x, y);

            bool tooClose = false;

            foreach (Vector2Int existing in placedDecor)
            {
                if ((existing - candidate).sqrMagnitude <
                    minSpacing * minSpacing)
                {
                    tooClose = true;
                    break;
                }
            }

            if (tooClose)
                continue;

            placedDecor.Add(candidate);

            decorTilemap.SetTile(
                pos,
                natureDecorTiles[
                    Random.Range(0, natureDecorTiles.Length)
                ]
            );
        }
    }

    bool NearFloor(Vector3Int pos)
    {
        for (int dx = -2; dx <= 2; dx++)
        {
            for (int dy = -2; dy <= 2; dy++)
            {
                if (floorTilemap.GetTile(pos + new Vector3Int(dx, dy, 0)) != null)
                    return true;
            }
        }

        return false;
    }

    /* Debug function to visualize rooms and their sub-cells in the editor. */
    private void OnDrawGizmos()
    {
        if (rooms == null)
        {
            return;
        }

        foreach (var room in rooms)
        {
            foreach (var cell in room.subCells)
            {
                var rnd = new System.Random(cell.center.GetHashCode());
                Color baseColor = Color.HSVToRGB((float)rnd.NextDouble(), 0.6f, 1f);
                baseColor.a = 0.5f;
                Gizmos.color = baseColor;

                foreach (var tile in cell.tiles)
                {
                    Vector3 pos = new Vector3(tile.x + 0.5f, tile.y + 0.5f, 0);
                    Gizmos.DrawCube(pos, Vector3.one * 0.9f);
                }

                Gizmos.color = Color.red;
                Vector3 centerPos = new Vector3(cell.center.x + 0.5f, cell.center.y + 0.5f, 0);
                Gizmos.DrawSphere(centerPos, 0.3f);
            }

            // Display room ID at room center
            Vector3 roomCenter = new Vector3(room.area.center.x + 0.5f, room.area.center.y + 0.5f, 0);
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(roomCenter, 0.5f);

#if UNITY_EDITOR
            Handles.color = Color.white;
            Handles.Label(roomCenter + Vector3.back * 0.5f, $"Room {room.roomID}");
#endif
        }
    }
}
