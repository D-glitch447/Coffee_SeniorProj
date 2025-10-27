using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1); // Load the next scene (feel free to change though)
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
