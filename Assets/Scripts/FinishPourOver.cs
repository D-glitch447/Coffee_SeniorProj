using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FinishPourOver : MonoBehaviour
{
    [Header("UI References")]
    public Button FinishButton;

    [Header("Scene Management")]
    [Tooltip("The name of the scene to go to next (e.g., 'Kitchen' or 'Serving').")]
    public string NextSceneName = "Kitchen"; 

    [Header("Dependencies")]
    public CoffeeBedManager bedManager;

    void Start()
    {
        if (bedManager == null) bedManager = Object.FindFirstObjectByType<CoffeeBedManager>();


        if (FinishButton != null)
        {
            FinishButton.onClick.AddListener(OnFinishClicked);
        }
        else
        {
            Debug.LogError("FinishPourOver: Please assign the UI Button in the Inspector!");
        }
    }

    public void OnFinishClicked()
    {
        // 1. Save Data to CoffeeRuntime
        if (CoffeeRuntime.Instance != null && bedManager != null)
        {
            // --- Save Raw Stats ---
            CoffeeRuntime.Instance.playerWaterWeight = bedManager.CurrentWaterInGrams;
            CoffeeRuntime.Instance.playerBrewTime = bedManager.brewTimer;
            CoffeeRuntime.Instance.playerBloomWaterUsed = bedManager.BloomTargetWeight;
            CoffeeRuntime.Instance.playerBloomDuration = bedManager.BloomDuration;

            // --- Save Calculated Component Scores (Crucial for Grading Scene) ---
            // We force a calculation just in case the state machine didn't reach 'Finished' yet
            bedManager.CalculateFinalScore(); 

            CoffeeRuntime.Instance.scoreWeight = bedManager.LastWeightScore;
            CoffeeRuntime.Instance.scoreSaturation = bedManager.LastSaturationScore;
            CoffeeRuntime.Instance.scoreTechnique = bedManager.LastTechniqueScore;
            CoffeeRuntime.Instance.scoreTime = bedManager.LastTimeScore;
            CoffeeRuntime.Instance.scoreImpatiencePenalty = bedManager.LastImpatiencePenalty;

            // Mark step as complete
            CoffeeRuntime.Instance.hasCompletedBrewing = true;
            CoffeeRuntime.Instance.kitchenState = KitchenState.AfterBrewing;

            Debug.Log($"Brew Finished & Saved! Final Score stored: {bedManager.FinalScore}");
        }
        else
        {
            Debug.LogWarning("CoffeeRuntime or BedManager not found! Stats were not saved.");
        }

        // 2. Load the next scene
        SceneManager.LoadScene(NextSceneName);
    }
}