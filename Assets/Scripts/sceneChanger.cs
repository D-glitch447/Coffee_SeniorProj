using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneChanger : MonoBehaviour
{
   public void changeScene(string sceneToLoad)
    {
        Debug.Log("Changing scene to: " + sceneToLoad);
        SceneManager.LoadScene(sceneToLoad);
    }
}