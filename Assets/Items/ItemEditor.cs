using UnityEngine;
using UnityEditor;
using UnityEngine.AI;

[CustomEditor(typeof(Item))]
public class ItemEditor : Editor
{
    SerializedProperty itemDataProp;

    private Item item;

    private void OnEnable()
    {
        item = (Item)target;
        itemDataProp = serializedObject.FindProperty("itemData");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        EditorGUILayout.PropertyField(itemDataProp);

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            ApplyItemData();
        }

        DrawPropertiesExcluding(serializedObject, "itemData");

        serializedObject.ApplyModifiedProperties();
    }

    void ApplyItemData()
    {
        if (item.ItemData == null)
            return;

        ItemData data = item.ItemData;


        GameObject itemObj = item.gameObject;
        itemObj.name = data.ItemName;

        SpriteRenderer sr = item.GetComponent<SpriteRenderer>();
        if (sr != null && data.ItemSprite)
        {
            sr.sprite = data.ItemSprite;
        }

        EditorUtility.SetDirty(item);
    }

    
}