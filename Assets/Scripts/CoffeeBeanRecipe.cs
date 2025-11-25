using UnityEngine;

[CreateAssetMenu(fileName = "CoffeeBeanRecipe", menuName = "Scriptable Objects/CoffeeBeanRecipe")]
public class CoffeeBeanRecipe : ScriptableObject
{
    [Header("Identity")]
    public string recipeName;

    [Header("Brewing Specs")]
    public float brewTimeSeconds;
    public float waterTemperatureCelsius;
    public float grindSize;
    public float coffeeWeightGrams;
    public float waterWeightGrams;

    [Header("Flavor Notes")]
    public string flavorNotes;
}
