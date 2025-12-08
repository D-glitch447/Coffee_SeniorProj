using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.ShaderGraph.Internal;

public class WaterFillController : MonoBehaviour
{
    [Header("UI")]
    public Image waterFillBar;
    public TextMeshProUGUI waterText;

    [Header("Water Settings")]
    public float currentWater = 0f;   // grams in kettle
    public float fillRate = 20f;      // grams per second
    public float maxVisual = 400f;    // how high the bar can appear visually

    [Header("Faucet")]
    public FaucetToggle faucet;       // reference to faucet on/off

    void Update()
    {
        if (faucet.IsOn)
        {
            currentWater += fillRate * Time.deltaTime;

            // Update bar (bar max is NOT recipe target)
            // float fillPercent = Mathf.Clamp01(currentWater / maxVisual);
            float fillPercent = Mathf.Clamp01((currentWater / maxVisual));
            waterFillBar.fillAmount = fillPercent;
        }

        // Update text
        waterText.text = $"{currentWater:F1} g";
    }

    // Called by the Check button ("Done Filling")
    public void ConfirmWaterAmount()
    {
        CoffeeRuntime.Instance.playerWaterWeight = currentWater;
        Debug.Log("Final Water = " + currentWater + "g");
    }
    public float GetFinalWater()
    {
        return currentWater;
    }
}

