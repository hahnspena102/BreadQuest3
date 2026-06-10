
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FloorUI : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;
    private TextMeshProUGUI floorText;
    [SerializeField] private Image[] possibleFlavorImages;

    void Start()
    {
        floorText = GetComponentInChildren<TextMeshProUGUI>();
        floorText.text = "Floor " + playerData.CurrentFloor.ToString("0");

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
        }


    }

    void Update()
    {
        
    }
}