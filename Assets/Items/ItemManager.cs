using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [SerializeField] private ItemDrops itemDrops;
    [SerializeField] private GameObject itemPrefab;

    public void SpawnItem(ItemData itemData, Vector3 position)
    {
        if (itemPrefab == null)
        {
            Debugger.LogError("Item prefab is not assigned in the ItemManager.", type: DebugType.Items);
            return;
        }

        if (itemData == null)
        {
            Debugger.LogError("Trying to spawn an item with null ItemData.", type: DebugType.Items);
            return;
        }

        GameObject itemObj = Instantiate(itemPrefab, position, Quaternion.identity);
        itemObj.transform.SetParent(transform); 
        Item itemComponent = itemObj.GetComponent<Item>();
        if (itemComponent != null)
        {
            itemComponent.ItemData = itemData;
        }
        else
        {
            Debugger.LogWarning("Spawned prefab does not have an Item component.", type: DebugType.Items);
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
            SpawnItem(randomItem, position);
        }
        else
        {
            Debugger.LogWarning("No item was returned from GetRandomDrop.", type: DebugType.Items);
        }
    }
}
