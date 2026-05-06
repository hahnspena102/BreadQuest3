using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ToppingsCard : MonoBehaviour
{
    [SerializeField]private TextMeshProUGUI titleText;
    [SerializeField]private TextMeshProUGUI descriptionText;
    [SerializeField]private Image iconImage;
    [SerializeField]private ToppingData toppingData;

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
        if (panel != null)        {
            panel.HidePanel();
        }
    }
}
