using UnityEngine;
using System.Collections.Generic;

public class Wave
{
    public List<(EnemyData, Vector3)> enemyDataInWave = new List<(EnemyData, Vector3)>();
    public bool isSpawned = false;
    public bool isCleared = false;
    public Room associatedRoom;
    public int enemiesLeft = 0;

    public Wave()
    {
        this.enemyDataInWave = new List<(EnemyData, Vector3)>();
    }

    
}