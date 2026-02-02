using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ShootProjectile))]
public class ShootProjectileEditor : Editor
{
    SerializedProperty targetMode;
    SerializedProperty targetTag;
    SerializedProperty targetDelta;

    private void OnEnable()
    {
        targetMode = serializedObject.FindProperty("targetMode");
        targetTag = serializedObject.FindProperty("targetTag");
        targetDelta = serializedObject.FindProperty("targetDelta");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // 🔹 Draw all base & shared fields (including behaviorDuration)
        DrawPropertiesExcluding(
            serializedObject,
            "targetMode",
            "targetTag",
            "targetDelta"
        );

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(targetMode);
        EditorGUILayout.Space();

        TargetMode mode = (TargetMode)targetMode.enumValueIndex;

        switch (mode)
        {
            case TargetMode.Tag:
                EditorGUILayout.PropertyField(targetTag);
                break;

            case TargetMode.Delta:
                EditorGUILayout.PropertyField(targetDelta);
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
