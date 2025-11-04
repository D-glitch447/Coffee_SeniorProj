using UnityEngine;
using TMPro;

public class ScaleController : MonoBehaviour
{

    public static ScaleController Instance;
    public TMP_Text ScaleText;
    private float totalWeight = 0f;

    void Awake()
    {
        Instance = this;
    }

    public void AddWeight(float w)
    {
        totalWeight += w;
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
}
