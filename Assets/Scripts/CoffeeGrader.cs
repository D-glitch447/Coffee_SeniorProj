using UnityEngine;

public class CoffeeGrader : MonoBehaviour
{
    void Start()
    {
        //Will modify to corrdinate with the actual formulas created
        var ideal = CoffeeRuntime.Instance.activeRecipe;

        float weightScore = Mathf.Max(0, 100 - Mathf.Abs(ideal.coffeeWeightGrams - CoffeeRuntime.Instance.playerFinalWeight) * 10);
        float grindScore = Mathf.Max(0, 100 - Mathf.Abs(ideal.grindSize - CoffeeRuntime.Instance.playerGrindSizeIndex) * 15);
        float waterScore = Mathf.Max(0, 100 - Mathf.Abs(ideal.waterTemperatureCelsius - CoffeeRuntime.Instance.playerWaterTemp) * 5);
        float brewScore = Mathf.Max(0, 100 - Mathf.Abs(ideal.brewTimeSeconds - CoffeeRuntime.Instance.playerBrewTime) * 4);

        float finalScore = (weightScore + grindScore + waterScore + brewScore) / 4f;

        Debug.Log("Final Coffee Score: " + finalScore);



    }
}
