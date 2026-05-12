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
    [SerializeField] private AudioClip chestOpenSound;

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
            StartCoroutine(OpenChest());
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        

        if (other.gameObject.CompareTag("Player"))
        {
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
        SoundManager.instance.PlaySoundFXClip(chestOpenSound, transform);
        yield return new WaitForSeconds(1f); // Wait for the animation to finish

        ItemDropEntry[] drops = null;

        float dropRoll = Random.value;
        if (dropRoll < 0.6f)
            drops = itemManager.ItemDrops.GetRandomDrops(GameManager.FloorToTier(player.PlayerData.CurrentFloor), 1);
        else if (dropRoll < 0.9f)
            drops = itemManager.ItemDrops.GetRandomDrops(GameManager.FloorToTier(player.PlayerData.CurrentFloor), 2);
        else
            drops = itemManager.ItemDrops.GetRandomDrops(GameManager.FloorToTier(player.PlayerData.CurrentFloor), 3);
        for (int i = 0; i < drops.Length; i++) {
            yield return new WaitForSeconds(0.5f); 
            itemManager.SpawnRandomDrop(transform.position, drops[i]);
        }
        
        Destroy(gameObject); // Remove the chest after opening
    }
}
