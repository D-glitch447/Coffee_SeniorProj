using UnityEngine;

public class StoveKnobToggle : MonoBehaviour
{
    [Header("Knob Visuals")]
    public GameObject knobOff;
    public GameObject knobOn;

    [Header("Burner Reference")]
    public StoveBurner burner;

    private bool isOn = false;

    private void Start()
    {
        knobOff.SetActive(true);
        knobOn.SetActive(false);
    }

    private void OnMouseDown()
    {
        if (!burner.hasKettle)
        {
            Debug.Log("Cannot turn on burner — no kettle is on this stove.");
            return;
        }

        isOn = !isOn;

        knobOff.SetActive(!isOn);
        knobOn.SetActive(isOn);

        burner.SetBurnerState(isOn);
    }
}
