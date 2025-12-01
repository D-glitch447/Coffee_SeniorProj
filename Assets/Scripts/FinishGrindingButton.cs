using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class FinishGrindingButton : MonoBehaviour
{
    public FadeController fade;
    public float grindSize = 0f; // replace with your value later

    public void OnFinishGrinding()
    {
        StartCoroutine(FinishSequence());
    }

    private IEnumerator FinishSequence()
    {
        if (fade != null)
            yield return fade.FadeIn();

        // store grind size
        CoffeeRuntime.Instance.playerGrindSize = grindSize;

        // mark grind completed
        CoffeeRuntime.Instance.hasCompletedGrind = true;

        Debug.Log("Grinding finished. Grind Size = " + grindSize);

        SceneManager.LoadScene("Kitchen");
    }
}
