using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(Move))]
public class MoveEditor : Editor
{
    SerializedProperty targetMode;
    SerializedProperty delta;

    SerializedProperty targetTag;
    SerializedProperty towardsTarget;
    SerializedProperty fleeDistance;

    private void OnEnable()
    {
        targetMode = serializedObject.FindProperty("targetMode");
        delta = serializedObject.FindProperty("delta");

        targetTag = serializedObject.FindProperty("targetTag");
        towardsTarget = serializedObject.FindProperty("towardsTarget");
        fleeDistance = serializedObject.FindProperty("fleeDistance");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // 🔹 Draw base & shared fields (EnemyBehavior stuff like behaviorDuration)
        DrawPropertiesExcluding(
            serializedObject,
            "targetMode",
            "delta",
            "targetTag",
            "towardsTarget",
            "fleeDistance"
        );

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(targetMode);
        EditorGUILayout.Space();

        MoveTargetMode mode = (MoveTargetMode)targetMode.enumValueIndex;

        switch (mode)
        {
            case MoveTargetMode.Delta:
                EditorGUILayout.PropertyField(delta);
                break;

            case MoveTargetMode.Tag:
                EditorGUILayout.PropertyField(targetTag);
                EditorGUILayout.PropertyField(towardsTarget);

                if (!towardsTarget.boolValue)
                {
                    EditorGUILayout.PropertyField(fleeDistance);
                }
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
