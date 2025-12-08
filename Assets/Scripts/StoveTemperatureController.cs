using UnityEngine;

public class StoveTemperatureController : MonoBehaviour
{
    public TemperatureUI leftTempUI;
    public TemperatureUI rightTempUI;

    // Example: call from a button when player confirms left kettle temp
    public void LogLeftTemperature()
    {
        if (leftTempUI != null)
            Debug.Log("Left burner temp = " + leftTempUI.GetCurrentTemp());
    }

    public void LogRightTemperature()
    {
        if (rightTempUI != null)
            Debug.Log("Right burner temp = " + rightTempUI.GetCurrentTemp());
    }
}
