using UnityEngine;

public class FaucetToggle : MonoBehaviour
{
    public GameObject waterStream;

    public GameObject handleUpSprite;
    public GameObject handleDownSprite;

    private bool isOn = false;

    public bool IsOn => isOn;

    void Start()
    {
        // Start off with water off
        isOn = false;
        handleUpSprite.SetActive(true);
        handleDownSprite.SetActive(false);
        waterStream.SetActive(false);
    }

    private void OnMouseDown()
    {
        ToggleFaucet();
    }

    void ToggleFaucet()
    {
        isOn = !isOn;

        // Swap sprites
        handleUpSprite.SetActive(!isOn);
        handleDownSprite.SetActive(isOn);

        // Toggle water
        waterStream.SetActive(isOn);

        Debug.Log("Faucet Toggled. Now ON? " + isOn);
    }
}
