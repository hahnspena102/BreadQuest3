using UnityEngine;
using TMPro;

public class DroppedItem : MonoBehaviour
{
    [SerializeField] private Item item;
    [SerializeField] private bool playSpawnPop = true;
    [SerializeField] private float popHeight = 0.85f;
    [SerializeField] private float popDuration = 0.32f;
    [SerializeField] private float popHorizontalSpread = 0.35f;
    [SerializeField] private float popVerticalSpread = 0.15f;
    [SerializeField] private float popScalePunch = 0.12f;
    
    private TextMeshProUGUI equipText;

    public Item Item
    {
        get => item;
        set
        {
            item = value;
            RefreshVisuals();
        }
    }
    private SpriteRenderer spriteRenderer;
    private Coroutine spawnPopRoutine;

    void OnEnable()
    {
        if (!playSpawnPop) return;

        if (spawnPopRoutine != null)
        {
            StopCoroutine(spawnPopRoutine);
        }

        spawnPopRoutine = StartCoroutine(PlaySpawnPop());
    }

    void Start()
    {
        equipText = GetComponentInChildren<TextMeshProUGUI>();
        RefreshVisuals();
        
        
    }

    private System.Collections.IEnumerator PlaySpawnPop()
    {
        float duration = Mathf.Max(0.01f, popDuration);
        Vector3 startPosition = transform.position;
        Vector3 startScale = transform.localScale;

        float horizontalOffset = Random.Range(-popHorizontalSpread, popHorizontalSpread);
        float verticalOffset = Random.Range(-popVerticalSpread, popVerticalSpread);
        Vector3 endPosition = startPosition + new Vector3(horizontalOffset, verticalOffset, 0f);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            float arc = Mathf.Sin(t * Mathf.PI) * popHeight;
            transform.position = Vector3.Lerp(startPosition, endPosition, t) + Vector3.up * arc;

            float punch = 1f + Mathf.Sin(t * Mathf.PI) * popScalePunch;
            transform.localScale = startScale * punch;

            yield return null;
        }

        transform.position = endPosition;
        transform.localScale = startScale;
        spawnPopRoutine = null;
    }

    public void RefreshVisuals()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        ItemData itemData = item != null ? item.ItemData : null;

        gameObject.name = itemData != null ? itemData.ItemName : "Item";
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = itemData != null ? itemData.ItemSprite : null;
        }

        if (equipText != null)
        {
            equipText.text = $"[E] {item?.ItemData.ItemName ?? "item"}";
            equipText.enabled = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Player"))
        {
            
            equipText.enabled = true;

        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            equipText.enabled = false;
        }
    }
}
