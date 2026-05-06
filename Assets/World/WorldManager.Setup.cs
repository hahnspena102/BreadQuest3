using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                enemyManager.PopulateWave(wave);
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
                        enemyManager.PopulateWave(extraWave);
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

        RectInt area = room.area;
        HashSet<Vector2Int> roomTiles = room.floorTiles.Count > 0
            ? new HashSet<Vector2Int>(room.floorTiles)
            : GetRectangleTiles(area);

        foreach (Vector2Int tile in roomTiles)
        {
            Vector3Int pos = new Vector3Int(tile.x, tile.y, 0);

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
                Vector2Int outsidePos = new Vector2Int(outside.x, outside.y);

                if (floorTilemap.GetTile(outside) != null && !room.IsPointInRoom(outsidePos))
                {
                    barrierTilemap.SetTile(outside, barrierTile);
                    room.barrierPositions.Add(outside);
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
        }

        room.barrierPositions.Clear();
        room.isSealed = false;
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