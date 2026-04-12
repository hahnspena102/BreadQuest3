using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Teleporter : MonoBehaviour
{

    private Player player;
    private bool playerInRange = false;
    private Canvas statusCanvas;

    void Start()
    {
        player = FindFirstObjectByType<Player>();
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
            Debug.Log("Player used the teleporter!");
            SceneManager.LoadScene("SampleScene");
            player.PlayerData.CurrentFloor += 1;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
     
        if (other.CompareTag("Player"))
        {
               Debug.Log("Player is in the teleporter area.");
            statusCanvas.enabled = true;
            Player player = other.GetComponent<Player>();
            if (player == null) return;
            playerInRange = true;


        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player has left the teleporter area.");
            statusCanvas.enabled = false;
            playerInRange = false;
        }
    }
}
