using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class sceneChanger : MonoBehaviour
{
    public FadeController fade;      // Assign in Inspector
    public string nextSceneName;     // Type your next scene

    public AudioSource audioSource;

    public AudioClip OpeningBookClip;

    public void OnBookClicked()
    {
        if(TutorialManager.InputLocked) return;
        StartCoroutine(FadeSequence());
    }
    private IEnumerator FadeSequence()
    {
        yield return fade.FadeIn();   // fade to black

        // Play book closing sound
        audioSource.PlayOneShot(OpeningBookClip, 3.5f);

        // ⏱️ WAIT for the sound to finish
        yield return new WaitForSeconds(OpeningBookClip.length);
        SceneManager.LoadScene(nextSceneName);
    }
}
