using UnityEngine;
using System.Collections.Generic;


public class EnemyManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyBasePrefab;
    [SerializeField] private EnemySpawns enemySpawns;
    private float minSpawnDistanceFromPlayer = 3f;
    private Enemy bossEnemy;

    public EnemySpawns EnemySpawns { get => enemySpawns; set => enemySpawns = value; }
    public Enemy BossEnemy { get => bossEnemy; set => bossEnemy = value; }



    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 spawnPosition = new Vector3(mousePosition.x, mousePosition.y, 0f);
            int tier = GameManager.FloorToTier(FindFirstObjectByType<Player>().PlayerData.CurrentFloor);
            EnemyData enemyData = enemySpawns.GetRandomEnemy(tier);
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
        if (assignedWave != null)
        {
            assignedWave.enemiesLeft++;
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
            GameObject enemy = SpawnEnemy(enemyData, spawnPosition, wave);
            if (wave.bossIndex >= 0 && wave.enemyDataInWave.IndexOf((enemyData, spawnPosition)) == wave.bossIndex)
            {
                Enemy enemyComponent = enemy.GetComponent<Enemy>();
                if (enemyComponent != null)
                {
                    enemyComponent.IsBoss = true;
                    Debugger.Log("Spawned boss: " + enemyComponent.EnemyData.EnemyName, type: DebugType.Enemies);
                }
             
                enemy.transform.position = new Vector3(wave.associatedRoom.GetRoomCenter().x, wave.associatedRoom.GetRoomCenter().y, enemy.transform.position.z);
            }
        }
    }

    public void PopulateWave(Wave wave)
    {
        Room room = wave.associatedRoom;
        Player player = FindFirstObjectByType<Player>();
        
        
        foreach (var subCell in room.subCells)
        {
            int tier = GameManager.FloorToTier(player.PlayerData.CurrentFloor);
            EnemyData enemyData = enemySpawns.GetRandomEnemy(tier);
            //Debugger.Log("Spawning enemy: " + enemyData.EnemyName, type: DebugType.Enemies);
            Vector3 spawnPosition = new Vector3(subCell.center.x, subCell.center.y, 0f);
            wave.enemyDataInWave.Add((enemyData, spawnPosition));
        }
        
    }

    public Enemy GetBoss()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        foreach (Enemy enemy in enemies)
        {
            if (enemy.IsBoss)
            {
                bossEnemy = enemy;
                return enemy;
            }
        }
        bossEnemy = null;
        return null;
        }
        
}
