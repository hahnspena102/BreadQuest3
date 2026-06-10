using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class Intro : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;

    [Header("Animation")]
    [SerializeField] private float letterDelay = 0.05f;

    [Header("Fade")]
    [SerializeField] private float fadeDuration = 1.4f;

    [Header("Flavor")]
    [SerializeField] private Image[] possibleFlavorImages;

    private TextMeshProUGUI floorText;
    private CanvasGroup canvasGroup;

    void Start()
    {
        floorText = GetComponentInChildren<TextMeshProUGUI>();
        canvasGroup = GetComponent<CanvasGroup>();

        string fullText = "Floor " + playerData.CurrentFloor.ToString("0");

        StartCoroutine(PlayIntro(fullText));

        InitiliazePossibleFlavorImages();
    }

    void InitiliazePossibleFlavorImages()
    {
        foreach (var img in possibleFlavorImages)
        {
            img.transform.parent.gameObject.SetActive(false);
        }

        WorldManager worldManager = FindFirstObjectByType<WorldManager>();
        if (worldManager == null) return;

        for (int i = 0; i < worldManager.CurrentFlavors.Length && i < possibleFlavorImages.Length; i++)
        {
            Image img = possibleFlavorImages[i];

            img.sprite = worldManager.CurrentFlavors[i].FlavorSprite;
            img.transform.parent.gameObject.SetActive(true);

            CanvasGroup cg = img.GetComponent<CanvasGroup>();
            if (cg == null)
                cg = img.gameObject.AddComponent<CanvasGroup>();

            cg.alpha = 0f;
            img.rectTransform.localScale = Vector3.zero;
        }
    }

    IEnumerator PlayIntro(string fullText)
    {
        canvasGroup.alpha = 1f;

        floorText.text = fullText;
        floorText.maxVisibleCharacters = 0;

        floorText.ForceMeshUpdate();

        int totalChars = floorText.textInfo.characterCount;

        // Reveal floor text
        for (int i = 0; i <= totalChars; i++)
        {
            floorText.maxVisibleCharacters = i;
            yield return new WaitForSeconds(letterDelay);
        }

        // Reveal flavor images one at a time
        foreach (var img in possibleFlavorImages)
        {
            if (!img.gameObject.activeSelf)
                continue;

            yield return StartCoroutine(PopImage(img));
        }

        yield return new WaitForSeconds(1f);

        yield return FadeOut();
    }
    IEnumerator PopImage(Image img)
    {
        RectTransform rt = img.rectTransform;

        CanvasGroup cg = img.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = img.gameObject.AddComponent<CanvasGroup>();

        cg.alpha = 0f;
        rt.localScale = Vector3.zero;

        float duration = 0.2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / duration;

            // Overshoot then settle
            float scale = Mathf.LerpUnclamped(
                0f,
                1f,
                Mathf.Sin(t * Mathf.PI * 0.5f) * 1.15f
            );

            rt.localScale = Vector3.one * scale;
            cg.alpha = t;

            yield return null;
        }

        rt.localScale = Vector3.one;
        cg.alpha = 1f;
    }

    IEnumerator FadeOut()
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
    }
}