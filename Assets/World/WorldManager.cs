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
    [SerializeField]private bool isBossFloor = true;
    [SerializeField] private Flavor[] possibleFlavors;
    [SerializeField][ReadOnly] private Flavor[] currentFlavors;

    public Tilemap floorTilemap;
    public Tilemap wallTilemap;
    public Tilemap barrierTilemap;
    public Tilemap decorTilemap;
    public TileBase floorTile;
    public TileBase wallTile;
    public TileBase smallWallTile;
    public TileBase wallTopTile;
    public TileBase barrierTile;
    public TileBase[] natureDecorTiles; 

    public Room startingRoom;
    public Room endingRoom;
    private EnemyManager enemyManager;

    public Flavor[] CurrentFlavors { get => currentFlavors; set => currentFlavors = value; }

    void Start()
    {
        enemyManager = FindFirstObjectByType<EnemyManager>();
        player = FindFirstObjectByType<Player>();
        
        if (navMeshSurface != null)
            navMeshSurface.hideEditorLogs = true;
        
        int tier = GameManager.FloorToTier(player.PlayerData.CurrentFloor);
        Debugger.Log("Player is on floor " + player.PlayerData.CurrentFloor + ", tier " + tier + ".", type: DebugType.World);
        
        // every 5th floor is a boss floor, starting with floor 5
        if (player.PlayerData.CurrentFloor > 0 && player.PlayerData.CurrentFloor % 5 == 0)
        {
            isBossFloor = true;
        } else
        {
            isBossFloor = false;
          
        }
        
         int numFlavors = Random.Range(2, 4);
        // if tier 1, all flavors
        if (GameManager.FloorToTier(player.PlayerData.CurrentFloor) <= 2)
        {
            numFlavors = possibleFlavors.Length;
        }
    
        currentFlavors = new Flavor[numFlavors];
        // dont allow duplicates
        for (int i = 0; i < numFlavors; i++)
        {
            Flavor flavor;
            do
            {
                flavor = possibleFlavors[Random.Range(0, possibleFlavors.Length)];
            }
            while (System.Array.IndexOf(currentFlavors, flavor) != -1);
            
            currentFlavors[i] = flavor;
        }

        Debugger.Log("Assigned floor flavors: " + string.Join(", ", (object[])currentFlavors), type: DebugType.World);


        StartCoroutine(BuildWorld());
    }

    void Update()
    {
        UpdateRoomStates();
    }
}
