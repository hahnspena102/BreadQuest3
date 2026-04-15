using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(DroppedItem))]
public class DroppedItemEditor : Editor
{
    private DroppedItem droppedItem;

    private void OnEnable()
    {
        droppedItem = (DroppedItem)target;
    }

    public override void OnInspectorGUI()
    {
        Item currentItem = droppedItem.Item;
        ItemData currentData = currentItem != null ? currentItem.ItemData : null;

        EditorGUI.BeginChangeCheck();
        ItemData newData = (ItemData)EditorGUILayout.ObjectField("Item Data", currentData, typeof(ItemData), false);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(droppedItem, "Change Dropped Item Data");
            AssignItemFromData(newData, currentItem);
            currentItem = droppedItem.Item;
        }

        if (currentItem != null)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("Type", currentItem.GetType().Name);

            EditorGUI.BeginChangeCheck();
            int newCount = EditorGUILayout.IntField("Count", currentItem.Count);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(droppedItem, "Edit Dropped Item Count");
                currentItem.Count = Mathf.Max(1, newCount);
                EditorUtility.SetDirty(droppedItem);
            }

            if (currentItem is Weapon weapon)
            {
                EditorGUI.BeginChangeCheck();
                int newLevel = EditorGUILayout.IntField("Weapon Level", weapon.Level);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(droppedItem, "Edit Dropped Weapon Level");
                    weapon.Level = Mathf.Max(1, newLevel);
                    EditorUtility.SetDirty(droppedItem);
                }
            }

            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space();
        DrawPropertiesExcluding(serializedObject, "m_Script", "item");
        serializedObject.ApplyModifiedProperties();
    }

    private void AssignItemFromData(ItemData newData, Item previousItem)
    {
        if (newData == null)
        {
            droppedItem.Item = null;
            ApplyItemVisuals();
            EditorUtility.SetDirty(droppedItem);
            return;
        }

        Item newItem = ItemFactory.CreateFromData(newData);
        if (newItem != null && previousItem != null)
        {
            newItem.Count = previousItem.Count;

            if (newItem is Weapon newWeapon && previousItem is Weapon previousWeapon)
            {
                newWeapon.Level = previousWeapon.Level;
            }
        }

        droppedItem.Item = newItem;
        ApplyItemVisuals();
        EditorUtility.SetDirty(droppedItem);
    }

    private void ApplyItemVisuals()
    {
        ItemData data = droppedItem.Item != null ? droppedItem.Item.ItemData : null;
        droppedItem.gameObject.name = data != null ? data.ItemName : "Item";

        SpriteRenderer sr = droppedItem.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sprite = data != null ? data.ItemSprite : null;
        }
    }
}
#endif