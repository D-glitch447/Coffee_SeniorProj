using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoffeeGrinder : MonoBehaviour
{
    [Header("Grind Difficulty")]
    // How much rotation (in degrees) is needed to grind 1 gram?
    // 3600 degrees (10 full circles) for 20g = 180 degrees per gram.
    public float degreesPerGram = 180f; 
    
    [Header("Live Data (Read Only)")]
    public float targetGrams = 20.0f; // Will be overwritten by Recipe
    public float totalGrindRequired;  // Calculated based on target
    public float currentGrams = 0f;

    [Header("Visuals (GameObjects)")]
    public GameObject fullGrinderObject;
    public GameObject emptyGrinderObject;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip grindingSound;
    public AudioClip emptyGrinderSound;

    [Header("UI")]
    public Image progressFillImage;
    public TextMeshProUGUI grindAmountText;

    // Private variables
    private float currentGrindProgress = 0f; // Total degrees rotated so far
    private bool hasReachedOptimalGrind = false;
    private float lastGrindTime; 

    private void Start()
    {
        // 1. LOAD TARGET FROM RECIPE
        if (CoffeeRuntime.Instance != null && CoffeeRuntime.Instance.activeRecipe != null)
        {
            targetGrams = CoffeeRuntime.Instance.activeRecipe.coffeeWeightGrams;
        }

        // 2. Calculate total rotation needed based on target
        totalGrindRequired = targetGrams * degreesPerGram;

        // 3. Reset UI
        if (progressFillImage != null) progressFillImage.fillAmount = 0;
        if (audioSource != null) audioSource.loop = true;
        
        UpdateGrindText();
    }

    private void Update()
    {
        // Audio Timeout Logic
        if (Time.time - lastGrindTime > 0.1f)
        {
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Pause();
            }
        }
    }

    // Called by GrinderHandle.cs
    public void AddGrindProgress(float angleAmount)
    {
        lastGrindTime = Time.time;
        PlayGrindSound();

        // Allow grinding as long as we haven't finished
        if (!hasReachedOptimalGrind && angleAmount > 0)
        {
            currentGrindProgress += angleAmount;

            // Cap progress at 100%
            if (currentGrindProgress >= totalGrindRequired)
            {
                currentGrindProgress = totalGrindRequired;
                hasReachedOptimalGrind = true;
                OnGrindComplete();
            }

            // Update Calculations
            currentGrams = (currentGrindProgress / totalGrindRequired) * targetGrams;

            // Update UI
            if (progressFillImage != null)
                progressFillImage.fillAmount = currentGrindProgress / totalGrindRequired;
            
            UpdateGrindText();
        }
    }

    private void PlayGrindSound()
    {
        if (audioSource == null) return;
        AudioClip correctClip = hasReachedOptimalGrind ? emptyGrinderSound : grindingSound;

        if (audioSource.clip != correctClip)
        {
            audioSource.clip = correctClip;
            audioSource.Play();
        }
        else if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    private void OnGrindComplete()
    {
        Debug.Log("Grinding Complete!");
        // Optional: Auto-hide visuals, or wait for player to click Finish
        // if (fullGrinderObject != null) fullGrinderObject.SetActive(false);
        // if (emptyGrinderObject != null) emptyGrinderObject.SetActive(true);
    }

    private void UpdateGrindText()
    {
        if (grindAmountText != null)
        {
            // Format: "12.5g / 20.0g"
            grindAmountText.text = $"{currentGrams:F1}g / {targetGrams:F1}g";
        }
    }

    public float GetCurrentGrams()
    {
        return currentGrams;
    }
}