using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.AI;

[CustomEditor(typeof(Enemy))]
public class EnemyEditor : Editor
{
    SerializedProperty enemyDataProp;

    private Enemy enemy;

    private void OnEnable()
    {
        enemy = (Enemy)target;
        enemyDataProp = serializedObject.FindProperty("enemyData");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        EditorGUILayout.PropertyField(enemyDataProp);

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            ApplyEnemyData();
        }

        DrawPropertiesExcluding(serializedObject, "enemyData");

        serializedObject.ApplyModifiedProperties();
    }

    void ApplyEnemyData()
    {
        if (enemy.EnemyData == null)
            return;

        EnemyData data = enemy.EnemyData;


        GameObject enemyObj = enemy.gameObject;
        enemyObj.name = data.EnemyName;

        SpriteRenderer sr = enemy.GetComponent<SpriteRenderer>();
        if (sr != null && data.EnemySprite)
        {
            sr.sprite = data.EnemySprite;
        }

        EnemyAnimator animator = enemy.GetComponent<EnemyAnimator>();
        if (animator != null)
        {
            animator.EnemyData = data;
        }
        ApplyAnimatorOverrides(data);



        EditorUtility.SetDirty(enemy);
    }

    void ApplyAnimatorOverrides(EnemyData data)
    {
        Animator animator = enemy.GetComponent<Animator>();
        EnemyAnimator enemyAnimator = enemy.GetComponent<EnemyAnimator>();

        if (animator == null || enemyAnimator == null)
            return;

        if (data.AnimatorOverride)
        {
            animator.runtimeAnimatorController = data.AnimatorOverride;
            Debug.Log("Applied animator override: " + data.AnimatorOverride.name);
            return;
        }

        RuntimeAnimatorController baseController = animator.runtimeAnimatorController;
        if (baseController == null)
            return;

        AnimatorOverrideController overrideController;

        if (animator.runtimeAnimatorController is AnimatorOverrideController existing)
        {
            overrideController = existing;
        }
        else
        {
            overrideController = new AnimatorOverrideController(baseController);
            animator.runtimeAnimatorController = overrideController;
        }

        if (enemyAnimator.IdleFPlaceholder && data.IdleAnimationF)
            overrideController[enemyAnimator.IdleFPlaceholder] = data.IdleAnimationF;

        if (enemyAnimator.IdleBPlaceholder && data.IdleAnimationB)
            overrideController[enemyAnimator.IdleBPlaceholder] = data.IdleAnimationB;
        if (enemyAnimator.MoveFPlaceholder && data.MoveAnimationF)
            overrideController[enemyAnimator.MoveFPlaceholder] = data.MoveAnimationF;
        if (enemyAnimator.MoveBPlaceholder && data.MoveAnimationB)
            overrideController[enemyAnimator.MoveBPlaceholder] = data.MoveAnimationB;
        if (enemyAnimator.AttackFPlaceholder && data.AttackAnimationF)
            overrideController[enemyAnimator.AttackFPlaceholder] = data.AttackAnimationF;
        if (enemyAnimator.AttackBPlaceholder && data.AttackAnimationB)
            overrideController[enemyAnimator.AttackBPlaceholder] = data.AttackAnimationB;

        Transform shadowTransform = enemy.ShadowTransform;

        if (shadowTransform != null)
        {
            shadowTransform.localPosition = data.ShadowOffset;
            shadowTransform.localScale = data.ShadowScale;
        }

        EditorUtility.SetDirty(animator);

        
    }
    
}
#endif