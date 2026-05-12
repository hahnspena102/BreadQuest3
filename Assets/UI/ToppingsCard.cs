using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ToppingsCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image iconImage;
    [SerializeField] private ToppingData toppingData;

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        rectTransform = GetComponent<RectTransform>();
    }

    public void InitializeCard(ToppingData data)
    {
        toppingData = data;

        if (toppingData != null)
        {
            titleText.text = toppingData.ToppingName;
            descriptionText.text = toppingData.Description;
            iconImage.sprite = toppingData.Icon;
            iconImage.enabled = true;
        }
        else
        {
            titleText.text = "";
            descriptionText.text = "";
            iconImage.sprite = null;
            iconImage.enabled = false;
        }
    }

    public void PrepareForAnimation()
    {
        canvasGroup.alpha = 0f;
        rectTransform.localScale = Vector3.one * 0.75f;
    }

    public IEnumerator AnimateIn(float duration)
    {
        float time = 0f;

        Vector3 startScale = Vector3.one * 0.75f;
        Vector3 endScale = Vector3.one;

        while (time < duration)
        {
            time += Time.deltaTime;

            float t = time / duration;

            // Smooth pop easing
            float eased = 1f - Mathf.Pow(1f - t, 3f);

            canvasGroup.alpha = Mathf.Lerp(0f, 1f, eased);
            rectTransform.localScale = Vector3.Lerp(startScale, endScale, eased);

            yield return null;
        }

        canvasGroup.alpha = 1f;
        rectTransform.localScale = Vector3.one;
    }

    public void ChooseCard()
    {
        if (toppingData != null)
        {
            Player player = FindFirstObjectByType<Player>();

            if (player != null)
            {
                player.PlayerData.AddTopping(toppingData);
            }
        }

        ToppingsPanel panel = GetComponentInParent<ToppingsPanel>();

        if (panel != null)
        {
            panel.HidePanel();
        }
    }
}