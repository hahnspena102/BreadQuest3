using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Teleporter : MonoBehaviour
{

    private Player player;
    private bool playerInRange = false;

    void Start()
    {
        player = FindFirstObjectByType<Player>();
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
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Player is in the teleporter area.");
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player == null) return;
            playerInRange = true;


        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("Player has left the teleporter area.");
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
