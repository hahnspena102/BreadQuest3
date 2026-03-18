using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [SerializeField] private GameObject itemPrefab;

    public void SpawnItem(ItemData itemData, Vector3 position)
    {
        if (itemPrefab == null)
        {
            Debug.LogError("Item prefab is not assigned in the ItemManager.");
            return;
        }

        if (itemData == null)
        {
            Debug.LogError("Trying to spawn an item with null ItemData.");
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
            Debug.LogWarning("Spawned prefab does not have an Item component.");
        }
    }
}
