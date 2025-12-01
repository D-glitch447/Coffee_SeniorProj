using UnityEngine;

public class BookButton : MonoBehaviour
{
    public string nextSceneName = "RecipeBookScene";

    // This will be called from the Button's OnClick event
    public void OnBookClicked()
    {
        if (FadeController.Instance != null)
        {
            FadeController.Instance.FadeToScene(nextSceneName);
        }
        else
        {
            Debug.LogError("FadeController.Instance is NULL! Make sure FadeCanvas exists in the first scene.");
        }
    }
}