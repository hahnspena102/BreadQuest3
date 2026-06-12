using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

public class Teleporter : MonoBehaviour
{

    private Player player;
    private bool playerInRange = false;
    private Canvas statusCanvas;
    [SerializeField] private AudioClip warpSound;

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
            StartCoroutine(WarpPlayer());
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
     
        if (other.CompareTag("Player") && !player.IsWarping)
        {
            statusCanvas.enabled = true;
            Player player = other.GetComponent<Player>();
            if (player == null) return;
            playerInRange = true;


        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        
        if (other.CompareTag("Player") || player.IsWarping)
        {

            statusCanvas.enabled = false;
            playerInRange = false;
        }
    }

    IEnumerator WarpPlayer()
    {
        player.IsWarping = true;
        Animator animator = player.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("warp");
        }
        SoundManager.instance.PlaySoundFXClip(player.PlayerData.GetWarpSound(), player.transform, 0.2f, 0.1f);
        SoundManager.instance.PlaySoundFXClip(warpSound, player.transform, 0.8f, 0.1f);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("MainScene");
        player.IsWarping = false;
        player.PlayerData.CurrentFloor += 1;
    }
}
