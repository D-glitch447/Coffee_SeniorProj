using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class PourOverButtonHandler : MonoBehaviour
{
    public FadeController fade;

    [Header("Kitchen Buttons")]
    public Button scaleButton;
    public Button grinderButton;
    public Button pourOverButton;
    public Button kettleButton;

    [Header("Scene Config")]
    public string pourOverSceneName = "pouringScene"; 

    public void OnPourOverClicked()
    {
        StartCoroutine(GoToPourOver());
    }

    private IEnumerator GoToPourOver()
    {
        // 1. Disable all buttons to prevent double-clicks
        if (scaleButton) scaleButton.interactable = false;
        if (grinderButton) grinderButton.interactable = false;
        if (pourOverButton) pourOverButton.interactable = false;
        if (kettleButton) kettleButton.interactable = false;

        // 2. Wait for fade out
        if (fade != null)
            yield return fade.FadeIn();

        // 3. Load the brewing scene
        SceneManager.LoadScene(pourOverSceneName);
    }
}