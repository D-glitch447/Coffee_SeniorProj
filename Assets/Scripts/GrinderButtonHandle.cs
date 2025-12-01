using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class GrinderButtonHandler : MonoBehaviour
{
    public FadeController fade;

    public Button scaleButton;
    public Button grinderButton;
    public Button pourOverButton;
    public Button kettleButton;

    public string grindingSceneName = "grindScene"; 

    public void OnGrinderClicked()
    {
        StartCoroutine(GoToGrinder());
    }

    private IEnumerator GoToGrinder()
    {
        // disable clicks
        scaleButton.interactable = false;
        grinderButton.interactable = false;
        pourOverButton.interactable = false;
        kettleButton.interactable = false;

        if (fade != null)
            yield return fade.FadeIn();

        SceneManager.LoadScene(grindingSceneName);
    }
}

