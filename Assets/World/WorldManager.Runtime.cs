using UnityEngine;

public partial class WorldManager
{
    /*Update the states of all rooms in the world. */
    private void UpdateRoomStates()
    {
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
                currentWave.isSpawned = true;
            }

            if (currentWave.isSpawned && !currentWave.isCleared)
            {
                bool allEnemiesDefeated = currentWave.enemiesLeft <= 0;

                if (allEnemiesDefeated)
                {
                    Debugger.Log("Room " + room.roomID + ": wave " + (room.currentWaveIndex + 1) + "/" + room.waves.Count + " cleared.", type: DebugType.World);
                    currentWave.isCleared = true;
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