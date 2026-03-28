using UnityEngine;

[System.Serializable]
public class Item
{
    [SerializeField] private ItemData itemData;
    [SerializeField] private int count = 1;

    public ItemData ItemData { get => itemData; set => itemData = value; }
    public int Count { get => count; set => count = value; }
}