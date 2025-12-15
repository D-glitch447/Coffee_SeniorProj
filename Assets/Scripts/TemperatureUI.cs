// using UnityEngine;
// using UnityEngine.UI;
// using System.Collections;
// using TMPro;

// public class TemperatureUI : MonoBehaviour
// {
//     [Header("UI References")]
//     public Image tempBar;
//     public TextMeshProUGUI tempText;

//     [Header("Temperature Values")]
//     public float currentTemp = 25f;
//     public float maxTemp = 100f;

//     [Header("Rates")]
//     public float heatRate = 2f;       // how fast it heats

//     [Header("Delays")]
//     public float heatDelay = 0.6f;    // wait before heating starts
//     private bool isHeatingRequested = false;
//     private bool isHeating = false;

//     // void Start()
//     // {
//     //     // The UI object itself starts OFF
//     //     gameObject.SetActive(false);
//     // }

//     void Update()
//     {
//         if (tempBar == null || tempText == null)
//             return;

//         // Heating
//         if (isHeating)
//         {
//             currentTemp += heatRate * Time.deltaTime;
//             currentTemp = Mathf.Clamp(currentTemp, 25f, maxTemp);
//         }


//         // Update UI
//         tempBar.fillAmount = currentTemp / maxTemp;
//         tempText.text = Mathf.RoundToInt(currentTemp) + "°C";
//     }
//     public void ShowUI()
//     {
//         // enable full UI container
//         gameObject.SetActive(true);
//     }

//     public void HideUI()
//     {
//         isHeating = false;
//         isHeatingRequested = false;
//         // hide full UI
//         gameObject.SetActive(false);
//     }

//     public void RequestHeating()
//     {

//         if (!isHeatingRequested)
//             StartCoroutine(HeatAfterDelay());
//     }

//     private IEnumerator HeatAfterDelay()
//     {
//         isHeatingRequested = true;
//         yield return new WaitForSeconds(heatDelay);
//         isHeatingRequested = false;
//         isHeating = true;
//     }
//      public void StopHeating()
//     {
//         isHeating = false;
//     }

//     public float GetCurrentTemp()
//     {
//         return currentTemp;
//     }
//     private void UpdateUIImmediate()
//     {
//         tempBar.fillAmount = currentTemp / maxTemp;
//         tempText.text = Mathf.RoundToInt(currentTemp) + "°C";
//     }
// }

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class TemperatureUI : MonoBehaviour
{
    [Header("UI References")]
    public Image tempBar;
    public TextMeshProUGUI tempText;

    [Header("Temperature Values")]
    public float currentTemp = 25f;
    public float maxTemp = 100f;

    [Header("Rates")]
    public float heatRate = 2f;

    [Header("Delays")]
    public float heatDelay = 0.6f;

    // 🔒 Internal state
    private bool isHeating = false;
    private bool targetHeatingState = false;
    private Coroutine temperatureCoroutine;

    void Awake()
    {
        // Just make sure UI is synced
        UpdateUIImmediate();
    }

    void Update()
    {
        if (!isHeating) return;

        currentTemp += heatRate * Time.deltaTime;
        currentTemp = Mathf.Clamp(currentTemp, 25f, maxTemp);

        UpdateUIImmediate();
    }

    public void ShowUI()
    {
        // Keep object active; visuals should be handled elsewhere if needed
        UpdateUIImmediate();
    }

    public void HideUI()
    {
        StopHeating();
    }
    public void RequestHeating()
    {
        SetHeating(true);
    }

    public void StopHeating()
    {
        SetHeating(false);
    }

    private void SetHeating(bool heat)
    {
        // 🚫 Ignore duplicate requests
        if (targetHeatingState == heat)
            return;

        targetHeatingState = heat;

        // 🛑 Stop any existing coroutine
        if (temperatureCoroutine != null)
            StopCoroutine(temperatureCoroutine);

        temperatureCoroutine = StartCoroutine(HeatingRoutine(heat));
    }

    private IEnumerator HeatingRoutine(bool heat)
    {
        if (heat)
        {
            yield return new WaitForSeconds(heatDelay);

            // State might have changed during delay
            if (targetHeatingState != true)
                yield break;

            isHeating = true;
        }
        else
        {
            // Immediate stop (no cooling here yet)
            isHeating = false;
        }
    }

    // --------------------------------------------------
    // UI UPDATE
    // --------------------------------------------------
    private void UpdateUIImmediate()
    {
        if (tempBar != null)
            tempBar.fillAmount = currentTemp / maxTemp;

        if (tempText != null)
            tempText.text = Mathf.RoundToInt(currentTemp) + "°C";
    }

    public float GetCurrentTemp()
    {
        return currentTemp;
    }
}
