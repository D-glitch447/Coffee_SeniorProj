using UnityEngine;

public class finalBeanWeight : MonoBehaviour
{
  private BookManager bookManager;

  void Start()
    {
        //Find BookManager that was carried over from the recipe book scene
        bookManager = FindFirstObjectByType<BookManager>();

        if (bookManager == null)
        {
            Debug.LogError("BookManager not found in this scene. Make sure it's set on DontDestroyOnLoad");
        }
    }
  public void OnFinishButtonPressed()
    {
        float currentWeight = ScaleController.Instance.GetCurrentWeight();

        CoffeeRuntime.Instance.playerFinalWeight = currentWeight;
        Debug.Log("Player Weight: " + currentWeight);

        //Compare to ideal
        float ideal = CoffeeRuntime.Instance.activeRecipe.coffeeWeightGrams;
        float diff = Mathf.Abs(currentWeight - ideal);

        Debug.Log("Ideal Weight: " + ideal);
        Debug.Log("Difference: " + diff);
    }
}
