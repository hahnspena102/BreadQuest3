using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


[System.Serializable]
public struct MinMaxFloat
{
    public float min;
    public float max;

    public float RandomValue => Random.Range(min, max);
}

public class MinMaxAttribute : PropertyAttribute
{
    public float minLimit;
    public float maxLimit;

    public MinMaxAttribute(float minLimit, float maxLimit)
    {
        this.minLimit = minLimit;
        this.maxLimit = maxLimit;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(MinMaxAttribute))]
public class MinMaxDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        MinMaxAttribute range = (MinMaxAttribute)attribute;

        SerializedProperty minProp = property.FindPropertyRelative("min");
        SerializedProperty maxProp = property.FindPropertyRelative("max");

        float minVal = minProp.floatValue;
        float maxVal = maxProp.floatValue;

        EditorGUI.BeginProperty(position, label, property);

        // Label
        position = EditorGUI.PrefixLabel(position, label);

        float fieldWidth = 50f;
        float spacing = 5f;

        Rect minRect = new Rect(position.x, position.y, fieldWidth, position.height);
        Rect maxRect = new Rect(position.x + position.width - fieldWidth, position.y, fieldWidth, position.height);
        Rect sliderRect = new Rect(
            position.x + fieldWidth + spacing,
            position.y,
            position.width - (fieldWidth * 2) - (spacing * 2),
            position.height
        );

        // Fields
        minVal = EditorGUI.FloatField(minRect, minVal);
        maxVal = EditorGUI.FloatField(maxRect, maxVal);

        // Slider
        EditorGUI.MinMaxSlider(sliderRect, ref minVal, ref maxVal, range.minLimit, range.maxLimit);

        // Clamp values
        minVal = Mathf.Clamp(minVal, range.minLimit, range.maxLimit);
        maxVal = Mathf.Clamp(maxVal, range.minLimit, range.maxLimit);

        if (minVal > maxVal)
            minVal = maxVal;

        minProp.floatValue = minVal;
        maxProp.floatValue = maxVal;

        EditorGUI.EndProperty();
    }
}
#endif