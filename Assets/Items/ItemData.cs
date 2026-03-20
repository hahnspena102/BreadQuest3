using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    [SerializeField] private string itemName;
    [SerializeField] private string description;
    [SerializeField] private Sprite itemSprite;
    [SerializeField] [Range(1, 5)] private int tier;
    public string ItemName { get => itemName; set => itemName = value; }
    public string Description { get => description; set => description = value; }
    public Sprite ItemSprite { get => itemSprite; set => itemSprite = value; }
    public global::System.Int32 Tier { get => tier; set => tier = value; }
}
