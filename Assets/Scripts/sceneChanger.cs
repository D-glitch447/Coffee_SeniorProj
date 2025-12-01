using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class sceneChanger : MonoBehaviour
{
    public FadeController fade;      // Assign in Inspector
    public string nextSceneName;     // Type your next scene

    public void OnBookClicked()
    {
        StartCoroutine(FadeSequence());
    }
    private IEnumerator FadeSequence()
    {
        yield return fade.FadeIn();   // fade to black
        SceneManager.LoadScene(nextSceneName);
    }
}
