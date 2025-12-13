using UnityEngine;
public enum KitchenState
{
    FirstTime,
    AfterRecipeSelected,
    AfterScaling,
    AfterGrinding,
    AfterBrewing
}

public enum PrepRoomState
{
    FirstTime,          // show sink dialogue
    AfterSink,          // show stove dialogue
}

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
    public float grindPenalty;

    public bool hasCompletedScale = false;
    public bool hasCompletedGrind = false;
    public bool hasCompletedMeasuringWater = false;
    public bool hasCompleteHeating = false;
    public bool hasCompletedBrewing = false;

    public KitchenState kitchenState = KitchenState.FirstTime;
    public PrepRoomState prepRoomState = PrepRoomState.FirstTime;


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
