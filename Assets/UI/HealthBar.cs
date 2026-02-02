using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    private Slider slider;
    [SerializeField] private PlayerData playerData;
    private TextMeshProUGUI healthText;
    void Start()
    {
        slider = GetComponent<Slider>();     
        slider.maxValue = 1f;

        healthText = GetComponentInChildren<TextMeshProUGUI>();
    }

    void Update()
    {
        slider.value = (float)playerData.CurrentHealth / playerData.MaxHealth;
        healthText.text = $"{(int)playerData.CurrentHealth}/{(int)playerData.MaxHealth}";
    }
}
