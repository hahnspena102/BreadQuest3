
using UnityEngine;
using TMPro;
public class FloorUI : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;
    private TextMeshProUGUI floorText;

    void Start()
    {
        floorText = GetComponentInChildren<TextMeshProUGUI>();
        floorText.text = "Floor " + playerData.CurrentFloor.ToString("0");
    }

    void Update()
    {
        
    }
}