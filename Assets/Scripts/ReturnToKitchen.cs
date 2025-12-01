using UnityEngine;
using UnityEngine.SceneManagement;
public class ReturnToKitchen : MonoBehaviour
{
    public void GoBack()
    {
        SceneManager.LoadScene("Measuring_Water");
        Debug.Log("Alright let's go back and heat this water up");
    }
}
