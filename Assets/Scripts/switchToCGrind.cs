using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    // 1. Create a slot to type the scene name in the Inspector
    public string sceneToLoad; 

    // 2. Unity automatically calls this when you click an object with a Collider
    private void OnMouseDown()
    {
        // Check if we actually typed a name to avoid errors
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("You forgot to type the scene name in the Inspector!");
        }
    }
}