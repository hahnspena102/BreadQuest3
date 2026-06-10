using UnityEngine;

public class InfoUI : MonoBehaviour
{
    private Player player;
    [SerializeField] private GameObject[] minimalInfoUI;
    [SerializeField] private GameObject[] detailedInfoUI;
    [SerializeField] private GameObject[] flavorInfoUI;
    [ReadOnly] [SerializeField] private InfoMode currentInfoMode;

    void Start()
    {
        player = FindFirstObjectByType<Player>();
    }

    void Update()
    {
        currentInfoMode = player.InfoMode;
        switch (player.InfoMode)
        {
            case InfoMode.Minimal:
                foreach (GameObject uiElement in minimalInfoUI)
                {
                    uiElement.SetActive(true);
                }
                foreach (GameObject uiElement in detailedInfoUI)
                {
                    uiElement.SetActive(false);
                }
                foreach (GameObject uiElement in flavorInfoUI)
                {
                    uiElement.SetActive(false);
                }
                break;
            case InfoMode.Detailed:
                foreach (GameObject uiElement in minimalInfoUI)
                {
                    uiElement.SetActive(true);
                }
                foreach (GameObject uiElement in detailedInfoUI)
                {
                    uiElement.SetActive(true);
                }
                foreach (GameObject uiElement in flavorInfoUI)
                {
                    uiElement.SetActive(false);
                }
                break;
            case InfoMode.Flavor:
                foreach (GameObject uiElement in minimalInfoUI)
                {
                    uiElement.SetActive(true);
                }
                foreach (GameObject uiElement in detailedInfoUI)
                {
                    uiElement.SetActive(true);
                }
                foreach (GameObject uiElement in flavorInfoUI)
                {
                    uiElement.SetActive(true);
                }
                break;
        }
    }
}