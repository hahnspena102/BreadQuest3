using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelBar : MonoBehaviour
{
    private Slider slider;
    [SerializeField] private PlayerData playerData;
    private TextMeshProUGUI levelText;
    void Start()
    {
        slider = GetComponent<Slider>();     
        slider.maxValue = 1f;

        levelText = GetComponentInChildren<TextMeshProUGUI>();
    }

    void Update()
    {
        float currentThreshold = playerData.GetNextThreshold();
            float previousThreshold = playerData.GetNextThreshold(playerData.Level - 1);
        slider.value = (playerData.Experience - previousThreshold) / (currentThreshold - previousThreshold);
        levelText.text = $"{(int)playerData.Level}";
    }
}
