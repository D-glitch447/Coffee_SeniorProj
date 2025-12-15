using UnityEngine;

public class GrindEvaluator : MonoBehaviour
{
    public static void Evaluate()
    {
        CoffeeRuntime rt = CoffeeRuntime.Instance;
        CoffeeBeanRecipe recipe = rt.activeRecipe;

        float ideal = (float)recipe.idealGrindSize;
        float actual = rt.playerActualGrindValue;

        rt.scoreTechnique = Mathf.Clamp(100 - Mathf.Abs(ideal - actual) * 12f, 0, 100);
        rt.scoreWeight = Mathf.Clamp(
            100 - Mathf.Abs(rt.playerGrindAmount - recipe.coffeeWeightGrams) / recipe.coffeeWeightGrams * 100f, 0,100);

        //grind size category selection penalty
        int selectedIndex = rt.playerSelectedGrindIndex;
        int idealIndex = (int)recipe.idealGrindSize;

        int indexDiff = Mathf.Abs(selectedIndex - idealIndex);
        float selectionPenalty = indexDiff * 5f;

        rt.scoreTechnique = Mathf.Clamp(
            rt.scoreTechnique - selectionPenalty,
            0, 100
        );
    }
}
