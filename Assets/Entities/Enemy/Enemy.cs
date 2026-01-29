using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [SerializeField]private EnemyData enemy;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(BehaviorLoop());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator BehaviorLoop() {
        EnemyBehavior eb = enemy.EnemyBehaviors[Random.Range(0,enemy.EnemyBehaviors.Count)];
        eb.PerformBehavior(gameObject);
        yield return new WaitForSeconds(1f);
        StartCoroutine(BehaviorLoop());

    }
}
