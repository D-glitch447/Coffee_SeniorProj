using UnityEngine;
using System.Collections;

public class StoveKnobToggle : MonoBehaviour
{
    [Header("Knob Visuals")]
    public GameObject knobOff;
    public GameObject knobOn;

    public AudioSource KnobSource;
    public AudioClip KnobTurnClip;

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
        if(TutorialManager.InputLocked) return;
        if (!burner.hasKettle)
        {
            Debug.Log("Cannot turn on burner — no kettle is on this stove.");
            return;
        }
        StartCoroutine(ToggleKnobSequence());
    }
    private IEnumerator ToggleKnobSequence()
    {
        //Play knob turn sound FIRST
        if (KnobSource != null && KnobTurnClip != null)
        {
            KnobSource.pitch = Random.Range(0.95f, 1.05f);
            KnobSource.PlayOneShot(KnobTurnClip, 1f);
        }

        // Small delay so sound leads the motion
        yield return new WaitForSeconds(0.10f); // tweak 0.05–0.12

        // Now toggle state
        isOn = !isOn;

        knobOff.SetActive(!isOn);
        knobOn.SetActive(isOn);

        burner.SetBurnerState(isOn);
    }

}

