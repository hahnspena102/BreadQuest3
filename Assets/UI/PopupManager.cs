using System.Collections;
using UnityEngine;
using TMPro;

public class PopupManager : MonoBehaviour
{
    [SerializeField] private GameObject damagePopupPrefab;
    [SerializeField] private GameObject deathParticlePrefab;

    public void ShowDamagePopup(Vector2 position, int damage, bool isCritical, bool isPlayerHurt)
    {
        if (damagePopupPrefab == null)
        {
            Debug.LogError("PopupManager: Damage popup prefab is not assigned.");
            return;
        }
        GameObject popupInstance = Instantiate(damagePopupPrefab, position, Quaternion.identity, transform);
        DamagePopup damagePopup = popupInstance.GetComponent<DamagePopup>();
        damagePopup.InitializePopup(damage, isCritical, isPlayerHurt);
    }

    public void ShowDeathParticles(Vector2 position)
    {
        if (deathParticlePrefab == null)
        {
            Debug.LogError("PopupManager: Death particle prefab is not assigned.");
            return;
        }
        Instantiate(deathParticlePrefab, position, Quaternion.identity);
    }
}