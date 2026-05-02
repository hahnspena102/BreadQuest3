using UnityEngine;
using System.Collections.Generic;


public class EnemyManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyBasePrefab;
    [SerializeField] private EnemySpawns enemySpawns;
    private float minSpawnDistanceFromPlayer = 3f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 spawnPosition = new Vector3(mousePosition.x, mousePosition.y, 0f);
            EnemyData enemyData = enemySpawns.GetRandomEnemy();
            SpawnEnemy(enemyData, spawnPosition, null);
        }
    }

    public GameObject SpawnEnemy(EnemyData enemyData, Vector3 position, Wave assignedWave)
    {
        position = EnforceMinDistanceFromPlayer(position);
        GameObject newEnemy = Instantiate(enemyBasePrefab, position, Quaternion.identity);

        newEnemy.transform.SetParent(transform);

        Enemy enemyComponent = newEnemy.GetComponent<Enemy>();
        if (enemyComponent != null) {
            enemyComponent.EnemyData = enemyData;
            enemyComponent.AssignedWave = assignedWave;
            enemyComponent.AssignedRoom = assignedWave != null ? assignedWave.associatedRoom : null;
            enemyComponent.ApplyEnemyData();
        }

        return newEnemy;
    }

    private Vector3 EnforceMinDistanceFromPlayer(Vector3 spawnPosition)
    {
        Player player = FindFirstObjectByType<Player>();
        if (player == null || minSpawnDistanceFromPlayer <= 0f)
        {
            return spawnPosition;
        }

        Vector2 playerPosition = player.transform.position;
        Vector2 desiredPosition = new Vector2(spawnPosition.x, spawnPosition.y);
        Vector2 offset = desiredPosition - playerPosition;
        float minDistanceSqr = minSpawnDistanceFromPlayer * minSpawnDistanceFromPlayer;

        if (offset.sqrMagnitude >= minDistanceSqr)
        {
            return spawnPosition;
        }

        if (offset.sqrMagnitude < 0.0001f)
        {
            offset = Random.insideUnitCircle;
            if (offset.sqrMagnitude < 0.0001f)
            {
                offset = Vector2.right;
            }
        }

        Vector2 clampedPosition = playerPosition + offset.normalized * minSpawnDistanceFromPlayer;
        return new Vector3(clampedPosition.x, clampedPosition.y, spawnPosition.z);
    }


    public void SpawnInWave(Wave wave)
    {
        foreach (var (enemyData, spawnPosition) in wave.enemyDataInWave)
        {
            SpawnEnemy(enemyData, spawnPosition, wave);
        }
    }

    public void PopulateWave(Wave wave)
    {
        Room room = wave.associatedRoom;
        
        
        foreach (var subCell in room.subCells)
        {
            EnemyData enemyData = enemySpawns.GetRandomEnemy();
            Debugger.Log("Spawning enemy: " + enemyData.EnemyName, type: DebugType.Enemies);
            Vector3 spawnPosition = new Vector3(subCell.center.x, subCell.center.y, 0f);
            wave.enemyDataInWave.Add((enemyData, spawnPosition));
            wave.enemiesLeft++;
        }
        
    }
    
}
