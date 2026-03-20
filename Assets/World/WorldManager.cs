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
    private EnemyManager enemyManager;
    private List<Room> rooms = new List<Room>();
    private Player player;
    [SerializeField]private GameObject teleporterPrefab;
    [SerializeField] private GameObject chestPrefab;    

    public Tilemap floorTilemap;
    public Tilemap wallTilemap;
    public Tilemap barrierTilemap;
    public TileBase floorTile;
    public TileBase wallTile;
    public TileBase wallTopTile;
    public TileBase barrierTile;

    public Room startingRoom;
    public Room endingRoom;

    void Start()
    {
        enemyManager = FindFirstObjectByType<EnemyManager>();
        player = FindFirstObjectByType<Player>();
        navMeshSurface.hideEditorLogs = true;
        StartCoroutine(BuildWorld());
    }

    void Update()
    {
        foreach (Room room in rooms)
        {
            bool wavesCompleted = room.waves.TrueForAll(wave => wave.isCleared);
            if (room.isEntered && room.isSealed && wavesCompleted)
            {
                Debugger.Log("Cleared Room " + room.roomID, type: DebugType.World);
                room.isCleared = true;
                CreateChest(room);
                if (room == endingRoom)
                {
                    Debugger.Log("Player has cleared the final room!", type: DebugType.World);
                    Vector2Int center = room.GetRoomCenter();
                    Vector3 teleporterPos = new Vector3(center.x+ 2, center.y + 2, 0);
                    Instantiate(teleporterPrefab, teleporterPos, Quaternion.identity);
                }
                UnsealRoom(room);
            }

            if (player != null && room.area.Contains(Vector2Int.FloorToInt(player.transform.position)))
            {
                if (!room.isEntered)
                {
                    Debugger.Log("Entered Room " + room.roomID, type: DebugType.World);
                    SealRoom(room);
                    room.isEntered = true;
                    Debugger.Log("room.currentWaveIndex: " + room.currentWaveIndex, type: DebugType.World);
                }

                if (room.currentWaveIndex >= room.waves.Count) continue;
                
                Wave currentWave = room.waves[room.currentWaveIndex];

                if (!currentWave.isSpawned)
                {                    

                    Debugger.Log("Spawning wave " + room.currentWaveIndex + " in Room " + room.roomID, type: DebugType.World);
                    enemyManager.SpawnInWave(currentWave);
                    currentWave.isSpawned = true;
                }

                
                if (currentWave.isSpawned && !currentWave.isCleared)
                {
                    bool allEnemiesDefeated = currentWave.enemiesLeft == 0;

                    if (allEnemiesDefeated)
                    {
                        Debugger.Log("Cleared wave " + room.currentWaveIndex + " in Room " + room.roomID, type: DebugType.World);
                        currentWave.isCleared = true;
                    }
                }

                if (currentWave.isCleared)
                {
                    room.currentWaveIndex = Mathf.Min(room.currentWaveIndex + 1, room.waves.Count - 1);
                }
                
             
               
            
                
                
            }

        }
    }

    void CreateChest(Room room)
    {
        Vector2Int center = room.GetRoomCenter();
        Vector3 chestPos = new Vector3(center.x, center.y, 0);
        GameObject chestObj = Instantiate(chestPrefab, chestPos, Quaternion.identity);
        chestObj.transform.SetParent(transform);
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
        Debugger.Log(rooms.Count + " rooms generated.", type: DebugType.World);
        startingRoom = ChooseStartingRoom();
        endingRoom = ChooseEndingRoom(startingRoom);
        Debugger.Log("Starting Room: " + startingRoom.roomID, type: DebugType.World);
        Debugger.Log("Ending Room: " + endingRoom.roomID, type: DebugType.World);

        InitializeRooms();

        InitializePlayer();
    }

    void InitializeRooms()
    {
        foreach (var room in rooms)
        {
          
            room.isEntered = false;
            room.isCleared = false;
            room.isSealed = false;
            room.barrierPositions.Clear();


            if (room == startingRoom) {
                continue;
            }
            
         
            int numWaves = Random.Range(1, 4);
            room.waves.Clear();
            for (int i = 0; i < numWaves; i++)
            {
                room.waves.Add(new Wave());
                room.waves[i].associatedRoom = room;
            }

            foreach (var wave in room.waves)
            {
                enemyManager.PopulateWave(wave);
                Debugger.Log("Populated wave in Room " + room.roomID + " with " + wave.enemiesLeft + " enemies.", type: DebugType.World);
            }
            

            
            Debugger.Log("Room " + room.roomID + " initialized with " + numWaves + " waves.", type: DebugType.World);
        }
    }
    

    void InitializePlayer()
    {
        if (player == null || startingRoom == null)
            return;

        Vector2Int startPos = startingRoom.GetRoomCenter();
        player.transform.position = new Vector3(startPos.x, startPos.y, 0);
        startingRoom.isEntered = true;
    }

    Room ChooseStartingRoom()
    {
        List<Room> validRooms = rooms.FindAll(room =>
            room.area.xMin > -worldWidth / 2 + 10 &&
            room.area.xMax < worldWidth / 2 - 10 &&
            room.area.yMin > -worldHeight / 2 + 10 &&
            room.area.yMax < worldHeight / 2 - 10
        );

        if (validRooms.Count == 0)
            validRooms = rooms;

        if (validRooms.Count == 0)
            return null; 

        return validRooms[Random.Range(0, validRooms.Count)];
    }
    Room ChooseEndingRoom(Room startRoom)
    {
        Room bestRoom = null;
        float maxDistance = 0f;

        List<Room> validRooms = rooms.FindAll(room =>
            room.area.xMin > -worldWidth / 2 + 10 &&
            room.area.xMax < worldWidth / 2 - 10 &&
            room.area.yMin > -worldHeight / 2 + 10 &&
            room.area.yMax < worldHeight / 2 - 10
        );

        // Fallback if none meet strict criteria
        if (validRooms.Count == 0)
        {
            validRooms = rooms;
            validRooms.Remove(startRoom);
            if (validRooms.Count == 0)
                return null;
        }
           
        foreach (Room room in validRooms)
        {
            float dist = Vector2.Distance(
                room.GetRoomCenter(),
                startRoom.GetRoomCenter()
            );

            if (dist > maxDistance)
            {
                maxDistance = dist;
                bestRoom = room;
            }
        }

        if (bestRoom == null && validRooms.Count > 0)
        {
            bestRoom = validRooms[Random.Range(0, validRooms.Count)];
        }

        return bestRoom;
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

        RectInt roomArea = new RectInt(roomX, roomY, roomWidth, roomHeight);
        Room newRoom = new Room(roomArea, rooms.Count);
        DrawRoom(newRoom);

        int subCellCount = Random.Range(2, 5); 
        var subCells = newRoom.GenerateVoronoiInRoom(subCellCount);

        rooms.Add(newRoom);
        node.room = newRoom;
    }

    void ConnectRooms(BSPNode node)
    {
        if (rooms.Count < 2)
            return;

        for (int i = 0; i < rooms.Count - 1; i++)
        {
            Vector2Int pointA = rooms[i].GetRoomCenter();
            Vector2Int pointB = rooms[i + 1].GetRoomCenter();

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

    void DrawRoom(Room room)
    {
        /* Helper function to draw a room in the tilemap. */
        for (int x = room.area.xMin; x < room.area.xMax; x++)
        {
            for (int y = room.area.yMin; y < room.area.yMax; y++)
            {
                DrawTile(x, y);
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


    void CreateWalls()
    {
        BoundsInt bounds = floorTilemap.cellBounds;
        bounds.yMax += 6;
        bounds.xMin -= 1;
        bounds.xMax += 1;
        bounds.yMin -= 1;

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
                Vector3Int oneUp = pos + Vector3Int.up;
                Vector3Int twoUp = pos + Vector3Int.up * 2;

                bool canPlaceOne = floorTilemap.GetTile(oneUp) == null;
                bool canPlaceTwo = canPlaceOne && floorTilemap.GetTile(twoUp) == null;

                bool placedWall = false;
                int startHeight = 0;

                if (canPlaceTwo)
                {
                    wallTilemap.SetTile(pos, wallTile);
                    wallTilemap.SetTile(oneUp, wallTile);

                    placedWall = true;
                    startHeight = 2;
                }
                else
                {
                    // Always allow base tile if current pos is empty (it is)
                    wallTilemap.SetTile(pos, wallTile);

                    placedWall = true;
                    startHeight = 1;
                }

                if (placedWall)
                {
                    for (int i = startHeight; i < 6; i++)
                    {
                        Vector3Int abovePos = pos + Vector3Int.up * i;

                        if (floorTilemap.GetTile(abovePos) == null)
                            wallTilemap.SetTile(abovePos, wallTopTile);
                        else
                            break;
                    }
                }
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

    void OnDrawGizmos()
    {
        if (rooms == null)
            return;

        foreach (var room in rooms)
        {
            foreach (var cell in room.subCells)
            {
                // Random stable color per cell
                Random.InitState(cell.center.GetHashCode());
                Color baseColor = Color.HSVToRGB(Random.value, 0.6f, 1f);
                baseColor.a = 0.5f;
                Gizmos.color = baseColor;


                // Draw tiles
                foreach (var tile in cell.tiles)
                {
                    Vector3 pos = new Vector3(tile.x + 0.5f, tile.y + 0.5f, 0);
                    Gizmos.DrawCube(pos, Vector3.one * 0.9f);
                }

                // Draw center
                Gizmos.color = Color.red;
                Vector3 centerPos = new Vector3(cell.center.x + 0.5f, cell.center.y + 0.5f, 0);
                Gizmos.DrawSphere(centerPos, 0.3f);
                
            }
        }
    }

    void SealRoom(Room room)
    {
        if (room.isSealed)
            return;

        RectInt area = room.area;

        for (int x = area.xMin; x < area.xMax; x++)
        {
            for (int y = area.yMin; y < area.yMax; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);

                // Only check perimeter
                bool isEdge =
                    x == area.xMin ||
                    x == area.xMax - 1 ||
                    y == area.yMin ||
                    y == area.yMax - 1;

                if (!isEdge)
                    continue;

                // Check outward direction for corridor
                Vector3Int[] directions =
                {
                    Vector3Int.up,
                    Vector3Int.down,
                    Vector3Int.left,
                    Vector3Int.right
                };

                foreach (var dir in directions)
                {
                    Vector3Int outside = pos + dir;

                    // If outside tile is floor but not inside this room
                    if (floorTilemap.GetTile(outside) != null &&
                        !area.Contains((Vector2Int)outside))
                    {
                        barrierTilemap.SetTile(outside, barrierTile);
                        room.barrierPositions.Add(outside);
                    }
                }
            }
        }

        room.isSealed = true;
    }

    void UnsealRoom(Room room)
    {
        foreach (var pos in room.barrierPositions)
        {
            barrierTilemap.SetTile(pos, null);
        }

        room.barrierPositions.Clear();
        room.isSealed = false;
    }
}
