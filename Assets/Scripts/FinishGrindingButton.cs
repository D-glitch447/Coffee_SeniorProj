using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class FinishGrindingButton : MonoBehaviour
{
    [Header("Dependencies")]
    public FadeController fade;
    public CoffeeGrinder coffeeGrinder;
    public GrinderAdjustment grinderAdjustment;

    [Header("Scene Settings")]
    public string nextSceneName = "Kitchen"; 

    public void OnFinishGrinding()
    {
        Debug.Log(">>> BUTTON CLICKED! Starting save process...");
        
        // 1. SAVE DATA FIRST (Don't wait for fade)
        SaveCoffeeData();

        // 2. Start the finish sequence (Fade -> Load)
        StartCoroutine(FinishSequence());
    }

    private void SaveCoffeeData()
    {
        if (CoffeeRuntime.Instance == null)
        {
            Debug.LogError("CRITICAL ERROR: CoffeeRuntime is missing! Can't save data.");
            return;
        }

        // --- Save Grind Amount ---
        if (coffeeGrinder != null)
        {
            float finalGrams = coffeeGrinder.GetCurrentGrams();
            CoffeeRuntime.Instance.playerGrindAmount = finalGrams;
            Debug.Log($"SUCCESS: Saved Grind Amount: {finalGrams}g");
        }
        else
        {
            Debug.LogError("ERROR: 'Coffee Grinder' slot is empty on the button!");
        }

        // --- Save Grind Size ---
        if (grinderAdjustment != null)
        {
            int sizeIndex = grinderAdjustment.GetCurrentGrindIndex();
            string sizeName = grinderAdjustment.GetCurrentGrindName();
            CoffeeRuntime.Instance.playerGrindSizeIndex = sizeIndex;
            CoffeeRuntime.Instance.playerGrindSizeName = sizeName;
            Debug.Log($"SUCCESS: Saved Grind Size: {sizeName}");
        }

        CoffeeRuntime.Instance.hasCompletedGrind = true;
    }

    private IEnumerator FinishSequence()
    {
        // 3. Attempt Fade (with safety skip)
        if (fade != null)
        {
            Debug.Log("Starting Fade...");
            yield return fade.FadeIn(); 
        }
        else
        {
            Debug.LogWarning("FadePanel is missing. Skipping fade animation.");
        }

        // 4. Load Scene
        Debug.Log($"Attempting to load scene: {nextSceneName}");
        SceneManager.LoadScene(nextSceneName);
    }
}