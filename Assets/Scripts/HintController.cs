using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class HintController : MonoBehaviour
{
    public static HintController Instance;

    [Header("Hint UI")]
    public GameObject hintBox;
    public TMP_Text hintText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // ---------------------------------------------------------
    // SCENE-BASED HINTS
    // ---------------------------------------------------------
    public void ShowSceneHint()
    {
        var r = CoffeeRuntime.Instance.activeRecipe;
        if (r == null)
        {
            hintText.text = "";
            hintBox.SetActive(false);
            return;
        }

        string scene = SceneManager.GetActiveScene().name;

        switch (scene)
        {
            case "Scaling_Beans":
                hintText.text = $"Beans Needed: {r.coffeeWeightGrams}g";
                break;

            case "SinkCloseUp":
                hintText.text = $"Water Needed: {r.waterWeightGrams}g";
                break;

            case "StoveCloseUp":
                hintText.text = $"Target Temp: {r.waterTemperatureCelsius}°C";
                break;

            case "grindScene":
                string grindName = GetGrindSizeName((int)r.idealGrindSize);
                hintText.text = $"Target Grind: {grindName}";
                break;

            default:
                hintBox.SetActive(false);
                return;
        }

        hintBox.SetActive(true);
    }

    // Call this manually if you want a warning or extra text
    public void ShowCustomHint(string message)
    {
        hintText.text = message;
        hintBox.SetActive(true);
    }

    public void HideHint()
    {
        hintBox.SetActive(false);
    }
    private string GetGrindSizeName(int grindIndex)
    {
        switch (grindIndex)
        {
            case 1: return "Extra Fine";
            case 2: return "Fine";
            case 3: return "Medium-Fine";
            case 4: return "Medium";
            case 5: return "Medium-Coarse";
            case 6: return "Coarse";
            case 7: return "Extra Coarse";
            case 8: return "Very Coarse";
            default: return "Unknown";
        }
    }

}

