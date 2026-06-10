using System.Collections;
using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    [Header("Motion")]
    public float moveUpSpeed = 1.2f;
    public float fadeDuration = 0.8f;

    [Header("Scale Punch")]
    public float popScale = 0.8f;
    public float popDuration = 0.2f;

    [Header("Random Size")]
    public float sizeVariance = 0.05f;

    [Header("Rotation")]
    public float maxRotation = 15f;
    public float rotationWobble = 5f;

    private TextMeshProUGUI textMesh;
    private Color originalColor;

    private Vector3 startPos;
    private Vector3 baseScale;

    private float baseZRot;
    private float sizeMultiplier;

    public void InitializePopup(int damage, bool isCritical, bool isPlayerHurt, Color? customOutlineColor = null)
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        if (!textMesh) return;

        startPos = transform.position;
        baseScale = transform.localScale;

        textMesh.text = damage.ToString();

        // COLOR
        if (isCritical)
            textMesh.color = new Color(1f, 0.85f, 0.1f);
        else if (isPlayerHurt)
            textMesh.color = new Color(1f, 0.3f, 0.3f);
        else
            textMesh.color = Color.white;

        originalColor = textMesh.color;

        // OUTLINE
        Color outline = customOutlineColor ?? Color.black;
        textMesh.fontMaterial.SetColor("_OutlineColor", outline);
        textMesh.fontMaterial.SetFloat("_OutlineWidth", 0.25f);

        // ---------- RANDOM SIZE ----------
        sizeMultiplier = 1f + Random.Range(-sizeVariance, sizeVariance);

        // crits feel bigger by default
        if (isCritical)
            sizeMultiplier *= 1.25f;

        transform.localScale = Vector3.zero;

        // ---------- ROTATION START ----------
        baseZRot = Random.Range(-maxRotation, maxRotation);
        transform.rotation = Quaternion.Euler(0, 0, baseZRot);

        StartCoroutine(PopupRoutine());
    }

    private IEnumerator PopupRoutine()
    {
        float t = 0f;

        while (t < fadeDuration)
        {
            float n = t / fadeDuration;

            // ---------- SCALE (pop + random size) ----------
            float popN = Mathf.Clamp01(t / popDuration);
            float popCurve = Mathf.Sin(popN * Mathf.PI * 0.5f);

            float scale = Mathf.Lerp(0f, popScale, popCurve);
            transform.localScale = baseScale * scale * sizeMultiplier;

            // ---------- MOVE UP ----------
            float moveEase = 1f - Mathf.Pow(1f - n, 3f);
            transform.position = startPos + new Vector3(0, moveUpSpeed * moveEase, 0);

            // ---------- ROTATION ----------
            float settleToCenter = Mathf.Lerp(baseZRot, 0f, n);
            float wobble = Mathf.Sin(t * 20f) * rotationWobble * (1f - n);
            transform.rotation = Quaternion.Euler(0, 0, settleToCenter + wobble);

            // ---------- FADE ----------
            float alpha = Mathf.Lerp(1f, 0f, Mathf.Pow(n, 1.5f));
            textMesh.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            t += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}