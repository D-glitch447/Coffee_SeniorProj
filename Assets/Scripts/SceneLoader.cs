using System;
using UnityEngine;
using UnityEngine.SceneManagement; // This line is crucial!

public class SceneLoader : MonoBehaviour
{
    // This is the function your button will call
    public void LoadScene(string sceneName)
    {
        Debug.Log("Button clicked! Attempting to load: " + sceneName); // Use this!
        SceneManager.LoadScene(sceneName);
        
    }
}