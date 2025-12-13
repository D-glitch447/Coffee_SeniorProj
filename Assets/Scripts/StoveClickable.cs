using UnityEngine;
using System.Collections;

public class StoveClickable : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip FootstepsClip;
    private void OnMouseDown()
    {
        // CoffeeRuntime must exist
        if (CoffeeRuntime.Instance == null)
        {
            Debug.LogError("CoffeeRuntime is missing!");
            return;
        }

        // Require water step to be finished
        if (!CoffeeRuntime.Instance.hasCompletedMeasuringWater)
        {
            Debug.Log("Stove is locked. Measure the water first!");
            return;
        }

        // Ignore clicks during dialogue
        if (TutorialManager.InputLocked) return;

        // Only allow stove click AFTER sink is done
        if (CoffeeRuntime.Instance.prepRoomState == PrepRoomState.AfterSink)
        {
            StartCoroutine(
                FadeController.Instance.FadeToSceneAfterAudio(
                    "StoveCloseUp",
                    FootstepsClip,
                    1f
                )
            );
        }
    }
}