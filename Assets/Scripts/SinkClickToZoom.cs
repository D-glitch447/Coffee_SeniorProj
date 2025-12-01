using UnityEngine;
using UnityEngine.SceneManagement;

public class SinkClickToZoom : MonoBehaviour
{
    private void OnMouseDown() 
    {
        //Loads the sink close-up scene
        SceneManager.LoadScene("SinkCloseUp");
        Debug.Log("Clicked sink, time to get some water");
    }
}
