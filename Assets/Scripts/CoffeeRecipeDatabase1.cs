using UnityEngine;

[CreateAssetMenu(fileName = "CoffeeRecipeDatabase1", menuName = "Scriptable Objects/CoffeeRecipeDatabase1")]
public class CoffeeRecipeDatabase1 : ScriptableObject
{
    public CoffeeBeanRecipe[] allRecipes;

    public CoffeeBeanRecipe GetRecipe(string name)
    {
        foreach(var recipe in allRecipes)
        {
            if(recipe.recipeName.ToLower() == name.ToLower()) 
                return recipe;
        }
        return null;
    }
}
