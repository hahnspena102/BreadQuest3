using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlotUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Image itemSprite;
    [SerializeField] private Image highlightBorder;
    [SerializeField] private Item item;
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private Slider cooldownSlider;
    [SerializeField] private TextMeshProUGUI cooldownText;

    private Player player;
    private UseItem useItem;
    private const float fallbackHealCooldownDuration = 30f;
    private const float fallbackGlucoseCooldownDuration = 10f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindFirstObjectByType<Player>();
        useItem = FindFirstObjectByType<UseItem>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetItem(Item newItem)
    {
        item = newItem;

        if (item == null)
        {
            itemSprite.sprite = null;
            itemSprite.enabled = false;
            quantityText.text = "";
             HideCooldownUI();
            return;
        } else if (item.ItemData == null)
        {
            itemSprite.sprite = null;
            itemSprite.enabled = false;
            quantityText.text = "";
             HideCooldownUI();
            return;
        }
        
        if (itemSprite != null)
        {
            itemSprite.sprite = item.ItemData.ItemSprite;
            itemSprite.enabled = true;
        }
        quantityText.text = item.Count > 1 ? item.Count.ToString() : "";

       UpdateCooldown();
    

    }

    public void HighlightSlot(bool highlight)
    {
        if (highlightBorder != null)
        {
            highlightBorder.enabled = highlight;
        }
        itemSprite.rectTransform.localScale = highlight ? Vector3.one * 1.4f : Vector3.one;
    }

    public void UpdateCooldown()
    {
        if (item == null || item.ItemData == null)
        {
            HideCooldownUI();
            return;
        }

        if (item is Potion potionItem)
        {
            if (player == null)
            {
                player = FindFirstObjectByType<Player>();
            }

            PotionData potionData = potionItem.PotionData;
            if (player == null || player.PlayerData == null || potionData == null || potionData.potionEffects == null)
            {
                HideCooldownUI();
                return;
            }

            float remainingCooldown = 0f;
            float cooldownDuration = 0f;

            for (int i = 0; i < potionData.potionEffects.Length; i++)
            {
                PotionEffect effect = potionData.potionEffects[i].effect;
                if (effect == PotionEffect.Heal)
                {
                    float healRemaining = player.PlayerData.HealCooldown;
                    if (healRemaining > remainingCooldown)
                    {
                        remainingCooldown = healRemaining;
                        cooldownDuration = GetHealCooldownDuration();
                    }
                }
                if (effect == PotionEffect.Glucose)
                {
                    float glucoseRemaining = player.PlayerData.GlucoseCooldown;
                    if (glucoseRemaining > remainingCooldown)
                    {
                        remainingCooldown = glucoseRemaining;
                        cooldownDuration = GetGlucoseCooldownDuration();
                    }
                }
            }

            if (remainingCooldown <= 0f)
            {
                HideCooldownUI();
                return;
            }

            if (cooldownSlider != null)
            {
                cooldownSlider.gameObject.SetActive(true);
                float maxCooldown = Mathf.Max(0.01f, cooldownDuration);
                float cooldownProgress = Mathf.Clamp01(remainingCooldown / maxCooldown);
                cooldownSlider.minValue = 0f;
                cooldownSlider.maxValue = 1f;
                cooldownSlider.value = cooldownProgress;
                Debug.Log($"Updating cooldown slider: remainingCooldown={remainingCooldown}, cooldownDuration={cooldownDuration}, cooldownProgress={cooldownProgress}");
                Debug.Log($"slider min={cooldownSlider.minValue}, max={cooldownSlider.maxValue}, value={cooldownSlider.value}");
            }

            if (cooldownText != null)
            {
                cooldownText.gameObject.SetActive(true);
                cooldownText.text = Mathf.Ceil(remainingCooldown).ToString();
            }
        }
        else if (item is Active activeItem)
        {
            float remainingCooldown = activeItem.Cooldown;
            if (remainingCooldown <= 0f)
            {
                HideCooldownUI();
                return;
            }

            if (cooldownSlider != null)
            {
                cooldownSlider.gameObject.SetActive(true);
                float maxCooldown = Mathf.Max(0.01f, activeItem.CooldownDuration);
                float cooldownProgress = Mathf.Clamp01(remainingCooldown / maxCooldown);
                cooldownSlider.minValue = 0f;
                cooldownSlider.maxValue = 1f;
                cooldownSlider.value = cooldownProgress;
            }

            if (cooldownText != null)
            {
                cooldownText.gameObject.SetActive(true);
                cooldownText.text = Mathf.Ceil(remainingCooldown).ToString();
            }
        }
        else
        {
            HideCooldownUI();
        }
    }

    private float GetHealCooldownDuration()
    {
        if (useItem == null)
        {
            useItem = FindFirstObjectByType<UseItem>();
        }

        return useItem != null ? useItem.HealCooldownDuration : fallbackHealCooldownDuration;
    }

    private float GetGlucoseCooldownDuration()
    {
        if (useItem == null)
        {
            useItem = FindFirstObjectByType<UseItem>();
        }

        return useItem != null ? useItem.GlucoseCooldownDuration : fallbackGlucoseCooldownDuration;
    }

    private void HideCooldownUI()
    {
        if (cooldownSlider != null)
        {
            cooldownSlider.gameObject.SetActive(false);
        }

        if (cooldownText != null)
        {
            cooldownText.text = "";
            cooldownText.gameObject.SetActive(false);
        }
    }
}
