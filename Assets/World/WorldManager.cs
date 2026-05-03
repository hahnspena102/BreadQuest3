using System.Collections;
using NavMeshPlus.Components;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;


public partial class WorldManager : MonoBehaviour
{
    [SerializeField] private int worldWidth;
    [SerializeField] private int worldHeight;
    [SerializeField] private NavMeshSurface navMeshSurface;
    [SerializeField] private int minPartitionSize;
    [SerializeField] private int minRoomWidth;
    [SerializeField] private int minRoomHeight;
    [SerializeField] private int maxRoomWidth = 20;
    [SerializeField] private int maxRoomHeight = 16;
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
    private EnemyManager enemyManager;
    void Start()
    {
        enemyManager = FindFirstObjectByType<EnemyManager>();
        player = FindFirstObjectByType<Player>();
        
        if (navMeshSurface != null)
            navMeshSurface.hideEditorLogs = true;
        
        int tier = GameManager.FloorToTier(player.PlayerData.CurrentFloor);
        Debugger.Log("Player is on floor " + player.PlayerData.CurrentFloor + ", tier " + tier + ".", type: DebugType.World);
            
        StartCoroutine(BuildWorld());
    }

    void Update()
    {
        UpdateRoomStates();
    }
}
