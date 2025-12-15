using UnityEngine;

public class SimpleGraphController : MonoBehaviour
{
    public SimpleBarRow beanRow;
    public SimpleBarRow waterWeightRow;
    public SimpleBarRow waterTempRow;
    public SimpleBarRow grindRow;
    public SimpleBarRow bloomWaterRow;
    public SimpleBarRow bloomTimeRow;

    private void Start()
    {
        var rt = CoffeeRuntime.Instance;
        var recipe = rt.activeRecipe;

        beanRow.SetValues(
            recipe.coffeeWeightGrams,
            rt.playerFinalWeight,
            "g"
        );

        // Water amount
        waterWeightRow.SetValues(
            recipe.waterWeightGrams,
            rt.playerWaterWeight,
            "g"
        );

         // Water temperature
        waterTempRow.SetValues(
            recipe.waterTemperatureCelsius,
            rt.playerWaterTemp,
            "°C"
        );

        // Grind size
        grindRow.SetValues(
            (float)recipe.idealGrindSize,
            rt.playerActualGrindValue
        );

        // Bloom water
        bloomWaterRow.SetValues(
            recipe.bloomWaterGrams,
            rt.playerBloomWaterUsed,
            "g"
        );

        // Bloom time
        bloomTimeRow.SetValues(
            recipe.bloomDurationSeconds,
            rt.playerBloomDuration,
            "s"
        );

    }
}
