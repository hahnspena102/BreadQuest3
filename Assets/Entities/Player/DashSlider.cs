using UnityEngine;
using UnityEngine.UI;

public class DashSlider : MonoBehaviour
{
    private Canvas canvas;
    private Player player;
    private Slider slider;
    private RectTransform rectTransform;

    [SerializeField] private float leftOffset = 0.9f;
    [SerializeField] private float verticalOffset = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canvas = GetComponent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        player = FindFirstObjectByType<Player>();
        slider = GetComponentInChildren<Slider>();


        canvas.worldCamera = Camera.main;
        
    }

    // Update is called once per frame
    void Update()
    {
        slider.value = 1 - (player.DashCooldownTimer / player.DashCooldown);

        float facingSign = player.transform.localScale.x >= 0 ? 1f : -1f;
        rectTransform.localPosition = new Vector3(-leftOffset * facingSign, verticalOffset, 0f);
        rectTransform.localScale = Vector3.one;
        
        if (player.DashCooldownTimer > 0)
        {
            if (!canvas.enabled) canvas.enabled = true;
        }
        else
        {
            if (canvas.enabled) canvas.enabled = false;
        }
    }
}
