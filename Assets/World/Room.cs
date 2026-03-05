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

public class Room
{
    public int roomID;
    public RectInt area;
    public List<RoomSubCell> subCells = new List<RoomSubCell>();
    public bool isEntered = false;

    public List<Vector3Int> barrierPositions = new List<Vector3Int>();
    public List<GameObject> enemiesInRoom = new List<GameObject>();
    public bool isSealed = false;

    public Room(RectInt area, int roomID = -1)
    {
        this.roomID = roomID;
        this.area = area;
        this.subCells = new List<RoomSubCell>();
        this.enemiesInRoom = new List<GameObject>();
    }


    public List<RoomSubCell> GenerateVoronoiInRoom(int seedCount)
    {
        List<Vector2Int> seeds = new List<Vector2Int>();
        List<RoomSubCell> cells = new List<RoomSubCell>();

        // Create seeds inside room
        for (int i = 0; i < seedCount; i++)
        {
            Vector2Int seed = new Vector2Int(
                Random.Range(area.xMin + 1, area.xMax - 1),
                Random.Range(area.yMin + 1, area.yMax - 1)
            );

            seeds.Add(seed);
            cells.Add(new RoomSubCell { center = seed });
        }

        // Assign tiles in room to nearest seed
        for (int x = area.xMin; x < area.xMax; x++)
        {
            for (int y = area.yMin; y < area.yMax; y++)
            {
                Vector2Int tile = new Vector2Int(x, y);

                int closestIndex = 0;
                float closestDist = float.MaxValue;

                for (int i = 0; i < seeds.Count; i++)
                {
                    float dist = (tile - seeds[i]).sqrMagnitude; 
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        closestIndex = i;
                    }
                }

                    cells[closestIndex].tiles.Add(tile);
                }
            }

        subCells = cells;
        return subCells;

    }

    public Vector2Int GetRoomCenter()
    {
        return new Vector2Int(
            area.xMin + area.width / 2,
            area.yMin + area.height / 2
        );
    }
}