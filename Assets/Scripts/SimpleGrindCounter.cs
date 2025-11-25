// Make sure to add this at the top!
using UnityEngine;
using TMPro; // Required for TextMeshPro

public class SimpleGrindCounter : MonoBehaviour
{
    [Header("UI Text")]
    [SerializeField] private TextMeshProUGUI grindText; // The text to update

    [Header("Grind Settings")]
    [SerializeField] private float currentGrindAmount = 0f;
    [SerializeField] private float grindSpeed = 7.5f; // Grams per second

    // You can delete this 'Start' function if you drag the text
    // in the inspector. This just finds it automatically.
    

    // Update is called once per frame
   public void UpdateGrindAmount(float newGrindAmount)
{
    // Update the internal value
    // Note: You might want to pass the total rotation and convert it here
    // If you pass the final amount, this is simple:
    currentGrindAmount = newGrindAmount;

    // Update the UI text
    grindText.text = $"{currentGrindAmount:F1}g";
}

    // A simple function to update the text display
    private void UpdateText()
    {
        // "F1" formats the number to 1 decimal place (e.g., "18.5")
        grindText.text = $"{currentGrindAmount:F1}g";
    }

    // --- Optional: Call this to reset the counter ---
    public void ResetGrind()
    {
        currentGrindAmount = 0f;
        UpdateText();
    }
}