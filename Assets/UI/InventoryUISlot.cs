using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventoryUISlot : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [Header("Components")]
    [SerializeField] private Image itemSprite;
    [SerializeField] private Image highlightBorder;
    [SerializeField] private TextMeshProUGUI quantityText;

    [ReadOnly] [SerializeField] private Item item;
    [ReadOnly] [SerializeField] private int slotIndex;

    private Player player;
    private UseItem useItem;

    public int SlotIndex
    {
        get => slotIndex;
        set => slotIndex = value;
    }

    // DRAG GHOST
    private static InventoryUISlot draggedSlot;
    private static GameObject dragIcon;
    private static RectTransform dragRect;
    private static Canvas canvas;
    private static Image dragImage;

    void Start()
    {
        player = FindFirstObjectByType<Player>();
        useItem = FindFirstObjectByType<UseItem>();

        canvas = GetComponentInParent<Canvas>();
    }

    void Update()
    {
        if (player == null || player.Inventory == null) return;

        HighlightSlot(player.Inventory.CurrentItemIndex == slotIndex);
    }

    // ======================
    // SET ITEM
    // ======================
    public void SetItem(Item newItem)
    {
        item = newItem;

        if (item == null || item.ItemData == null)
        {
            itemSprite.sprite = null;
            itemSprite.enabled = false;
            quantityText.text = "";
            return;
        }

        itemSprite.sprite = item.ItemData.ItemSprite;
        itemSprite.enabled = true;

        quantityText.text = item.Count > 1 ? item.Count.ToString() : "";
    }

    // ======================
    // HIGHLIGHT
    // ======================
    public void HighlightSlot(bool highlight)
    {
        if (highlightBorder != null)
            highlightBorder.enabled = highlight;

        itemSprite.rectTransform.localScale =
            highlight ? Vector3.one * 1.4f : Vector3.one;
    }

    // ======================
    // CLICK SELECT
    // ======================
    public void SelectItem()
    {
        player.Inventory.CycleTo(slotIndex);
    }

    // ======================
    // DRAG START
    // ======================
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item == null || item.ItemData == null) return;

        draggedSlot = this;

        // create ghost icon
        dragIcon = new GameObject("DragIcon");
        dragIcon.transform.SetParent(canvas.transform, false);

        dragImage = dragIcon.AddComponent<Image>();
        dragImage.sprite = item.ItemData.ItemSprite;
        dragImage.raycastTarget = false;

        dragRect = dragIcon.GetComponent<RectTransform>();
        dragRect.sizeDelta = itemSprite.rectTransform.sizeDelta;

        SetDragPosition(eventData.position);
    }

    // ======================
    // DRAG MOVE
    // ======================
    public void OnDrag(PointerEventData eventData)
    {
        if (dragIcon == null) return;

        SetDragPosition(eventData.position);
    }

    private void SetDragPosition(Vector2 pos)
    {
        dragRect.position = pos;
    }

    // ======================
    // DRAG END
    // ======================
    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragIcon != null)
            Destroy(dragIcon);

        dragIcon = null;
        draggedSlot = null;
    }

    // ======================
    // DROP
    // ======================
    public void OnDrop(PointerEventData eventData)
    {
        if (draggedSlot == null || draggedSlot == this) return;

        SwapItems(draggedSlot, this);
    }

    // ======================
    // SWAP
    // ======================
    private void SwapItems(InventoryUISlot a, InventoryUISlot b)
    {
        var inv = player.Inventory.Items;

        int ai = a.SlotIndex;
        int bi = b.SlotIndex;

        Item temp = inv[ai];
        inv[ai] = inv[bi];
        inv[bi] = temp;

        a.SetItem(inv[ai]);
        b.SetItem(inv[bi]);
    }
}