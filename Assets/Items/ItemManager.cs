using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [SerializeField] private ItemDrops itemDrops;
    [SerializeField] private GameObject itemPrefab;

    public void SpawnItem(Item item, Vector3 position)
    {
        if (item == null)
        {
            Debugger.LogError("Trying to spawn a null item.", type: DebugType.Items);
            return;
        }

        ItemData itemData = item != null ? item.ItemData : null;
        if (itemPrefab == null)
        {
            Debugger.LogError("Item prefab is not assigned in the ItemManager.", type: DebugType.Items);
            return;
        }

        GameObject itemObj = Instantiate(itemPrefab, position, Quaternion.identity);
        itemObj.transform.SetParent(transform); 
        DroppedItem droppedItemComponent = itemObj.GetComponent<DroppedItem>();
        if (droppedItemComponent != null)
        {
            droppedItemComponent.Item = ItemFactory.Clone(item);
        }
        else
        {
            Debugger.LogWarning("Spawned prefab does not have a DroppedItem component.", type: DebugType.Items);
        }
    }

    public void SpawnRandomDrop(Vector3 position)
    {
        if (itemDrops == null)
        {
            Debugger.LogError("ItemDrops is not assigned in the ItemManager.", type: DebugType.Items);
            return;
        }

        ItemData randomItem = itemDrops.GetRandomDrop();
        if (randomItem != null)
        {
            Item item = ItemFactory.CreateFromData(randomItem);
            SpawnItem(item, position);
        }
        else
        {
            Debugger.LogWarning("No item was returned from GetRandomDrop.", type: DebugType.Items);
        }
    }
}
