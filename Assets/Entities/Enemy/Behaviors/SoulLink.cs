using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "SoulLink", menuName = "Scriptable Objects/Behaviors/SoulLink")]
public class SoulLink : EnemyBehavior
{

    [SerializeField]private EnemyData soulLinkData;

    public override float PerformBehavior(Enemy enemy, float behaviorDuration)
    {
        enemy.StartCoroutine(SoulLinkRoutine(enemy, behaviorDuration));
        return behaviorDuration;
    }

    private IEnumerator SoulLinkRoutine(Enemy enemy, float duration)
    {
        EnemyManager manager = GameObject.FindFirstObjectByType<EnemyManager>();
        if (manager == null)
        {
            Debug.LogWarning("EnemyManager not found in scene");
            yield break;
        }

        GameObject soulLinkTarget = manager.SpawnEnemy(soulLinkData, enemy.transform.position, enemy.AssignedWave);

        Enemy linkedEnemy = soulLinkTarget.GetComponent<Enemy>();
        if (linkedEnemy == null)
        {
            Debug.LogWarning("Spawned soul link target does not have an Enemy component");
            yield break;
        }
        enemy.SetLinkedEnemy(linkedEnemy);
        linkedEnemy.SetLinkedEnemy(enemy);
        
        yield return new WaitForSeconds(duration);

        
    }
}