using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
public class SinkClickable : MonoBehaviour
{
    public AudioClip FootstepsClip;

    private void OnMouseDown()
    {
        if (TutorialManager.InputLocked) return;

        if (CoffeeRuntime.Instance.prepRoomState == PrepRoomState.FirstTime)
        {
            CoffeeRuntime.Instance.prepRoomState = PrepRoomState.AfterSink;

            StartCoroutine(
                FadeController.Instance.FadeToSceneAfterAudio(
                    "SinkCloseUp",
                    FootstepsClip,
                    1f
                )
            );
        }
    }

}
