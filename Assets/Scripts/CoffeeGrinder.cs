using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoffeeGrinder : MonoBehaviour
{
    [Header("Grind Difficulty")]
    public float degreesPerGram = 180f;

    [Header("Live Data (Read Only)")]
    public float targetGrams;
    public float totalGrindRequired;
    public float currentGrams = 0f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip grindingSound;
    public AudioClip emptyGrinderSound;

    [Header("UI")]
    public Image progressFillImage;
    public TextMeshProUGUI grindAmountText;

    // --- Existing ---
    private float currentGrindProgress = 0f;
    private bool hasReachedOptimalGrind = false;
    private float lastGrindTime;

    // ✅ ADDED – technique simulation
    [Header("Technique Simulation")]
    public float grindSpeedThreshold = 800f;
    public float grindDriftRate = 0.35f;
    private float grindSizeError = 0f;

    [SerializeField] private GrinderHandle grinderHandle; 
    private float grindStartTime;
    private bool grindStarted = false;


    private void Start()
    {
        Debug.Log("[CoffeeGrinder] Start() fired");

        if(CoffeeRuntime.Instance.playerFinalWeight > 0f)
        {
            targetGrams = CoffeeRuntime.Instance.playerFinalWeight;
        } 
        else if (CoffeeRuntime.Instance.activeRecipe != null)
        {
            targetGrams = CoffeeRuntime.Instance.activeRecipe.coffeeWeightGrams;
        }

        totalGrindRequired = targetGrams * degreesPerGram;

        if (progressFillImage != null) progressFillImage.fillAmount = 0;
        if (audioSource != null) audioSource.loop = true;

        UpdateGrindText();
    }

    private void Update()
    {
        if (Time.time - lastGrindTime > 0.1f)
        {
            if (audioSource != null && audioSource.isPlaying)
                audioSource.Pause();
        }
    }

    public void AddGrindProgress(float angleAmount)
    {
         if (!grindStarted)
        {
            grindStarted = true;
            grindStartTime = Time.time;
        }

        lastGrindTime = Time.time;
        PlayGrindSound();

        if (!hasReachedOptimalGrind && angleAmount > 0)
        {
            // ✅ Technique drift
            float speed = angleAmount / Time.deltaTime;

            if (speed > grindSpeedThreshold)
                grindSizeError -= grindDriftRate * Time.deltaTime;
            else
                grindSizeError += grindDriftRate * Time.deltaTime * 0.5f;

            currentGrindProgress += angleAmount;

            if (currentGrindProgress >= totalGrindRequired)
            {
                currentGrindProgress = totalGrindRequired;
                hasReachedOptimalGrind = true;
                OnGrindComplete();
            }

            currentGrams = (currentGrindProgress / totalGrindRequired) * targetGrams;

            if (progressFillImage != null)
                progressFillImage.fillAmount = currentGrindProgress / totalGrindRequired;

            UpdateGrindText();
        }
    }

    private void PlayGrindSound()
    {
        if (audioSource == null) return;

        AudioClip clip = hasReachedOptimalGrind ? emptyGrinderSound : grindingSound;

        if (audioSource.clip != clip)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
        else if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
    private void OnGrindComplete()
    {
        float selected = CoffeeRuntime.Instance.playerSelectedGrindIndex;

        // ✅ SAFETY (prevents default-4 confusion)
        if (selected <= 0)
            selected = 4f;

        float actual = Mathf.Clamp(selected + grindSizeError, 1f, 8f);

        CoffeeRuntime.Instance.playerActualGrindValue = actual;
        CoffeeRuntime.Instance.playerGrindAmount = currentGrams;
        CoffeeRuntime.Instance.hasCompletedGrind = true;
        CoffeeRuntime.Instance.playerGrindDuration = Time.time - grindStartTime;

         if (grinderHandle != null)
            grinderHandle.FinalizeGrindStats();
        GrindEvaluator.Evaluate();

        Debug.Log($"Grinding Complete | Ideal {selected} → Actual {actual:F2}");
    }

    // private void OnGrindComplete()
    // {
    //     float selected = CoffeeRuntime.Instance.playerSelectedGrindIndex;
    //     if (selected <= 0)
    //         selected = 4f; // Medium fallback

    //     float actual = Mathf.Clamp(selected + grindSizeError, 1f, 8f);

    //     // float selected = CoffeeRuntime.Instance.playerSelectedGrindIndex;
    //     // float actual = Mathf.Clamp(selected + grindSizeError, 1f, 8f);

    //     CoffeeRuntime.Instance.playerActualGrindValue = actual;
    //     CoffeeRuntime.Instance.playerGrindAmount = currentGrams;
    //     CoffeeRuntime.Instance.hasCompletedGrind = true;

    //     GrindEvaluator.Evaluate();

    //     Debug.Log($"Grinding Complete | Selected {selected} → Actual {actual:F2}");
    //     Debug.Log($"Selected: {CoffeeRuntime.Instance.playerSelectedGrindIndex}");
    //     Debug.Log($"Actual: {CoffeeRuntime.Instance.playerActualGrindValue}");
    //     Debug.Log($"Score: {CoffeeRuntime.Instance.scoreTechnique}");
    // }

    private void UpdateGrindText()
    {
        if (grindAmountText != null)
            grindAmountText.text = $"{currentGrams:F1}g / {targetGrams:F1}g";
    }
    public float GetCurrentGrams()
    {
        return currentGrams;
    }

}

