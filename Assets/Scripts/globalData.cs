using UnityEngine;
using UnityEngine.SceneManagement;

public class globalData : MonoBehaviour
{
    public static globalData Instance;

    // your persistent variable(s)
    public bool isKitchenUnlocked = false;

    private void Awake()
    {
        // If there's already an instance, destroy this one
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Make this the instance
        Instance = this;

        // Make sure it persists between scenes
        DontDestroyOnLoad(gameObject);
    }

    public void changeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}