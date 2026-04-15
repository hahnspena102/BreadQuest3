using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(Inventory))]
public class InventoryEditor : Editor
{
    private SerializedProperty capacityProp;
    private SerializedProperty itemsProp;
    private SerializedProperty currentItemIndexProp;
    private SerializedProperty equippedItemProp;

    private Inventory inventory;

    private void OnEnable()
    {
        inventory = (Inventory)target;
        capacityProp = serializedObject.FindProperty("capacity");
        itemsProp = serializedObject.FindProperty("items");
        currentItemIndexProp = serializedObject.FindProperty("currentItemIndex");
        equippedItemProp = serializedObject.FindProperty("equippedItem");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(capacityProp);
        if (EditorGUI.EndChangeCheck())
        {
            EnsureSlotsMatchCapacity();
        }

        EnsureSlotsMatchCapacity();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Inventory Slots", EditorStyles.boldLabel);

        for (int i = 0; i < itemsProp.arraySize; i++)
        {
            DrawSlotField(i);
        }

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(currentItemIndexProp);
        EditorGUILayout.PropertyField(equippedItemProp);

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawSlotField(int index)
    {
        Item currentItem = inventory.GetItemAtIndex(index);
        ItemData currentData = currentItem != null ? currentItem.ItemData : null;

        EditorGUI.BeginChangeCheck();
        ItemData newData = (ItemData)EditorGUILayout.ObjectField($"Slot {index}", currentData, typeof(ItemData), false);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(inventory, $"Change Inventory Slot {index}");

            if (newData == null)
            {
                inventory.SetItemAtIndex(index, null);
                if (inventory.CurrentItemIndex == index)
                {
                    inventory.EquippedItem = null;
                }
            }
            else
            {
                Item newItem = ItemFactory.CreateFromData(newData);
                if (newItem != null && currentItem != null)
                {
                    newItem.Count = currentItem.Count;

                    if (newItem is Weapon newWeapon && currentItem is Weapon currentWeapon)
                    {
                        newWeapon.Level = currentWeapon.Level;
                    }
                }

                inventory.SetItemAtIndex(index, newItem);
                if (inventory.CurrentItemIndex == index)
                {
                    inventory.EquippedItem = newItem;
                }
            }

            EditorUtility.SetDirty(inventory);
            serializedObject.Update();

            currentItem = inventory.GetItemAtIndex(index);
        }

        if (currentItem != null)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("Type", currentItem.GetType().Name);

            EditorGUI.BeginChangeCheck();
            int newCount = EditorGUILayout.IntField("Count", currentItem.Count);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(inventory, $"Edit Inventory Slot {index} Count");
                currentItem.Count = Mathf.Max(1, newCount);
                if (inventory.CurrentItemIndex == index)
                {
                    inventory.EquippedItem = currentItem;
                }
                EditorUtility.SetDirty(inventory);
            }

            if (currentItem is Weapon weapon)
            {
                EditorGUI.BeginChangeCheck();
                int newLevel = EditorGUILayout.IntField("Weapon Level", weapon.Level);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(inventory, $"Edit Inventory Slot {index} Weapon Level");
                    weapon.Level = Mathf.Max(1, newLevel);
                    if (inventory.CurrentItemIndex == index)
                    {
                        inventory.EquippedItem = weapon;
                    }
                    EditorUtility.SetDirty(inventory);
                }
            }

            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space();
    }

    private void EnsureSlotsMatchCapacity()
    {
        int targetCapacity = Mathf.Max(0, capacityProp.intValue);
        if (itemsProp.arraySize != targetCapacity)
        {
            itemsProp.arraySize = targetCapacity;
        }

        if (targetCapacity == 0)
        {
            inventory.CurrentItemIndex = 0;
            inventory.EquippedItem = null;
            return;
        }

        if (inventory.CurrentItemIndex >= targetCapacity)
        {
            inventory.CurrentItemIndex = targetCapacity - 1;
        }
    }
}
#endif
