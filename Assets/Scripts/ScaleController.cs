using UnityEngine;
using TMPro;

public class ScaleController : MonoBehaviour
{
    public static ScaleController Instance;
    public TMP_Text ScaleText;
    private float totalWeight = 0f;

    private bool shownTooMuchDialogue = false;

    void Awake()
    {
        Instance = this;
    }

    public float GetCurrentWeight()
    {
        return totalWeight;
    }

    public void AddWeight(float w)
    {
        totalWeight += w;
        if(totalWeight <0f) totalWeight = 0f;

        float target = CoffeeRuntime.Instance.activeRecipe.coffeeWeightGrams;

        if(totalWeight > target && !shownTooMuchDialogue)
        {
            shownTooMuchDialogue = true;

            TutorialManager.Instance.ShowTooManyBeansWarning();
        }
        Debug.Log($"Scale new weight: {totalWeight}g");

        UpdateScaleDisplay();
    }

    public void ClearWeight()
    {
        totalWeight = 0f;
        UpdateScaleDisplay();
    }

    private void UpdateScaleDisplay()
    {
        ScaleText.text = $"{totalWeight:F1} ";
    }

    public void RemoveWeight(float w)
    {
        totalWeight -= w;
        if (totalWeight < 0f) totalWeight = 0f;
        UpdateScaleDisplay();
    }
}