using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SimpleBarRow : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI valueText;
    public Slider slider;

    /// <summary>
    /// Sets the bar and text based on ideal vs player values.
    /// Slider always represents percentage match.
    /// </summary>
    public void SetValues(float ideal, float player, string unit = "")
    {
        if (ideal <= 0f)
        {
            slider.value = 0f;
            valueText.text = $"Ideal: 0{unit} | You: {player:F1}{unit}";
            return;
        }

        float percent = Mathf.Clamp01(player / ideal) * 100f;

        slider.minValue = 0f;
        slider.maxValue = 100f;
        slider.value = percent;

        valueText.text = $"Ideal: {ideal}{unit} | You: {player:F1}{unit}";
    }
}
