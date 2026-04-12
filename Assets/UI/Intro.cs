using UnityEngine;
using TMPro;
using System.Collections;

public class Intro : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;
    private TextMeshProUGUI floorText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        floorText = GetComponentInChildren<TextMeshProUGUI>();
        floorText.text = "Floor " + playerData.CurrentFloor.ToString("0");
        StartCoroutine(FadeOut());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator FadeOut()
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        float duration = 1f;
        float elapsed = 0f;
        canvasGroup.alpha = 1f;
        yield return new WaitForSeconds(1f); 
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
    }
}
