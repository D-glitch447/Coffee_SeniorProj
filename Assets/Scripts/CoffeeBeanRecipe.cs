using UnityEngine;

public enum PourPattern
{
    Any,
    Circular,   // Matches "Circular Motion"
    Center      // Matches "Spot Pour / Center"
}

[CreateAssetMenu(fileName = "CoffeeBeanRecipe", menuName = "Scriptable Objects/CoffeeBeanRecipe")]
public class CoffeeBeanRecipe : ScriptableObject
{
    [Header("Identity")]
    public string recipeName;

    [Header("Brewing Specs")]
    public float brewTimeSeconds;
    public float waterTemperatureCelsius;
    public GrindSize idealGrindSize;
    // public float grindSize;
    public float coffeeWeightGrams;
    public float waterWeightGrams;

    [Header("Bloom Specs")]
    public float bloomWaterGrams = 40f;
    public float bloomDurationSeconds = 30f;

    [Header("Technique Specs")]
    [Tooltip("If true, pouring too fast reduces score.")]
    public bool requiresGentlePour = true; 

    [Tooltip("The specific motion required.")]
    public PourPattern optimalPattern; // Uses the Enum above

    [Header("Flavor Notes")]
    public string flavorNotes;
}