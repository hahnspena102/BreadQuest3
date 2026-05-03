using UnityEngine;
using UnityEngine.Tilemaps;
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
        RectInt worldArea = new RectInt(
            -worldWidth / 2,
            -worldHeight / 2,
            worldWidth,
            worldHeight
        );


        /* Generate BSP tree and rooms */
        BSPNode root = new BSPNode(worldArea);
        root.GenerateBSP(minPartitionSize);

        CreateRooms(root);
        ConnectRooms();
        CreateWalls();
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
        Room newRoom = new Room(roomArea, rooms.Count);

        /* Randomly choose a shape for the room and generate its floor tiles. */
        float shapeRoll = Random.value;
        if (shapeRoll < 0.45f)
            newRoom.GenerateRoomShape(RoomShape.Rectangle);
        else if (shapeRoll < 0.65f)
            newRoom.GenerateRoomShape(RoomShape.LShape);
        else if (shapeRoll < 0.80f)
            newRoom.GenerateRoomShape(RoomShape.TShape);
        else
            newRoom.GenerateRoomShape(RoomShape.Rectangle);
            //newRoom.GenerateRoomShape(RoomShape.Organic);

        /* Place the tiles of the room */
        DrawRoom(newRoom);


        /* Generate sub-cells within the room for more varied enemy spawn points. */
        int subCellCount = Random.Range(2, 5);
        newRoom.GenerateVoronoiInRoom(subCellCount);

        /* Add the new room to the list of rooms and assign it to the BSP node. */
        rooms.Add(newRoom);
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
            Vector2Int pointA = rooms[i].GetRoomCenter();
            Vector2Int pointB = rooms[i + 1].GetRoomCenter();

            CreateCorridor(pointA, pointB);
        }
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
        bounds.yMax += 6;
        bounds.xMin -= 1;
        bounds.xMax += 1;
        bounds.yMin -= 1;

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
                    wallTilemap.SetTile(pos, wallTile);
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

    /* Helper function to fill any empty space with walls. */
    private void FillEmptyWithWalls()
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

                if (floorTilemap.GetTile(pos) == null &&
                    wallTilemap.GetTile(pos) == null)
                {
                    wallTilemap.SetTile(pos, wallTopTile);
                }
            }
        }
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
                Random.InitState(cell.center.GetHashCode());
                Color baseColor = Color.HSVToRGB(Random.value, 0.6f, 1f);
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
