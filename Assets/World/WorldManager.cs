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

    public Tilemap targetTilemap;
    public TileBase tileToDraw;

    void Start()
    {
        StartCoroutine(BuildWorld());
    }

    IEnumerator BuildWorld()
    {
        GenerateMap();

        targetTilemap.RefreshAllTiles();
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

    void DrawTile(int x, int y)
    {
        Vector3Int position = new Vector3Int(x, y, 0);
        targetTilemap.SetTile(position, tileToDraw);
    }

    void GenerateMap()
    {
        targetTilemap.ClearAllTiles();
        rooms.Clear();

        RectInt worldArea = new RectInt(
            -worldWidth / 2,
            -worldHeight / 2,
            worldWidth,
            worldHeight
        );

        BSPNode root = GenerateBSP(worldArea);
        CreateRooms(root);
        DrawRooms();
    }
    void CreateRooms(BSPNode node)
    {
        if (!node.IsLeaf)
        {
            CreateRooms(node.left);
            CreateRooms(node.right);
            return;
        }

        int roomWidth = Random.Range(minRoomSize, node.area.width - 2);
        int roomHeight = Random.Range(minRoomSize, node.area.height - 2);

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

    void DrawRooms()
    {
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

    

    BSPNode GenerateBSP(RectInt area)
    {
        BSPNode node = new BSPNode(area);

        if (area.width < minPartitionSize * 2 &&
            area.height < minPartitionSize * 2)
            return node;

        bool splitHorizontally = Random.value > 0.5f;

        if (area.width > area.height && area.width / area.height >= 1.25f)
            splitHorizontally = false;
        else if (area.height > area.width && area.height / area.width >= 1.25f)
            splitHorizontally = true;

        int max = (splitHorizontally ? area.height : area.width) - minPartitionSize;
        if (max <= minPartitionSize)
            return node;

        int split = Random.Range(minPartitionSize, max);

        if (splitHorizontally)
        {
            node.left = GenerateBSP(new RectInt(
                area.x,
                area.y,
                area.width,
                split
            ));

            node.right = GenerateBSP(new RectInt(
                area.x,
                area.y + split,
                area.width,
                area.height - split
            ));
        }
        else
        {
            node.left = GenerateBSP(new RectInt(
                area.x,
                area.y,
                split,
                area.height
            ));

            node.right = GenerateBSP(new RectInt(
                area.x + split,
                area.y,
                area.width - split,
                area.height
            ));
        }

        return node;
    }

}
