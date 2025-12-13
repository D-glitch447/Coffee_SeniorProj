using UnityEngine;

public class FaucetToggle : MonoBehaviour
{
    public GameObject waterStream;

    public GameObject handleUpSprite;
    public GameObject handleDownSprite;

    public AudioSource audioSource;

    private bool isOn = false;

    public bool IsOn => isOn;

    void Start()
    {
        TurnOff();
    }

    private void OnMouseDown()
    {
        if(TutorialManager.InputLocked) return;
        ToggleFaucet();
    }

    void ToggleFaucet()
    {
        if(isOn) 
            TurnOff();
        else 
            TurnOn();
    }

    public void TurnOn()
    {
        if (isOn) return;

        isOn = true;

        // Visuals
        if (handleUpSprite != null) handleUpSprite.SetActive(false);
        if (handleDownSprite != null) handleDownSprite.SetActive(true);
        if (waterStream != null) waterStream.SetActive(true);

        // Audio
        if (audioSource != null && !audioSource.isPlaying)
            audioSource.Play();

        Debug.Log("[FaucetToggle] Faucet ON");
    }
    public void TurnOff()
    {
        if (!isOn) return;

        isOn = false;

        // Visuals
        if (handleUpSprite != null) handleUpSprite.SetActive(true);
        if (handleDownSprite != null) handleDownSprite.SetActive(false);
        if (waterStream != null) waterStream.SetActive(false);

        // Audio
        if (audioSource != null)
            audioSource.Stop();

        Debug.Log("[FaucetToggle] Faucet OFF");
    }
}
