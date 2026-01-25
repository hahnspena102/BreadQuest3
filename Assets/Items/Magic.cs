using UnityEngine;

public class Magic: MonoBehaviour
{
    [SerializeField] private MagicData magicData;
    private Player player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        player = GetComponentInParent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && player.Inventory.EquippedItemData == magicData)
        {    
            

            Vector2 direction = player.WorldPointPosition - (Vector2)transform.position;

            Debug.Log("Attacking with " + magicData.name + " in direction: " + direction.normalized);


            player.Attack(direction.normalized, "magic");
        }
    }
}
