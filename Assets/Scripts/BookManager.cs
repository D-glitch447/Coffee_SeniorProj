using UnityEngine;
using System.Collections;

public class BookManager : MonoBehaviour
{
    [Header("Table of Contents Pages")]
    // These are the containers for the TOC buttons, which is your current view
    public GameObject leftPageTOC;
    public GameObject rightPageTOC;

    [Header("Fade Transition")]
    public FadeController fadeController;  // Add this field
    
    [Header("Target Recipe Pages")]
    // The individual recipe pages you want to swap to
    public GameObject blackCoffeeRecipePage; 
    // You'll need one of these for every final recipe page (Latte, Americano, etc.)

    // We will treat LeftPageTOC and RightPageTOC as the "currentPage"
    // Since they are visible when the book is at the TOC
    private GameObject[] currentTOCPages; 

    void Start()
    {
        currentTOCPages = new GameObject[] { leftPageTOC, rightPageTOC };
        
        // Ensure all recipe pages are hidden on start
        SetPageVisibility(blackCoffeeRecipePage, false);

        // Ensure TOC pages (the buttons) are visible on start
        SetPageVisibility(leftPageTOC, true);
        SetPageVisibility(rightPageTOC, true);
    }

    // Public method that the Black Coffee button will call
    public void GoToRecipe(string recipeName)
    {
        // For this example, we only handle the Black Coffee page for simplicity
        if (recipeName.ToLower() == "blackcoffee")
        {
            StartCoroutine(SwitchToRecipe(blackCoffeeRecipePage));
        }
        else
        {
            Debug.LogError("Recipe not yet implemented: " + recipeName);
        }
    }

   private IEnumerator SwitchToRecipe(GameObject newPage)
{
    // Fade in
    yield return fadeController.FadeIn();

        // Hide the current TOC pages
        foreach (GameObject tocPage in currentTOCPages)
        {
            SetPageVisibility(tocPage, false);
        }
    
    // Show the new recipe page
    SetPageVisibility(newPage, true);

    // 5. Fade out
    yield return fadeController.FadeOut();
    }

    //Function for the "Back" button to call
    public void GoBackToTOC()
    {
        //1. FInd the current visible recipe page
        if (blackCoffeeRecipePage.activeSelf)
        {
            StartCoroutine(SwitchBackToTOC(blackCoffeeRecipePage));
        }
        //Add more 'else if' later
    }

    private IEnumerator SwitchBackToTOC(GameObject currentPageToHide)
{
    // Fade in before transition
    yield return fadeController.FadeIn();

    SetPageVisibility(currentPageToHide, false);


    SetPageVisibility(leftPageTOC, true);
    SetPageVisibility(rightPageTOC, true);

    // Fade out back to normal
    yield return fadeController.FadeOut();
    }

    private void SetPageVisibility(GameObject page, bool isVisible)
    {
        page.SetActive(isVisible);
    }
    
    //Function for the Begin button to call
    public void StartRecipe()
    {
        Debug.Log("Recipe Started! Time to make some coffee! ");

    }
}