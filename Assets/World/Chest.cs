using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Chest : MonoBehaviour
{

    private Player player;
    private ItemManager itemManager;
    private bool playerInRange = false;

    void Start()
    {
        player = FindFirstObjectByType<Player>();
        itemManager = FindFirstObjectByType<ItemManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerInRange) return;
        InputActionReference useAction = player.useAction;

        if (useAction != null && useAction.action.WasPressedThisFrame())
        {
            Debug.Log("Player used the chest!");
            itemManager.SpawnRandomDrop(transform.position);
            // Optionally, you could add an animation or sound effect here
             Destroy(gameObject); // Remove the chest after opening
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("Player is in the chest range.");
        if (other.gameObject.CompareTag("Player"))
        {
            Player player = other.gameObject.GetComponent<Player>();
            if (player == null) return;
            playerInRange = true;


        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        Debug.Log("Player has left the chest range.");
        if (other.gameObject.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
