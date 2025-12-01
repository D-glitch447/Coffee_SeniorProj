using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BeginButton : MonoBehaviour
{
    public FadeController fade;
    public BookManager bookManager;
    public string kitchenSceneName = "Kitchen";  // <-- change to your real kitchen scene name

    public void OnBeginClicked()
    {
        Debug.Log("[BeginButton] OnBeginClicked fired.");
        StartCoroutine(BeginSequence());
    }

    private IEnumerator BeginSequence()
    {
       if(bookManager.activeRecipe == null)
        {
            Debug.Log("Cannot BEGIN - no recipe selected");
            yield break;
        }

        //Fade to black
        yield return fade.FadeIn();

        //Save recipe into CoffeeRuntime
        bookManager.StartRecipe();

        //Now load your kitchen scene
        SceneManager.LoadScene(kitchenSceneName);
    }
}
