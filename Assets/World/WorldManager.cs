using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using NavMeshPlus.Components;
using System.Collections.Generic;

public class WorldManager : MonoBehaviour
{
    [SerializeField] private int worldWidth = 108;
    [SerializeField] private int worldHeight = 72;
    [SerializeField] private NavMeshSurface navMeshSurface;
    [SerializeField] private int minPartitionSize = 12;
    [SerializeField] private int minRoomSize = 6;

    private List<RectInt> rooms = new List<RectInt>();

    public Tilemap floorTilemap;
    public Tilemap wallTilemap;
    public TileBase floorTile;
    public TileBase wallTile;
    public TileBase wallTopTile;

    void Start()
    {
        StartCoroutine(BuildWorld());
    }


    IEnumerator BuildWorld()
    {
        /* Build the world in a coroutine to allow for better performance 
        and to ensure that the NavMesh is built after the tilemap is fully updated. */    
        GenerateMap();

        floorTilemap.RefreshAllTiles();
        yield return null;
        yield return new WaitForFixedUpdate();

        Physics2D.SyncTransforms();
        navMeshSurface.RemoveData();
        navMeshSurface.BuildNavMesh();

        Debug.Log(
            "NavMesh verts: " +
            UnityEngine.AI.NavMesh.CalculateTriangulation().vertices.Length
        );
    }

    void GenerateMap()
    {
        /* Generate entire map.*/
        floorTilemap.ClearAllTiles();
        rooms.Clear();

        RectInt worldArea = new RectInt(
            -worldWidth / 2,
            -worldHeight / 2,
            worldWidth,
            worldHeight
        );

        BSPNode root = new BSPNode(worldArea);
        root.GenerateBSP(minPartitionSize);

        CreateRooms(root);
        DrawRooms();   
        ConnectRooms(root);
        CreateWalls();
    }

    void CreateRooms(BSPNode node)
    {
        /* Recursively create rooms in the leaf nodes of the BSP tree. */
        if (!node.IsLeaf)
        {
            CreateRooms(node.left);
            CreateRooms(node.right);
            return;
        }

        int maxWidth = node.area.width - 2;
        int maxHeight = node.area.height - 2;

        if (maxWidth < minRoomSize || maxHeight < minRoomSize)
            return;

        // Make width large (70–100% of available)
        int roomWidth = Random.Range(
            Mathf.Max(minRoomSize, (int)(maxWidth * 0.7f)),
            maxWidth
        );

        // Make height smaller (40–70% of available)
        int roomHeight = Random.Range(
            minRoomSize,
            Mathf.Max(minRoomSize + 1, (int)(maxHeight * 0.6f))
        );

        // Ensure width is greater than height
        if (roomHeight >= roomWidth)
            roomHeight = Mathf.Max(minRoomSize, roomWidth - 2);

        int roomX = Random.Range(
            node.area.x + 1,
            node.area.xMax - roomWidth - 1
        );

        int roomY = Random.Range(
            node.area.y + 1,
            node.area.yMax - roomHeight - 1
        );

        node.room = new RectInt(roomX, roomY, roomWidth, roomHeight);
        rooms.Add(node.room);
    }

    void ConnectRooms(BSPNode node)
    {
        if (rooms.Count < 2)
            return;

        for (int i = 0; i < rooms.Count - 1; i++)
        {
            Vector2Int pointA = GetRoomCenter(rooms[i]);
            Vector2Int pointB = GetRoomCenter(rooms[i + 1]);

            CreateCorridor(pointA, pointB);
        }
    }

    void CreateCorridor(Vector2Int a, Vector2Int b)
    {
        /* Creates a corridor between two points by drawing an L-shaped path. */
        if (Random.value > 0.5f)
        {
            // Horizontal then vertical
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
            // Vertical then horizontal
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

    void DrawTile(int x, int y)
    {
        /* Helper function to draw a tile at the given coordinates. */
        Vector3Int position = new Vector3Int(x, y, 0);
        floorTilemap.SetTile(position, floorTile);
    }

    void DrawRooms()
    {
        /* Helper function to draw all rooms in the tilemap. */
        foreach (var room in rooms)
        {
            for (int x = room.xMin; x < room.xMax; x++)
            {
                for (int y = room.yMin; y < room.yMax; y++)
                {
                    DrawTile(x, y);
                }
            }
        }
    }

    void DrawCorridor(RectInt corridor)
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

    Vector2Int GetRoomCenter(RectInt room)
    {
        return new Vector2Int(
            room.xMin + room.width / 2,
            room.yMin + room.height / 2
        );
    }

    void CreateWalls()
    {
        BoundsInt bounds = floorTilemap.cellBounds;

        foreach (var pos in bounds.allPositionsWithin)
        {
            if (floorTilemap.GetTile(pos) != null)
                continue;

            bool hasFloorBelow = floorTilemap.GetTile(pos + Vector3Int.down) != null;
            bool hasFloorAbove = floorTilemap.GetTile(pos + Vector3Int.up) != null;
            bool hasFloorLeft  = floorTilemap.GetTile(pos + Vector3Int.left) != null;
            bool hasFloorRight = floorTilemap.GetTile(pos + Vector3Int.right) != null;

            // ===== TOP WALLS =====
            if (hasFloorBelow)
            {
                // First 2 layers = wallTile
                wallTilemap.SetTile(pos, wallTile);
                wallTilemap.SetTile(pos + Vector3Int.up, wallTile);

                // Everything above those = wallTopTile
                for (int i = 2; i < 6; i++)
                {
                    Vector3Int abovePos = pos + Vector3Int.up * i;

                    if (floorTilemap.GetTile(abovePos) == null)
                        wallTilemap.SetTile(abovePos, wallTopTile);
                    else
                        break;
                }
            }
            // ===== BOTTOM WALL =====
            else if (hasFloorAbove)
            {
                //wallTilemap.SetTile(pos, wallTopTile);
            }
            // ===== SIDE WALLS =====
            else if (hasFloorLeft || hasFloorRight)
            {
                //wallTilemap.SetTile(pos, wallTopTile);
            }
        }

        FillEmptyWithWalls();
    }

    void FillEmptyWithWalls()
    {
        RectInt worldArea = new RectInt(
            -worldWidth / 2,
            -worldHeight / 2,
            worldWidth,
            worldHeight
        );

        for (int x = worldArea.xMin; x < worldArea.xMax; x++)
        {
            for (int y = worldArea.yMin; y < worldArea.yMax; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);

                // If not floor and not already a wall
                if (floorTilemap.GetTile(pos) == null &&
                    wallTilemap.GetTile(pos) == null)
                {
                    wallTilemap.SetTile(pos, wallTopTile);
                }
            }
        }
    }


}
