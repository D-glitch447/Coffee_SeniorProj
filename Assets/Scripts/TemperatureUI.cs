// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;
// using System.Collections;

// public class TemperatureUI : MonoBehaviour
// {
//     public Image tempBar;
//     public TextMeshProUGUI tempText;

//     public float currentTemp = 25f;
//     public float maxTemp = 100f;
//     public float heatRate = 5f;
//     public float coolRate = 2f;

//     private bool isHeating = false;
//     private Coroutine tempRoutine;

//     void Awake()
//     {
//         // ROOT stays active → allowed for coroutines
//         // Children start hidden
//         tempBar.gameObject.SetActive(false);
//         tempText.gameObject.SetActive(false);
//     }

//     public void ShowUI()
//     {
//         tempBar.gameObject.SetActive(true);
//         tempText.gameObject.SetActive(true);
//     }

//     public void HideUI()
//     {
//         tempBar.gameObject.SetActive(false);
//         tempText.gameObject.SetActive(false);
//     }

//     public void SetHeating(bool heating)
//     {
//         isHeating = heating;

//         if (tempRoutine != null)
//             StopCoroutine(tempRoutine);

//         tempRoutine = StartCoroutine(HeatCoolRoutine());
//     }

//     private IEnumerator HeatCoolRoutine()
//     {
//         // Delay before heating or cooling
//         yield return new WaitForSeconds(1.0f);

//         while (true)
//         {
//             if (isHeating)
//                 currentTemp += heatRate * Time.deltaTime;
//             else
//                 currentTemp -= coolRate * Time.deltaTime;

//             currentTemp = Mathf.Clamp(currentTemp, 25f, maxTemp);

//             tempBar.fillAmount = currentTemp / maxTemp;
//             tempText.text = Mathf.RoundToInt(currentTemp) + "°C";

//             yield return null;
//         }
//     }

//     public float GetCurrentTemp() => currentTemp;
// }
// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;
// using System.Collections;

// public class TemperatureUI : MonoBehaviour
// {
//     [Header("UI Elements")]
//     public GameObject visualGroup;          // A parent object that contains TempText + TempBar
//     public Image tempBar;
//     public TextMeshProUGUI tempText;

//     [Header("Temperature Values")]
//     public float currentTemp = 25f;
//     public float maxTemp = 100f;
//     public float heatRate = 5f;
//     public float coolRate = 2f;

//     [Header("Delays")]
//     public float heatDelay = 0.5f;          // wait before temperature rises
//     public float coolDelay = 2.0f;          // wait before temperature cools

//     private bool isHeating = false;
//     private Coroutine tempRoutine;

//     void Awake()
//     {
//         // IMPORTANT: TemperatureUI object MUST be active in hierarchy.
//         // We disable ONLY the visual group (so coroutines can run).
//         if (visualGroup != null)
//             visualGroup.SetActive(false);
//     }

//     //---------------------------------------------
//     // CALLED BY StoveBurner — turns heating ON/OFF
//     //---------------------------------------------
//     public void SetHeating(bool heating)
//     {
//         if (tempRoutine != null)
//             StopCoroutine(tempRoutine);

//         tempRoutine = StartCoroutine(TemperatureRoutine(heating));
//     }

//     //---------------------------------------------
//     // CALLED BY StoveBurner — shows UI when kettle is placed
//     //---------------------------------------------
//     public void ShowUI()
//     {
//         if (visualGroup != null)
//             visualGroup.SetActive(true);
//     }

//     //---------------------------------------------
//     // CALLED BY StoveBurner — hides UI when kettle removed
//     //---------------------------------------------
//     public void HideUI()
//     {
//         if (visualGroup != null)
//             visualGroup.SetActive(false);
//     }

//     //---------------------------------------------
//     // MAIN HEATING / COOLING COROUTINE
//     //---------------------------------------------
//     private IEnumerator TemperatureRoutine(bool heating)
//     {
//         if (heating)
//         {
//             // Delay before heating begins
//             yield return new WaitForSeconds(heatDelay);

//             while (heating)
//             {
//                 currentTemp += heatRate * Time.deltaTime;
//                 currentTemp = Mathf.Clamp(currentTemp, 25f, maxTemp);
//                 UpdateUI();

//                 yield return null;
//                 heating = isHeating;  // updated externally
//             }
//         }
//         else
//         {
//             // Delay before cooling begins
//             yield return new WaitForSeconds(coolDelay);

//             while (!heating)
//             {
//                 currentTemp -= coolRate * Time.deltaTime;
//                 currentTemp = Mathf.Clamp(currentTemp, 25f, maxTemp);
//                 UpdateUI();

//                 yield return null;
//             }
//         }
//     }

//     //---------------------------------------------
//     // UI Update helper
//     //---------------------------------------------
//     private void UpdateUI()
//     {
//         if (tempBar != null)
//             tempBar.fillAmount = currentTemp / maxTemp;

//         if (tempText != null)
//             tempText.text = Mathf.RoundToInt(currentTemp) + "°C";
//     }

//     //---------------------------------------------
//     // Called by StoveController when capturing final result
//     //---------------------------------------------
//     public float GetCurrentTemp()
//     {
//         return currentTemp;
//     }
// }
// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;
// using System.Collections;

// public class TemperatureUI : MonoBehaviour
// {
//     [Header("UI Elements")]
//     public GameObject visualGroup;          // A parent object that contains TempText + TempBar
//     public Image tempBar;
//     public TextMeshProUGUI tempText;

//     [Header("Temperature Values")]
//     public float currentTemp = 25f;
//     public float maxTemp = 100f;
//     public float heatRate = 5f;
//     public float coolRate = 2f;

//     [Header("Delays")]
//     public float heatDelay = 0.5f;          // wait before temperature rises
//     public float coolDelay = 2.0f;          // wait before temperature cools

//     private bool isHeating = false;
//     private Coroutine tempRoutine;

//     void Awake()
//     {
//         // IMPORTANT: TemperatureUI object MUST be active in hierarchy.
//         // We disable ONLY the visual group (so coroutines can run).
//         if (visualGroup != null)
//             visualGroup.SetActive(false);
//     }

//     //---------------------------------------------
//     // CALLED BY StoveBurner — turns heating ON/OFF
//     //---------------------------------------------
//     public void SetHeating(bool heating)
//     {
//         if (tempRoutine != null)
//             StopCoroutine(tempRoutine);

//         tempRoutine = StartCoroutine(TemperatureRoutine(heating));
//     }

//     //---------------------------------------------
//     // CALLED BY StoveBurner — shows UI when kettle is placed
//     //---------------------------------------------
//     public void ShowUI()
//     {
//         if (visualGroup != null)
//             visualGroup.SetActive(true);
//     }

//     //---------------------------------------------
//     // CALLED BY StoveBurner — hides UI when kettle removed
//     //---------------------------------------------
//     public void HideUI()
//     {
//         if (visualGroup != null)
//             visualGroup.SetActive(false);
//     }

//     //---------------------------------------------
//     // MAIN HEATING / COOLING COROUTINE
//     //---------------------------------------------
//     private IEnumerator TemperatureRoutine(bool heating)
//     {
//         if (heating)
//         {
//             // Delay before heating begins
//             yield return new WaitForSeconds(heatDelay);

//             while (heating)
//             {
//                 currentTemp += heatRate * Time.deltaTime;
//                 currentTemp = Mathf.Clamp(currentTemp, 25f, maxTemp);
//                 UpdateUI();

//                 yield return null;
//                 heating = isHeating;  // updated externally
//             }
//         }
//         else
//         {
//             // Delay before cooling begins
//             yield return new WaitForSeconds(coolDelay);

//             while (!heating)
//             {
//                 currentTemp -= coolRate * Time.deltaTime;
//                 currentTemp = Mathf.Clamp(currentTemp, 25f, maxTemp);
//                 UpdateUI();

//                 yield return null;
//             }
//         }
//     }

//     //---------------------------------------------
//     // UI Update helper
//     //---------------------------------------------
//     private void UpdateUI()
//     {
//         if (tempBar != null)
//             tempBar.fillAmount = currentTemp / maxTemp;

//         if (tempText != null)
//             tempText.text = Mathf.RoundToInt(currentTemp) + "°C";
//     }

//     //---------------------------------------------
//     // Called by StoveController when capturing final result
//     //---------------------------------------------
//     public float GetCurrentTemp()
//     {
//         return currentTemp;
//     }
// }

using UnityEngine;
using UnityEngine.UI;
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
    public float heatRate = 8f;       // how fast it heats
    public float coolRate = 4f;       // how fast it cools

    [Header("Delays")]
    public float heatDelay = 0.6f;    // wait before heating starts
    public float coolDelay = 1.2f;    // wait before cooling starts

    private bool isHeatingRequested = false;
    private bool isCoolingRequested = false;

    private bool isHeating = false;
    private bool isCooling = false;

    void Start()
    {
        // The UI object itself starts OFF
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (tempBar == null || tempText == null)
            return;

        // Heating
        if (isHeating)
        {
            currentTemp += heatRate * Time.deltaTime;
            currentTemp = Mathf.Clamp(currentTemp, 25f, maxTemp);
        }

        // Cooling
        if (isCooling)
        {
            currentTemp -= coolRate * Time.deltaTime;
            currentTemp = Mathf.Clamp(currentTemp, 25f, maxTemp);
        }

        // Update UI
        tempBar.fillAmount = currentTemp / maxTemp;
        tempText.text = Mathf.RoundToInt(currentTemp) + "°C";
    }

    // -------------------------------
    // PUBLIC API CALLED BY BURNER
    // -------------------------------
    public void ShowUI()
    {
        // enable full UI container
        gameObject.SetActive(true);
    }

    public void HideUI()
    {
        isHeating = false;
        isCooling = false;
        isHeatingRequested = false;
        isCoolingRequested = false;

        // hide full UI
        gameObject.SetActive(false);
    }

    public void RequestHeating()
    {
        // Stop cooling
        isCooling = false;
        isCoolingRequested = false;

        if (!isHeatingRequested)
            StartCoroutine(HeatAfterDelay());
    }

    public void RequestCooling()
    {
        // Stop heating
        isHeating = false;
        isHeatingRequested = false;

        if (!isCoolingRequested)
            StartCoroutine(CoolAfterDelay());
    }

    // -------------------------------
    // COROUTINES
    // -------------------------------
    private System.Collections.IEnumerator HeatAfterDelay()
    {
        isHeatingRequested = true;
        yield return new WaitForSeconds(heatDelay);
        isHeatingRequested = false;
        isHeating = true;
    }

    private System.Collections.IEnumerator CoolAfterDelay()
    {
        isCoolingRequested = true;
        yield return new WaitForSeconds(coolDelay);
        isCoolingRequested = false;
        isCooling = true;
    }

    public float GetCurrentTemp()
    {
        return currentTemp;
    }
}

