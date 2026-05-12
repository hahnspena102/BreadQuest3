using UnityEngine;
using System.Collections;

public class ToppingsPanel : MonoBehaviour
{
    [SerializeField] private ToppingsCard[] toppingCards;
    [SerializeField] private PlayerData playerData;

    [Header("Animation")]
    [SerializeField] private float fadeDuration = 0.25f;
    [SerializeField] private float cardDelay = 0.08f;
    [SerializeField] private float cardPopDuration = 0.2f;
    private Player player;

    private CanvasGroup canvasGroup;

    private void Start()
    {
        canvasGroup = GetComponentInParent<CanvasGroup>();
        player = FindFirstObjectByType<Player>();
        HideInstant();
    }

    public void DisplayToppings(ToppingData[] toppings)
    {
        StopAllCoroutines();
        player.IsMenuing = true;
        StartCoroutine(DisplayRoutine(toppings));
    }

    private IEnumerator DisplayRoutine(ToppingData[] toppings)
    {
        gameObject.SetActive(true);

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        // Setup cards first
        for (int i = 0; i < toppingCards.Length; i++)
        {
            if (i < toppings.Length)
            {
                toppingCards[i].InitializeCard(toppings[i]);
            }
            else
            {
                toppingCards[i].InitializeCard(null);
            }

            toppingCards[i].PrepareForAnimation();
        }

        // Fade in panel
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, time / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;

        // Animate cards one-by-one
        for (int i = 0; i < toppingCards.Length; i++)
        {
            StartCoroutine(toppingCards[i].AnimateIn(cardPopDuration));
            yield return new WaitForSeconds(cardDelay);
        }

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void HidePanel()
    {
        StopAllCoroutines();
        StartCoroutine(HideRoutine());

        player.IsMenuing = false;
    }

    private IEnumerator HideRoutine()
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        float startAlpha = canvasGroup.alpha;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, time / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
    }

    private void HideInstant()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}