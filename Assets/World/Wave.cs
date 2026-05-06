using UnityEngine;
using System.Collections.Generic;

public class Wave
{
    public List<(EnemyData, Vector3)> enemyDataInWave = new List<(EnemyData, Vector3)>();
    public bool isSpawned = false;
    public bool isCleared = false;
    public Room associatedRoom;
    public int enemiesLeft = 0;
    public int bossIndex = -1;

    public Wave()
    {
        this.enemyDataInWave = new List<(EnemyData, Vector3)>();
    }


    public EnemyData GetPossibleBoss(List<EnemyData> possibleBosses = null)
    {
      
        if (possibleBosses.Count == 0)
        {
            return null;
        }
        return possibleBosses[Random.Range(0, possibleBosses.Count)];
    }

    public void AddBoss(List<EnemyData> possibleBosses)
    {
        EnemyData bossData = GetPossibleBoss(possibleBosses);
        if (bossData == null)        {
            Debug.LogWarning("No valid boss candidates in wave");
            return;
        }

        // isSpawned in center of room
        Vector3 spawnPosition = new Vector3(associatedRoom.GetRoomCenter().x, associatedRoom.GetRoomCenter().y, 0f);
        enemyDataInWave.Add((bossData, spawnPosition));
        bossIndex = enemyDataInWave.Count - 1; 
    }


    
}