  using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class finalBeanWeight : MonoBehaviour
{

    public FadeController fade;

    public void OnFinishClicked()
    {
        StartCoroutine(FinishSequence());
    }

    private IEnumerator FinishSequence()
    {
        // 1. Fade to black
        if (fade != null)
            yield return fade.FadeIn();

        // 2. Store final weight
        float finalWeight = ScaleController.Instance.GetCurrentWeight();
        CoffeeRuntime.Instance.playerFinalWeight = finalWeight;

        // 3. Mark scale step as completed
        CoffeeRuntime.Instance.hasCompletedScale = true;

        Debug.Log("Scale finished. Final Weight = " + finalWeight);

        // 4. Return to kitchen scene
        SceneManager.LoadScene("Kitchen");
    }
}

//   private BookManager bookManager;

//   void Start()
//     {
//         //Find BookManager that was carried over from the recipe book scene
//         bookManager = FindFirstObjectByType<BookManager>();

//         if (bookManager == null)
//         {
//             Debug.LogError("BookManager not found in this scene. Make sure it's set on DontDestroyOnLoad");
//         }
//     }
//   public void OnFinishButtonPressed()
//     {
//         float currentWeight = ScaleController.Instance.GetCurrentWeight();

//         CoffeeRuntime.Instance.playerFinalWeight = currentWeight;
//         Debug.Log("Player Weight: " + currentWeight);

//         //Compare to ideal
//         float ideal = CoffeeRuntime.Instance.activeRecipe.coffeeWeightGrams;
//         float diff = Mathf.Abs(currentWeight - ideal);

//         Debug.Log("Ideal Weight: " + ideal);
//         Debug.Log("Difference: " + diff);
//     }
// }
