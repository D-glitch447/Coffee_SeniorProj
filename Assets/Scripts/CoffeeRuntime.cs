using UnityEngine;

public class CoffeeRuntime : MonoBehaviour
{
    public static CoffeeRuntime Instance;

    public CoffeeBeanRecipe activeRecipe;

    // Player performance values
    public float playerFinalWeight;
    public float playerGrindSize;
    public float playerWaterTemp;
    public float playerWaterWeight;
    public float playerBrewTime;

    // 🔹 NEW: progression flag
    public bool hasCompletedScale = false;
    public bool hasCompletedGrind = false;

    public bool hasCompletedMeasuringWater = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

}
