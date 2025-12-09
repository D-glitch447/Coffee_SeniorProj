using UnityEngine;
using UnityEngine.SceneManagement;
public class SinkClickable : MonoBehaviour
{
   private void OnMouseDown() {
    //fade, then load sink close-up scene
     FadeController.Instance.FadeToScene("SinkCloseUp");
   }
}
