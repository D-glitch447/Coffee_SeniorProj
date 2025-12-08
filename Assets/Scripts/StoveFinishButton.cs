using UnityEngine;
using UnityEngine.UI;

public class StoveFinishButton : MonoBehaviour
{
    public StoveBurner leftBurner;     // assign in inspector
    public StoveBurner rightBurner;    // assign in inspector

    // ideal temperature window
    public float perfectMin = 90f;
    public float perfectMax = 96f;
    public float penaltyMultiplier = 0.5f;

    // called by the UI Button OnClick()
    public void FinishHeating()
    {
        if (CoffeeRuntime.Instance == null)
        {
            Debug.LogError("CoffeeRuntime missing!");
            return;
        }

        TemperatureUI chosenUI = null;

        // determine which burner has the kettle
        if (leftBurner != null && leftBurner.hasKettle)
            chosenUI = leftBurner.tempUI;

        if (rightBurner != null && rightBurner.hasKettle)
            chosenUI = rightBurner.tempUI;

        // if neither burner has the kettle
        if (chosenUI == null)
        {
            Debug.Log("No kettle detected on any burner!");
            return;
        }

        float finalTemp = chosenUI.currentTemp;
        // float penalty = CalculatePenalty(finalTemp);

        CoffeeRuntime.Instance.playerWaterTemp = finalTemp;
        // CoffeeRuntime.Instance.waterTempPenalty = penalty;

        Debug.Log("Final Temperature: " + finalTemp);
        // Debug.Log("Penalty: " + penalty);

        // fade into next scene
        FadeController.Instance.FadeToScene("Kitchen");
    }

    // private float CalculatePenalty(float temp)
    // {
    //     if (temp >= perfectMin && temp <= perfectMax)
    //         return 0f;

    //     if (temp > perfectMax)
    //         return (temp - perfectMax) * penaltyMultiplier;

    //     if (temp < perfectMin)
    //         return (perfectMin - temp) * penaltyMultiplier;

    //     return 0f;
    // }
}
