using UnityEngine;
using UnityEngine.SceneManagement;

public class ScaleLevelController : MonoBehaviour
{
    public void OnScaleLevelComplete()
    {
        if (CoffeeRuntime.Instance != null)
        {
            CoffeeRuntime.Instance.hasCompletedScale = true;
            Debug.Log("Scale level completed. Unlocking other tools.");
        }

        // Go back to Kitchen scene
        SceneManager.LoadScene("Kitchen");
    }
}
