using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class ScaleButtonHandler : MonoBehaviour
{
    public FadeController fade;

    // Optional: references to other buttons to visually lock them
    public Button grinderButton;
    public Button pourOverButton;
    public Button kettleButton;
    public Button selfScaleButton; // the scale button itself

    public string scaleSceneName = "Scaling_Beans"; // <-- put your exact scene name here

    public AudioSource audioSource;

    public AudioClip FootstepsClip;

    public void OnScaleClicked()
    {
        if(TutorialManager.InputLocked) return;
        StartCoroutine(FadeToScaleScene());
    }

    private IEnumerator FadeToScaleScene()
    {
        // 🔹 Disable all gameplay buttons so you can't spam clicks during fade
        SetButtonsInteractable(false);

        if (fade != null)
        {
            yield return fade.FadeIn();   // fade to black
            // Play book closing sound
            audioSource.PlayOneShot(FootstepsClip);

            // ⏱️ WAIT for the sound to finish
            yield return new WaitForSeconds(FootstepsClip.length);
        }

        SceneManager.LoadScene(scaleSceneName);
    }

    private void SetButtonsInteractable(bool value)
    {
        if (selfScaleButton != null) selfScaleButton.interactable = value;
        if (grinderButton != null)   grinderButton.interactable = value;
        if (pourOverButton != null)  pourOverButton.interactable = value;
        if (kettleButton != null)    kettleButton.interactable = value;
    }
}
