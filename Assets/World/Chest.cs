using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

public class Chest : MonoBehaviour
{

    private Player player;
    private ItemManager itemManager;
    private bool playerInRange = false;
    private Canvas statusCanvas;

    void Start()
    {
        player = FindFirstObjectByType<Player>();
        itemManager = FindFirstObjectByType<ItemManager>();
        statusCanvas = GetComponentInChildren<Canvas>();
        statusCanvas.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerInRange) return;
        InputActionReference useAction = player.useAction;

        if (useAction != null && useAction.action.WasPressedThisFrame())
        {
            Debug.Log("Player used the chest!");
            StartCoroutine(OpenChest());
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        

        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player is in the chest range.");
            statusCanvas.enabled = true;
            Player player = other.gameObject.GetComponent<Player>();
            if (player == null) return;
            playerInRange = true;


        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player has left the chest range.");
            statusCanvas.enabled = false;
            playerInRange = false;
        }
    }

    IEnumerator OpenChest()
    {
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("open");
        }
        yield return new WaitForSeconds(1f); // Wait for the animation to finish

        float dropRoll = Random.value;
        int numDrops;
        if (dropRoll < 0.65f)
            numDrops = 1;
        else if (dropRoll < 0.95f)
            numDrops = 2;
        else
            numDrops = 3;
        for (int i = 0; i < numDrops; i++) {
            yield return new WaitForSeconds(0.5f); 
            itemManager.SpawnRandomDrop(transform.position);
        }
        
        Destroy(gameObject); // Remove the chest after opening
    }
}
