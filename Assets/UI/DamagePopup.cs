using System.Collections;
using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    private float moveSpeed = 0.5f;
    private float fadeDuration = 1f;
    private TextMeshProUGUI textMesh;
    private Color originalColor;
  
    private Color defaultOutlineColor = Color.black;

    public void InitializePopup(int damage, bool isCritical, bool isPlayerHurt, Color? customOutlineColor = null)
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        if (textMesh == null)
        {
            Debug.LogError("DamagePopup: No TextMeshProUGUI component found.");
            return;
        }

        if (isCritical) {
            textMesh.color = Color.yellow;
            textMesh.fontSize = textMesh.fontSize * 1.1f;
        }
        if (isPlayerHurt) {
            textMesh.color = new Color(255/255f, 110/255f,110/255f);
        }
        Debug.Log("custom outline color: " + (customOutlineColor.HasValue ? customOutlineColor.Value.ToString() : "None"));
        Color outlineColor;
        if (customOutlineColor.HasValue)
        {
            outlineColor = customOutlineColor.Value;
        } else
        {
            outlineColor = defaultOutlineColor;
        }
        
        originalColor = textMesh.color;
        StartCoroutine(FadeOutAndMoveUp());
        textMesh.text = $"{damage}";

        textMesh.fontMaterial.SetColor("_OutlineColor", outlineColor);
        textMesh.fontMaterial.SetFloat("_OutlineWidth", 0.2f);
    }

    private IEnumerator FadeOutAndMoveUp()
    {
        float elapsedTime = 0f;
        Vector2 startPosition = transform.position;

        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            textMesh.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            transform.position = startPosition + new Vector2(0, moveSpeed * (elapsedTime / fadeDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}