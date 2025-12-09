using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaterFillController : MonoBehaviour
{
    [Header("References")]
    public FaucetToggle faucet;         // faucet reference
    public TextMeshProUGUI weightText;  // UI text element
    public Image waterFillImage;        // THIS object’s Image

    [Header("Fill Settings")]
    public float fillRate = 20f;        // grams per second
    public float maxWeight = 400f;      
    private float currentWeight = 0f;

    private void Awake()
    {
        waterFillImage = GetComponent<Image>();

        if(waterFillImage == null) 
            Debug.LogError("[WaterFillController] No Image found on WaterFillBar!");
        if(faucet == null)
            Debug.LogWarning("[WaterFillController] Faucet not assigned");
        if(weightText == null)
            Debug.LogWarning("[WaterFillController] WeightTect is not assigned");
    }

    private void Start()
    {
        currentWeight = 0f;
        UpdateUI();
    }
    private void Update()
    {
        if (faucet == null || waterFillImage == null)
            return;
        
        if(faucet.IsOn && currentWeight < maxWeight)
        {
            currentWeight += fillRate * Time.deltaTime;
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        currentWeight = Mathf.Clamp(currentWeight, 0f, maxWeight);

        waterFillImage.fillAmount = currentWeight/maxWeight;

        if(weightText != null) 
            weightText.text = $"{currentWeight:F0} g";
    }

    public float GetFinalWeight()
    {
        return currentWeight;
    }
}
