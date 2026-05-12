using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public partial class WorldManager
{
    private IEnumerator BuildWorld()
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
        Debugger.Log("Starting Room: " + startingRoom.roomID + " | Ending Room: " + endingRoom.roomID, type: DebugType.World);
        

        InitializeRooms();
        InitializePlayer();
    }

    private void InitializeRooms()
    {
        int numFlavors = Random.Range(2, 4);
        // if tier 1, all flavors
        if (GameManager.FloorToTier(player.PlayerData.CurrentFloor) <= 2)
        {
            numFlavors = possibleFlavors.Length;
        }
        Flavor[] flavors = new Flavor[numFlavors];
        // dont allow duplicates
        for (int i = 0; i < numFlavors; i++)
        {
            Flavor flavor;
            do
            {
                flavor = possibleFlavors[Random.Range(0, possibleFlavors.Length)];
            }
            while (System.Array.IndexOf(flavors, flavor) != -1);
            flavors[i] = flavor;
        }

        Debugger.Log("Assigned floor flavors: " + string.Join(", ", (object[])flavors), type: DebugType.World);


        foreach (var room in rooms)
        {
            room.isEntered = false;
            room.isCleared = false;
            room.isSealed = false;
            room.barrierPositions.Clear();

            if (room == startingRoom)
            {
                continue;
            }

            int numWaves = isBossFloor && room == endingRoom ? 1 : Random.Range(1, 4);
            room.waves.Clear();
            for (int i = 0; i < numWaves; i++)
            {
                room.waves.Add(new Wave());
                room.waves[i].associatedRoom = room;
            }

           
            foreach (var wave in room.waves)
            {
                enemyManager.PopulateWave(wave, flavors);
            }

            if (isBossFloor) {
                if (room == endingRoom)
                {
                    /* pick random enemy from wave and make it a boss */
                    Wave lastWave = room.waves[room.waves.Count - 1];
                    lastWave.AddBoss(enemyManager.EnemySpawns.GetBossForTier(GameManager.FloorToTier(player.PlayerData.CurrentFloor)));
                    lastWave.bossIndex = lastWave.enemyDataInWave.Count - 1;

                    int additionalWaves = Random.Range(1, 3);
                    for (int i = 0; i < additionalWaves; i++)
                    {
                        Wave extraWave = new Wave();
                        extraWave.associatedRoom = room;
                        enemyManager.PopulateWave(extraWave, flavors);
                        room.waves.Add(extraWave);
                    }
                    
                }
            }

            string waveInfo = "";
            for (int i = 0; i < room.waves.Count; i++)
            {
                string enemyList = "";
                foreach (var (enemyData, spawnPos) in room.waves[i].enemyDataInWave)
                {
                    enemyList += enemyData.EnemyName + ", ";
                }
                waveInfo += $"Wave {i+1}: [{enemyList.TrimEnd(',', ' ')}]; ";
            }
            Debugger.Log("Room " + room.roomID + ": [" + waveInfo + "]", type: DebugType.World);
        }
    }

    private void InitializePlayer()
    {
        if (player == null || startingRoom == null)
        {
            return;
        }

        Vector2Int startPos = startingRoom.GetRoomCenter();
        player.transform.position = new Vector3(startPos.x, startPos.y, 0);
        startingRoom.isEntered = true;
    }

    private Room ChooseStartingRoom()
    {
        List<Room> validRooms = GetInteriorRooms();

        if (validRooms.Count == 0)
        {
            validRooms = rooms;
        }

        if (validRooms.Count == 0)
        {
            return null;
        }

        if (isBossFloor)
        {
            Room smallestRoom = validRooms[0];
            int smallestArea = smallestRoom.area.width * smallestRoom.area.height;

            for (int i = 1; i < validRooms.Count; i++)
            {
                Room room = validRooms[i];
                int roomArea = room.area.width * room.area.height;

                if (roomArea < smallestArea)
                {
                    smallestArea = roomArea;
                    smallestRoom = room;
                }
            }

            return smallestRoom;
        }

        return validRooms[Random.Range(0, validRooms.Count)];
    }

    private Room ChooseEndingRoom(Room startRoom)
    {
        if (isBossFloor)
        {
            Room largestRoom = null;
            int largestArea = 0;

            foreach (Room room in rooms)
            {
                if (room == startRoom)
                {
                    continue;
                }

                int roomArea = room.area.width * room.area.height;
                if (roomArea > largestArea)
                {
                    largestArea = roomArea;
                    largestRoom = room;
                }
            }

            if (largestRoom != null)
            {
                return largestRoom;
            }
        }

        Room bestRoom = null;
        float maxDistance = 0f;

        List<Room> validRooms = GetInteriorRooms();
        validRooms.Remove(startRoom);

        if (validRooms.Count == 0)
        {
            validRooms = new List<Room>(rooms);
            validRooms.Remove(startRoom);

            if (validRooms.Count == 0)
            {
                return null;
            }
        }

        foreach (Room room in validRooms)
        {
            float dist = Vector2.Distance(room.GetRoomCenter(), startRoom.GetRoomCenter());

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

    private void EnsureEndingRoomTeleporter()
    {
        if (teleporterPrefab == null || endingRoom == null || !endingRoom.isCleared)
        {
            return;
        }

        if (HasTeleporterInRoom(endingRoom))
        {
            return;
        }

        Vector2 spawnPos = endingRoom.subCells.Count > 0
            ? endingRoom.subCells[Random.Range(0, endingRoom.subCells.Count)].center
            : endingRoom.GetRoomCenter();
        spawnPos += new Vector2(0.5f, 0.5f);

        Instantiate(teleporterPrefab, spawnPos, Quaternion.identity);
        Debugger.LogWarning("Teleporter fallback spawned in ending room.", type: DebugType.World);
    }

    private bool HasTeleporterInRoom(Room room)
    {
        Teleporter[] teleporters = FindObjectsByType<Teleporter>(FindObjectsSortMode.None);
        for (int i = 0; i < teleporters.Length; i++)
        {
            Teleporter teleporter = teleporters[i];
            if (teleporter == null)
            {
                continue;
            }

            Vector2Int teleporterPos = Vector2Int.FloorToInt(teleporter.transform.position);
            if (room.IsPointInRoom(teleporterPos))
            {
                return true;
            }
        }

        return false;
    }

    private void CreateChest(Room room)
    {
        Vector2Int center = room.GetRoomCenter();
        Vector3 chestPos = new Vector3(center.x, center.y, 0);
        GameObject chestObj = Instantiate(chestPrefab, chestPos, Quaternion.identity);
        chestObj.transform.SetParent(transform);
    }

    private void SealRoom(Room room)
    {
        if (room.isSealed)
        {
            return;
        }
        HashSet<Vector2Int> roomTiles = room.floorTiles.Count > 0
            ? new HashSet<Vector2Int>(room.floorTiles)
            : GetRectangleTiles(room.area);

        // Collect immediate outside candidates: tiles adjacent to the room that are floor
        HashSet<Vector2Int> outsideCandidates = new HashSet<Vector2Int>();
        Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        foreach (var inside in roomTiles)
        {
            foreach (var d in dirs)
            {
                Vector2Int outside = inside + d;
                if (roomTiles.Contains(outside))
                    continue;

                Vector3Int outside3 = new Vector3Int(outside.x, outside.y, 0);
                if (floorTilemap.GetTile(outside3) != null)
                {
                    outsideCandidates.Add(outside);
                }
            }
        }

        // Cluster outside candidates into contiguous components and place barriers on each tile in a component
        HashSet<Vector3Int> placedBarriers = new HashSet<Vector3Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        foreach (var start in outsideCandidates)
        {
            if (visited.Contains(start))
                continue;

            // BFS to collect component
            List<Vector2Int> component = new List<Vector2Int>();
            Queue<Vector2Int> q = new Queue<Vector2Int>();
            q.Enqueue(start);
            visited.Add(start);

            while (q.Count > 0)
            {
                Vector2Int cur = q.Dequeue();
                component.Add(cur);

                foreach (var d in dirs)
                {
                    Vector2Int n = cur + d;
                    if (visited.Contains(n) || !outsideCandidates.Contains(n))
                        continue;

                    visited.Add(n);
                    q.Enqueue(n);
                }
            }

            // Place barriers for the component (cover the entire entrance footprint)
            foreach (var outside in component)
            {
                Vector3Int barrierPos = new Vector3Int(outside.x, outside.y, 0);
                if (placedBarriers.Add(barrierPos))
                {
                    barrierTilemap.SetTile(barrierPos, barrierTile);
                    room.barrierPositions.Add(barrierPos);
                    CreateBarrierObstacleAt(barrierPos);
                }
            }
        }

        room.isSealed = true;
    }
    
    private HashSet<Vector2Int> GetRectangleTiles(RectInt area)
    {
        HashSet<Vector2Int> tiles = new HashSet<Vector2Int>();
        for (int x = area.xMin; x < area.xMax; x++)
        {
            for (int y = area.yMin; y < area.yMax; y++)
            {
                tiles.Add(new Vector2Int(x, y));
            }
        }

        return tiles;
    }

    private void UnsealRoom(Room room)
    {
        foreach (var pos in room.barrierPositions)
        {
            barrierTilemap.SetTile(pos, null);
            DestroyBarrierObstacleAt(pos);
        }

        room.barrierPositions.Clear();
        room.isSealed = false;
    }

    private void CreateBarrierObstacleAt(Vector3Int cellPos)
    {
        if (barrierTilemap == null) return;

        string name = $"BarrierObstacle_{cellPos.x}_{cellPos.y}";
        if (transform.Find(name) != null) return;

        Vector3 worldPos = barrierTilemap.GetCellCenterWorld(cellPos);

        GameObject go = new GameObject(name);
        go.transform.SetParent(transform);
        go.transform.position = new Vector3(worldPos.x, worldPos.y, 0f);

        // 2D physical blocker
        var box = go.AddComponent<BoxCollider2D>();
        box.size = Vector2.one;
        var rb = go.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static;

        // NavMesh obstacle for agent carving
        var obstacle = go.AddComponent<NavMeshObstacle>();
        obstacle.carving = true;
        obstacle.shape = NavMeshObstacleShape.Box;
        obstacle.size = new Vector3(1f, 1f, 1f);
    }

    private void DestroyBarrierObstacleAt(Vector3Int cellPos)
    {
        string name = $"BarrierObstacle_{cellPos.x}_{cellPos.y}";
        Transform t = transform.Find(name);
        if (t != null)
        {
            DestroyImmediate(t.gameObject);
        }
    }

    private List<Room> GetInteriorRooms()
    {
        return rooms.FindAll(room =>
            room.area.xMin > -worldWidth / 2 + 10 &&
            room.area.xMax < worldWidth / 2 - 10 &&
            room.area.yMin > -worldHeight / 2 + 10 &&
            room.area.yMax < worldHeight / 2 - 10
        );
    }
}