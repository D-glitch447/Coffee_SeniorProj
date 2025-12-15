using UnityEngine;
using TMPro;
public class AdviceUIShowcase : MonoBehaviour
{
  
    [SerializeField] private TextMeshProUGUI adviceText;

    private void OnEnable()
    {
        if (CoffeeRuntime.Instance == null)
            return;

        string advice = CoffeeRuntime.Instance.finalAdvice;

        if (string.IsNullOrEmpty(advice))
            advice = "Nice work! This was a solid cup of coffee.";

        adviceText.text = advice;
    }
}

