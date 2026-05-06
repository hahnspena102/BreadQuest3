using UnityEngine;

public partial class WorldManager
{
    public float timeBeforeUpdatingRoomStates = 0.5f;
    private float roomStateTimer = 0f;
    private Enemy bossEnemy;
    /*Update the states of all rooms in the world. */
    private void UpdateRoomStates()
    {
        if (roomStateTimer < timeBeforeUpdatingRoomStates)
        {
            roomStateTimer += Time.deltaTime;
            return;
        }
        
        if (player == null)
        {
            return;
        }

        Vector2Int playerPos = Vector2Int.FloorToInt(player.transform.position);

        foreach (Room room in rooms)
        {
            bool wavesCompleted = room.waves.TrueForAll(wave => wave.isCleared);
            if (room.isEntered && room.isSealed && wavesCompleted)
            {
                Debugger.Log("Cleared Room " + room.roomID, type: DebugType.World);
                room.isCleared = true;
                CreateChest(room);

                if (room == endingRoom)
                {
                    Debugger.Log("Player has cleared the final room!", type: DebugType.World);
                    EnsureEndingRoomTeleporter();
                }

                UnsealRoom(room);
            }

            if (!room.IsPointInRoom(playerPos))
            {
                continue;
            }

            if (!room.isEntered)
            {
                Debugger.Log("Entered Room " + room.roomID, type: DebugType.World);
                SealRoom(room);
                room.isEntered = true;
            }

            if (room.currentWaveIndex >= room.waves.Count)
            {
                continue;
            }

            Wave currentWave = room.waves[room.currentWaveIndex];

            if (!currentWave.isSpawned)
            {
                Debugger.Log("Spawning wave " + room.currentWaveIndex + " in Room " + room.roomID, type: DebugType.Enemies);
                enemyManager.SpawnInWave(currentWave);
                bossEnemy = enemyManager.GetBoss();
                Debug.Log("Boss enemy in this wave: " + (bossEnemy != null ? bossEnemy.EnemyData.EnemyName : "None"));
    
                currentWave.isSpawned = true;
            }

            if (currentWave.isSpawned && !currentWave.isCleared)
            {
                float thresholdPercentage = 1.0f / room.waves.Count;  
                float nextWaveThreshold = 1 - thresholdPercentage * (room.currentWaveIndex + 1);
                float currentPercentage = -1f;

                if (bossEnemy == null)
                {
                    bossEnemy = enemyManager.GetBoss();
                } else {
                    currentPercentage = bossEnemy.GetHealthPercentage();
                }
         
                if (isBossFloor) {
                    if (room.currentWaveIndex == room.waves.Count - 1)
                    {
                        int totalEnemies = 0;
                        foreach (Wave wave in room.waves)
                        {
                            Debug.Log($"Wave has {wave.enemyDataInWave.Count} enemies.");
                            totalEnemies += wave.enemiesLeft;
                        }

                        Debug.Log("total enemies in all waves: " + totalEnemies);
                        bool allEnemiesDefeated = totalEnemies <= 0;
                        if (allEnemiesDefeated)
                        {
                            Debugger.Log("Boss defeated in Room " + room.roomID, type: DebugType.Enemies);
                            currentWave.isCleared = true;
                            GameManager gameManager = FindFirstObjectByType<GameManager>();
                            if (gameManager != null)
                            {
                                gameManager.OnBossDefeated();
                            }
                        }
                    } 
                    else 
                    {
                        if (currentPercentage <= nextWaveThreshold)
                        {
                            Debugger.Log("Boss health at " + (int)(currentPercentage * 100) + "%, triggering next wave in Room " + room.roomID, type: DebugType.Enemies);
                            currentWave.isCleared = true;
                        }
                    }
                    

                } else {
                    bool allEnemiesDefeated = currentWave.enemiesLeft <= 0;

                    if (allEnemiesDefeated)
                    {
                        Debugger.Log("Room " + room.roomID + ": wave " + (room.currentWaveIndex + 1) + "/" + room.waves.Count + " cleared.", type: DebugType.World);
                        currentWave.isCleared = true;
                    }
                }
            }

            if (currentWave.isCleared)
            {
                room.currentWaveIndex = Mathf.Min(room.currentWaveIndex + 1, room.waves.Count - 1);
            }
        }

        EnsureEndingRoomTeleporter();
    }

}