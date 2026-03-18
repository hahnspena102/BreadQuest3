using UnityEngine;
using System.Collections.Generic;


public class EnemyManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyBasePrefab;
    [SerializeField] private EnemyData enemyData;
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
            SpawnEnemy(enemyData, spawnPosition);
        }
    }

    GameObject SpawnEnemy(EnemyData enemyData, Vector3 position)
    {
        GameObject newEnemy = Instantiate(enemyBasePrefab, position, Quaternion.identity);

        newEnemy.transform.SetParent(transform);

        Enemy enemyComponent = newEnemy.GetComponent<Enemy>();
        if (enemyComponent != null)        {
            enemyComponent.EnemyData = enemyData;
            enemyComponent.ApplyEnemyData();
        }

        return newEnemy;
    }

    public void PopulateSubCells(Room room)
    {
        foreach (var cell in room.subCells)
        {
            int enemyCount = Random.Range(0, 2);

            for (int i = 0; i < enemyCount; i++)
            {
                if (cell.tiles.Count == 0)
                    continue;

                Vector2Int spawnTile =
                    cell.tiles[Random.Range(0, cell.tiles.Count)];

                Vector3 worldPos =
                    new Vector3(spawnTile.x + 0.5f, spawnTile.y + 0.5f, 0);

                GameObject enemy = SpawnEnemy(enemyData, worldPos);
                enemy.GetComponent<Enemy>().AssignedRoom = room;
                room.enemiesInRoom.Add(enemy);
            }
        }
    }
}
