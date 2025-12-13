using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BeginButton : MonoBehaviour
{
    public FadeController fade;
    public BookManager bookManager;
    public string kitchenSceneName = "Kitchen";  

    public AudioSource audioSource;
    public AudioClip BeginRecipeClip;

    public AudioClip BookClosingClip;

    public void OnBeginClicked()
    {
        Debug.Log("[BeginButton] OnBeginClicked fired.");
        audioSource.PlayOneShot(BeginRecipeClip);
        StartCoroutine(BeginSequence());
    }


    private IEnumerator BeginSequence()
    {
        if (bookManager.activeRecipe == null)
        {
            Debug.Log("Cannot BEGIN - no recipe selected");
            yield break;
        }

        // Fade to black
        yield return fade.FadeIn();

        // Play book closing sound
        audioSource.PlayOneShot(BookClosingClip);

        // ⏱️ WAIT for the sound to finish
        yield return new WaitForSeconds(BookClosingClip.length);

        // Save recipe into CoffeeRuntime
        bookManager.StartRecipe();
        CoffeeRuntime.Instance.kitchenState = KitchenState.AfterRecipeSelected;

        // Now load the kitchen
        SceneManager.LoadScene(kitchenSceneName);
    }

}
