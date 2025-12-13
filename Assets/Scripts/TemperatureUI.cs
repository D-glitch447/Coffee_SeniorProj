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
    public float heatRate = 2f;       // how fast it heats

    [Header("Delays")]
    public float heatDelay = 0.6f;    // wait before heating starts

    private bool isHeatingRequested = false;

    private bool isHeating = false;

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


        // Update UI
        tempBar.fillAmount = currentTemp / maxTemp;
        tempText.text = Mathf.RoundToInt(currentTemp) + "°C";
    }
    public void ShowUI()
    {
        // enable full UI container
        gameObject.SetActive(true);
        UpdateUIImmediate();
    }

    public void HideUI()
    {
        isHeating = false;
        isHeatingRequested = false;
        // hide full UI
        gameObject.SetActive(false);
    }

    public void RequestHeating()
    {

        if (!isHeatingRequested)
            StartCoroutine(HeatAfterDelay());
    }

    private IEnumerator HeatAfterDelay()
    {
        isHeatingRequested = true;
        yield return new WaitForSeconds(heatDelay);
        isHeatingRequested = false;
        isHeating = true;
    }
     public void StopHeating()
    {
        isHeating = false;
    }

    public float GetCurrentTemp()
    {
        return currentTemp;
    }
    private void UpdateUIImmediate()
    {
        tempBar.fillAmount = currentTemp / maxTemp;
        tempText.text = Mathf.RoundToInt(currentTemp) + "°C";
    }
}

