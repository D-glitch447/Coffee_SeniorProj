using UnityEngine;
using UnityEngine.UI;

public class AdvicePanelController : MonoBehaviour
{
    
    [Header("References")]
    public GameObject advicePanel;
    public GameObject ScorePanel;
    public GameObject buttonCanvas;
    public GameObject continueButton; // optional

    public void ShowAdvice()
    {
        if (advicePanel != null)
            advicePanel.SetActive(true);
            buttonCanvas.SetActive(true);
            ScorePanel.SetActive(false);


        // Optional: hide the continue button after click
        if (continueButton != null)
            continueButton.SetActive(false);
    }
}
