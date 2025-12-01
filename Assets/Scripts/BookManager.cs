using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

[System.Serializable]
public class RecipePageLink
{
    public string recipeName;   // e.g. "blackcoffee"
    public GameObject page;     // e.g. BlackCoffee_Page in the scene
}

public class BookManager : MonoBehaviour
{
    [Header("Recipe Data")]
    public CoffeeRecipeDatabase1 recipeDatabase;
    public CoffeeBeanRecipe activeRecipe;

    [Header("Recipe Pages (Scene Objects)")]
    public RecipePageLink[] recipePages;  // Links recipeName → Page GameObject

    [Header("Table of Contents Pages")]
    public GameObject leftPageTOC;
    public GameObject rightPageTOC;

    [Header("Fade Transition")]
    public FadeController fadeController;

    private GameObject[] currentTOCPages;

    // Normalize string so that "Black Coffee" == "blackcoffee"
    private string NormalizeKey(string s)
    {
        return string.IsNullOrWhiteSpace(s)
            ? ""
            : s.Trim().ToLower().Replace(" ", "");
    }

    void Start()
    {
        currentTOCPages = new GameObject[] { leftPageTOC, rightPageTOC };

        // Hide all recipe pages on start
        foreach (var link in recipePages)
        {
            if (link != null && link.page != null)
                link.page.SetActive(false);
        }

        // Show the Table of Contents pages
        SetPageVisibility(leftPageTOC, true);
        SetPageVisibility(rightPageTOC, true);

        Debug.Log("[BookManager] Start complete.");
    }

    public void GoToRecipe(string recipeName)
    {
        string key = NormalizeKey(recipeName);
        Debug.Log($"[BookManager] GoToRecipe called with '{recipeName}', normalized = '{key}'");

        // 1. Load recipe data
        activeRecipe = recipeDatabase.GetRecipe(recipeName);

        if (activeRecipe == null)
        {
            Debug.LogError($"[BookManager] No recipe data found for '{recipeName}'. " +
                           $"Make sure the ScriptableObject's recipeName matches.");
            return;
        }

        Debug.Log($"[BookManager] Found recipe data: '{activeRecipe.recipeName}'");

        // 2. Find the matching page in recipePages[]
        GameObject targetPage = null;

        foreach (var link in recipePages)
        {
            if (link == null) continue;

            string linkKey = NormalizeKey(link.recipeName);

            Debug.Log($"[BookManager] Checking page link: '{link.recipeName}' → normalized '{linkKey}'");

            if (linkKey == key)
            {
                targetPage = link.page;
                break;
            }
        }

        if (targetPage == null)
        {
            Debug.LogError($"[BookManager] No page GameObject found for recipe '{recipeName}'. " +
                           "Check BookManager.recipePages in the Inspector.");
            return;
        }

        // Page found — switch to it
        StartCoroutine(SwitchToRecipe(targetPage));
    }

    private IEnumerator SwitchToRecipe(GameObject newPage)
    {
        yield return fadeController.FadeIn();

        // Hide TOC pages
        foreach (GameObject tocPage in currentTOCPages)
            SetPageVisibility(tocPage, false);

        // Hide all recipe pages
        foreach (var link in recipePages)
        {
            if (link.page != null)
                link.page.SetActive(false);
        }

        // Show new page
        SetPageVisibility(newPage, true);

        yield return fadeController.FadeOut();
    }

    public void GoBackToTOC()
    {
        // Look through all recipe pages to find the one currently active
        foreach (var link in recipePages)
        {
            if (link.page != null && link.page.activeSelf)
            {
                StartCoroutine(SwitchBackToTOC(link.page));
                return;
            }
        }

        Debug.LogWarning("GoBackToTOC called but no active recipe page was found.");
    }


    private IEnumerator SwitchBackToTOC(GameObject currentPageToHide)
    {
        yield return fadeController.FadeIn();

        // Hide the recipe page
        currentPageToHide.SetActive(false);

        // Show TOC pages
        SetPageVisibility(leftPageTOC, true);
        SetPageVisibility(rightPageTOC, true);

        yield return fadeController.FadeOut();
    }

    private void SetPageVisibility(GameObject page, bool isVisible)
    {
        if (page != null)
            page.SetActive(isVisible);
    }

    // public void StartRecipe()
    // {
    //     //Save the active recipe globally
    //     CoffeeRuntime.Instance.activeRecipe = activeRecipe;

    //     Debug.Log("StartRecipe called. activeRecipe = " + activeRecipe);

    //     //Persist runtime between scenes
    //     DontDestroyOnLoad(CoffeeRuntime.Instance.gameObject);

    //     Debug.Log("Recipe Started with: " + activeRecipe.recipeName);
    // }
   public void StartRecipe()
    {

        // 1. Validate that a recipe is selected
        if (activeRecipe == null)
        {
            Debug.LogError("[BookManager] ERROR: No recipe selected");
            return;
        }

        // 2. Validate that CoffeeRuntime singleton exists
        if (CoffeeRuntime.Instance == null)
        {
            Debug.LogError("[BookManager] ERROR: CoffeeRuntime.Instance is NULL. " +
                        "Add CoffeeRUntime to the Kitchen Scene");
            return;
        }

        // 3. Store the selected recipe into the runtime (so next scenes can read it)
        CoffeeRuntime.Instance.activeRecipe = activeRecipe;

        // 4. Keep the runtime alive between scenes
        DontDestroyOnLoad(CoffeeRuntime.Instance.gameObject);

        // 5. Debug confirmation
        Debug.Log("[BookManager] Recipe STARTED: " + activeRecipe.recipeName);
    }



}
