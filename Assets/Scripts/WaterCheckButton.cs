using UnityEngine;

public class WaterCheckButton : MonoBehaviour
{
    public WaterFillController kettle;   // reference to your kettle script

    public void CheckWater()
    {
        float waterAmount = kettle.currentWater;

        // Store the final water amount in CoffeeRuntime
        CoffeeRuntime.Instance.playerWaterWeight = waterAmount;
        CoffeeRuntime.Instance.hasCompletedMeasuringWater = true;

        // Debug log result
        Debug.Log($"[WATER CHECK] Final Water: {waterAmount} g");

        // Optional: penalty check (if you want to use it later)
        if (CoffeeRuntime.Instance.activeRecipe != null)
        {
            float target = CoffeeRuntime.Instance.activeRecipe.waterWeightGrams;

            if (waterAmount > target)
            {
                float over = waterAmount - target;
                float penaltyPoints = Mathf.Floor(over / 5f) * 1f; // 1 point per 5g

                Debug.Log($"[WATER CHECK] Over target by {over} g → Penalty: {penaltyPoints} points");
            }
        }
        FadeController.Instance.FadeToScene("MeasuringWater");
    }
}

