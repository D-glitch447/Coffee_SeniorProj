using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class FinalGrind : MonoBehaviour
{
    [Header("Dependencies")]
    public FadeController fade;          // Drag your FadePanel/Controller here
    public CoffeeGrinder coffeeGrinder;  // Drag the object with CoffeeGrinder.cs here

    [Header("Scene Settings")]
    public string nextSceneName = "NextSceneName"; // Type the exact name of the next scene

    // Link this function to your "Finish" button
    public void OnFinishClicked()
    {
        StartCoroutine(FinishSequence());
    }

    private IEnumerator FinishSequence()
    {
        // 1. Fade to black
        if (fade != null)
            yield return fade.FadeIn();

        // 2. Capture Data
        if (CoffeeRuntime.Instance != null)
        {
            // --- Capture Grind Amount ---
            if (coffeeGrinder != null)
            {
                float finalGrams = coffeeGrinder.GetCurrentGrams();
                CoffeeRuntime.Instance.playerGrindAmount = finalGrams;
                
                // Optional: Store the target if you need to compare later
                // CoffeeRuntime.Instance.targetGrindAmount = coffeeGrinder.targetGrams;
                
                Debug.Log($"Saved Grind Amount: {finalGrams}g");
            }

            // 3. Mark step as completed
            CoffeeRuntime.Instance.hasCompletedGrind = true;
        }

        // 4. Load Next Scene
        SceneManager.LoadScene(nextSceneName);
    }
}