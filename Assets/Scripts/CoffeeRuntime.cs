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
    public bool beanWeightLocked = false;

    // public float playerFinalWeight;
    private float _playerFinalWeight;
    public float playerFinalWeight
    {
        get => _playerFinalWeight;
        set
        {
            if (beanWeightLocked)
            {
                Debug.LogWarning($"[BLOCKED WRITE] Attempt to overwrite bean weight with {value}");
                return;
            }

            Debug.Log($"[RUNTIME WRITE] playerFinalWeight = {value} | Time={Time.time}");
            _playerFinalWeight = value;
        }
    }



    public float playerGrindAmount;
    public int playerSelectedGrindIndex;
    public float playerActualGrindValue;
    public int playerGrindSizeIndex;
    public string playerGrindSizeName; 
    public float playerWaterTemp;
    public float playerWaterWeight;
    public float playerBrewTime;
    public float playerBloomWaterUsed;
    public float playerBloomDuration;
    public float playerGrindDuration;
    public float playerTotalGrindRotation;
    public float grinderAttempts;

    // --- Player Scoring Stats (Passed from BedManager) ---
    // These store the 0-100 scores for each category
    public float scoreWeight;
    public float scoreSaturation;
    public float scoreTechnique;
    public float scoreTime;
    public float scoreImpatiencePenalty;
    public float scoreGrindSize;
    public float scoreGrindAmount;

    public bool hasCompletedScale = false;
    public bool hasCompletedGrind = false;
    public bool hasCompletedMeasuringWater = false;
    public bool hasCompleteHeating = false;
    public bool hasCompletedBrewing = false;
    public string finalAdvice;


    public float scoreScale;
    public float scoreGrind;
    public float scoreHeat;
    public float scoreBrew;
    public float finalScore;


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

    public void ResetCurrentRun()
    {
        // --- Active recipe ---
        activeRecipe = null;

        // --- Player raw stats ---
        playerFinalWeight = 0f;
        playerGrindAmount = 0f;
        playerSelectedGrindIndex = 0;
        playerActualGrindValue = 0f;
        playerGrindSizeIndex = 0;
        playerGrindSizeName = string.Empty;
        playerWaterTemp = 0f;
        playerWaterWeight = 0f;
        playerBrewTime = 0f;
        playerBloomWaterUsed = 0f;
        playerBloomDuration = 0f;

        // --- Scoring stats ---
        scoreWeight = 0f;
        scoreSaturation = 0f;
        scoreTechnique = 0f;
        scoreTime = 0f;
        scoreImpatiencePenalty = 0f;
        scoreGrindSize = 0f;
        scoreGrindAmount = 0f;
        scoreScale = 0f;
        scoreGrind = 0f;
        scoreHeat = 0;
        scoreBrew = 0f; 
        finalScore = 0f;

        // --- Progress flags ---
        hasCompletedScale = false;
        hasCompletedGrind = false;
        hasCompletedMeasuringWater = false;
        hasCompleteHeating = false;
        hasCompletedBrewing = false;
        finalAdvice = null;

        // --- Reset states ---
        kitchenState = KitchenState.FirstTime;
        prepRoomState = PrepRoomState.FirstTime;

        Debug.Log("[CoffeeRuntime] Current run reset");
    }


}