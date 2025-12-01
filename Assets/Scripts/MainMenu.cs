using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    public FadeController fade;
    public string kitchenSceneName = "Kitchen";

    // PLAY BUTTON
    public void OnPlayClicked()
    {
        StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        if (fade != null)
            yield return fade.FadeIn();

        SceneManager.LoadScene(kitchenSceneName);
    }

    // QUIT BUTTON
    public void OnQuitClicked()
    {
        Debug.Log("Quit button pressed.");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; 
#else
        Application.Quit();
#endif
    }
}
