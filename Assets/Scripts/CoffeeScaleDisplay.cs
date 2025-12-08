using UnityEngine;
using TMPro; // Make sure to include this for TextMeshPro!

public class CoffeeScaleDisplay : MonoBehaviour
{
    [Header("References")]
    public CoffeeBedManager coffeeManager; // Drag your Coffee object here
    public TMP_Text textDisplay;       // Drag your Text object here

    [Header("Settings")]
    public float GroundsWeight = 18.0f;    // e.g., The 18g of beans
    public bool TareScale = true;          // If true, starts at 0.0g. If false, starts at 18.0g

    [Header("Visuals")]
    public string Format = "{0:F1}g";      // Shows "25.4g" (F1 = 1 decimal place)
    public Color NormalColor = Color.black;
    public Color TargetColor = Color.green;
    public float TargetWeight = 250f;      // When do we turn green?

    void Update()
    {
        if (coffeeManager == null || textDisplay == null) return;

        // 1. Calculate the number to show
        float waterWeight = coffeeManager.CurrentWaterInGrams;
        float displayWeight = waterWeight;

        if (!TareScale)
        {
            displayWeight += GroundsWeight;
        }

        // 2. Update the Text
        textDisplay.text = string.Format(Format, displayWeight);

        // 3. Optional: Change color if we are close to the target recipe
        if (displayWeight >= TargetWeight - 2f && displayWeight <= TargetWeight + 2f)
        {
            textDisplay.color = TargetColor;
        }
        else
        {
            textDisplay.color = NormalColor;
        }
    }
    
    // Helper function if you want to add a "Tare Button" to your UI later
    public void ToggleTare()
    {
        TareScale = !TareScale;
    }
}