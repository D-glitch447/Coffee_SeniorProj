using UnityEngine;

public class StoveClickable : MonoBehaviour
{
    private void OnMouseDown()
    {
        // CoffeeRuntime must exist
        if (CoffeeRuntime.Instance == null)
        {
            Debug.LogError("CoffeeRuntime is missing!");
            return;
        }

        // Require water step to be finished
        if (!CoffeeRuntime.Instance.hasCompletedMeasuringWater)
        {
            Debug.Log("Stove is locked. Measure the water first!");
            return;
        }

        // Everything is good → fade and load scene
        FadeController.Instance.FadeToScene("StoveCloseUp");
    }
}
