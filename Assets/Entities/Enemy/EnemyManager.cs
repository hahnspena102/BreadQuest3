using UnityEngine;
using System.Collections.Generic;


public class EnemyManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyBasePrefab;
    [SerializeField] private EnemySpawns enemySpawns;
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

    GameObject SpawnEnemy(EnemyData enemyData, Vector3 position, Wave assignedWave)
    {
        GameObject newEnemy = Instantiate(enemyBasePrefab, position, Quaternion.identity);

        newEnemy.transform.SetParent(transform);

        Enemy enemyComponent = newEnemy.GetComponent<Enemy>();
        if (enemyComponent != null) {
            enemyComponent.EnemyData = enemyData;
            enemyComponent.AssignedWave = assignedWave;
            enemyComponent.ApplyEnemyData();
        }

        return newEnemy;
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
