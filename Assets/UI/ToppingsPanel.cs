using UnityEngine;

public class ToppingsPanel : MonoBehaviour
{
    [SerializeField] private ToppingsCard[] toppingCards;
    [SerializeField] private PlayerData playerData;
    private CanvasGroup canvasGroup;
    void Start()
    {
        canvasGroup = GetComponentInParent<CanvasGroup>();
        HidePanel();
    }

    public void DisplayToppings(ToppingData[] toppings)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
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
        }
    }

    public void HidePanel()
    {
       canvasGroup.alpha = 0f;
       canvasGroup.interactable = false;
    }
}
