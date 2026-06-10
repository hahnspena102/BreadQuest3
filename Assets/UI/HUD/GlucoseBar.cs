using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GlucoseBar : MonoBehaviour
{
    private Slider slider;
    [SerializeField] private PlayerData playerData;
    private TextMeshProUGUI glucoseText;
    void Start()
    {
        slider = GetComponent<Slider>();     
        slider.maxValue = 1f;

        glucoseText = GetComponentInChildren<TextMeshProUGUI>();
    }

    void Update()
    {
        slider.value = (float)playerData.CurrentGlucose / playerData.MaxGlucose;
        glucoseText.text = $"{(int)playerData.CurrentGlucose}/{(int)playerData.MaxGlucose}";
    }
}
