using UnityEngine;

public class Melee : MonoBehaviour
{
    [SerializeField] private MeleeData meleeData;
    private Player player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        player = GetComponentInParent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && player.Inventory.EquippedItemData == meleeData)
        {    
            

            Vector2 direction = player.WorldPointPosition - (Vector2)transform.position;


            player.MeleeAttack(direction.normalized);
        }
    }
}
