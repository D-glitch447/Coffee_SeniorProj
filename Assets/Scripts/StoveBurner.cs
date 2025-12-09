// using UnityEngine;

// public class StoveBurner : MonoBehaviour
// {
//     public TemperatureUI tempUI;
//     public GameObject fireArt; 
//     public Transform dropPoint;

//     public bool hasKettle = false;
//     private bool burnerOn = false;

//     public void SetKettlePresent(bool value)
//     {
//         hasKettle = value;

//         if (value)
//         {
//             // Turn UI ON immediately
//             tempUI.ShowUI();

//             // Do NOT heat yet — only when knob is turned on
//             if (burnerOn)
//                 tempUI.SetHeating(true);
//         }
//         else
//         {
//             // Kettle gone — UI off
//             tempUI.SetHeating(false);
//             tempUI.HideUI();

//             burnerOn = false;
//             fireArt.SetActive(false);
//         }
//     }
//     public void SetBurnerState(bool on)
//     {
//         burnerOn = on;
//         fireArt.SetActive(on);

//         if (on && hasKettle)
//         {
//             // UI should already be ON before this call
//             tempUI.SetHeating(true);
//         }
//         else
//         {
//             tempUI.SetHeating(false);
//         }
//     }

// }

using UnityEngine;

public class StoveBurner : MonoBehaviour
{
    public TemperatureUI tempUI;
    public GameObject fireArt;
    public Transform dropPoint;

    public bool hasKettle = false;
    private bool burnerOn = false;

    public void SetKettlePresent(bool value)
    {
        hasKettle = value;

        if (value)
        {
            // Show UI immediately
            tempUI.ShowUI();

            // If burner was already on, start heating
            if (burnerOn)
                tempUI.RequestHeating();
        }
        else
        {
            // Kettle removed → stop heating or cooling
            burnerOn = false;
            fireArt.SetActive(false);
            tempUI.HideUI();
        }
    }

    public void SetBurnerState(bool on)
    {
        burnerOn = on;
        fireArt.SetActive(on);

        if (on && hasKettle)
        {
            tempUI.RequestHeating();
        }
        else if (!on && hasKettle)
        {
            tempUI.RequestCooling();
        }
    }
}

