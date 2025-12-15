using UnityEngine;
using TMPro;
using System.Text;

public class CoffeeGrader : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI adviceText;

    [Header("Score Weights (Must total 1.0)")]
    [SerializeField] private float weightScaling = 0.10f;
    [SerializeField] private float weightGrind = 0.25f;
    [SerializeField] private float weightWaterTemp = 0.20f;
    [SerializeField] private float weightBrewTime = 0.25f;
    [SerializeField] private float weightBloom = 0.20f;




    private void Start()
    {

        var rt = CoffeeRuntime.Instance;
        var ideal = rt.activeRecipe;

        Debug.Log($"[GRADER START] playerFinalWeight={rt.playerFinalWeight} | Time={Time.time}");


        //Base sub-scores from grind scene 
        float techniqueScore = rt.scoreTechnique;
        float saturationScore = rt.scoreSaturation;

        //Base sub-scores from grind scene 
        float grindSizeScore   = rt.scoreTechnique;  
        float grindAmountScore = rt.scoreWeight;    

       // Grind duration sanity penalty (no recipe value needed)
        float grindTimePenalty = 0f;

        // Too fast = sloppy grind
        if (rt.playerGrindDuration < 2.0f)
        {
            grindTimePenalty += 15f;
        }

        // Too slow = overworking beans
        if (rt.playerGrindDuration > 15.0f)
        {
            grindTimePenalty += 10f;
        }

        // over-rotation penalty
        float idealRotation = rt.playerGrindAmount * 180f; // degreesPerGram
        float rotationDiff = Mathf.Max(0f, rt.playerTotalGrindRotation - idealRotation);
        float rotationPenalty = Mathf.Clamp(rotationDiff * 0.05f, 0f, 20f);
 
        //composite grind score
        float grindScore =
        (
            grindSizeScore   * 0.45f +
            grindAmountScore * 0.35f +
            100f             * 0.20f   // baseline behavior credit
        )
        - grindTimePenalty
        - rotationPenalty;

        grindScore = Mathf.Clamp(grindScore, 0f, 100f);


        // --- Individual Scores ---
        float weightScore = ScoreByDifference(
            ideal.coffeeWeightGrams,
            rt.playerFinalWeight,
            10f
        );

        float waterWeightScore = ScoreByDifference (
            ideal.waterWeightGrams,
            rt.playerWaterWeight,
            400f
        );
        float waterTempScore = ScoreByDifference(
            ideal.waterTemperatureCelsius,
            rt.playerWaterTemp,
            5f
        );

        // Use brewing scene score instead of recalculating
        float brewTimeScore = rt.scoreTime;

        float bloomScore = ScoreByDifference(
            ideal.bloomDurationSeconds,
            rt.playerBloomDuration,
            3f
        );

        // ✅ Combined brewing score (bloom + brew)
        float combinedBrewScore =
        (
            brewTimeScore * 0.6f +
            bloomScore    * 0.4f
        );

        combinedBrewScore = Mathf.Clamp(combinedBrewScore, 0f, 100f);


        float finalScore =
        weightScore        * weightScaling +
        grindScore         * weightGrind +
        waterTempScore     * weightWaterTemp +
        combinedBrewScore  * (weightBrewTime + weightBloom);


        // Clamp + round
        finalScore = Mathf.Clamp(finalScore, 0f, 100f);
        finalScore = Mathf.Round(finalScore);

        if (finalScoreText != null)
            finalScoreText.text = $"Final Score: {finalScore}%";
        
        string advice = GenerateAdvice(
            ideal,
            rt,
            weightScore,
            grindScore,
            waterTempScore,
            brewTimeScore,
            bloomScore
        );

        rt.finalAdvice = advice;


        // ⭐ STORE SCORES FOR UI
        rt.scoreScale = weightScore;
        rt.scoreGrind = grindScore;
        rt.scoreHeat  = waterTempScore;
        rt.scoreBrew  = combinedBrewScore; 
        rt.finalScore = finalScore;
    }

    // 🔧 Helper: converts difference into a 0–100 score
    private float ScoreByDifference(float ideal, float actual, float penaltyMultiplier)
    {
        float diff = Mathf.Abs(ideal - actual);
        return Mathf.Clamp(100f - diff * penaltyMultiplier, 0f, 100f);
    }

    private string GenerateAdvice(
    CoffeeBeanRecipe ideal,
    CoffeeRuntime rt,
    float weightScore,
    float grindScore,
    float tempScore,
    float brewScore,
    float bloomScore
)
{
    StringBuilder advice = new StringBuilder();

    // Bean weight
    if (weightScore < 70)
    {
        if (rt.playerFinalWeight > ideal.coffeeWeightGrams)
            advice.AppendLine("• You used more coffee than needed, which can make the brew overly strong.");
        else
            advice.AppendLine("• You used too little coffee, resulting in a weak extraction.");
    }

    // Grind size
    if (grindScore < 70)
    {
        if (rt.playerActualGrindValue > (int)ideal.idealGrindSize)
            advice.AppendLine("• The grind was too coarse, leading to under-extraction.");
        else
            advice.AppendLine("• The grind was too fine, which may cause bitterness.");
    }

    // Water temperature
    if (tempScore < 70)
    {
        if (rt.playerWaterTemp > ideal.waterTemperatureCelsius)
            advice.AppendLine("• Water was too hot, extracting harsh flavors.");
        else
            advice.AppendLine("• Water was too cool, limiting extraction.");
    }

    // Brew time
    if (brewScore < 70)
    {
        if (rt.playerBrewTime > ideal.brewTimeSeconds)
            advice.AppendLine("• Brew time was too long, which can mute flavors.");
        else
            advice.AppendLine("• Brew time was too short, preventing full extraction.");
    }

    if (CoffeeRuntime.Instance.scoreTechnique < 70)
    {
        advice.AppendLine("• Pouring technique was inconsistent, affecting extraction quality.");
    }

    if (CoffeeRuntime.Instance.scoreSaturation < 70)
    {
        advice.AppendLine("• Water coverage was uneven, leading to patchy extraction.");
    }

    // Bloom
    if (bloomScore < 70)
    {
        advice.AppendLine("• The bloom phase could be improved to release trapped gases.");
    }

    if (advice.Length == 0)
    {
        advice.Append("• Excellent brew! Your technique was well balanced.");
    }

    return advice.ToString();
}

}
