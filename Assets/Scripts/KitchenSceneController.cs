using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class KitchenSceneController : MonoBehaviour
{
    [Header("Intro Mode Objects")]
    public GameObject introBackground;    // backgroundKitchenBook_0
    public GameObject recipeBookButton;   // RecipeBook

    [Header("Gameplay Mode Objects")]
    public GameObject gameplayBackground; // backgroundKitchenTools_0
    public GameObject gameplayButtons;    // kitchenGroup/Canvas

    [Header("Gameplay Buttons")]
    public Button scaleButton;
    public Button grinderButton;
    public Button pourOverButton;
    public Button kettleButton;

    public FadeController fade;

    private void Start()
    {
        StartCoroutine(fade.FadeOut());   // fade-in when scene loads

        bool hasRuntime = CoffeeRuntime.Instance != null;
        bool hasRecipe  = hasRuntime && CoffeeRuntime.Instance.activeRecipe != null;
        bool finishedScale = hasRuntime && CoffeeRuntime.Instance.hasCompletedScale;

        if (!hasRecipe)
        {
            CoffeeRuntime.Instance.kitchenState = KitchenState.FirstTime;
        }
        else
        {
            if (!CoffeeRuntime.Instance.hasCompletedScale)
                CoffeeRuntime.Instance.kitchenState = KitchenState.AfterRecipeSelected;
            else if (!CoffeeRuntime.Instance.hasCompletedGrind)
                CoffeeRuntime.Instance.kitchenState = KitchenState.AfterScaling;
            else if (!CoffeeRuntime.Instance.hasCompletedBrewing)
                CoffeeRuntime.Instance.kitchenState = KitchenState.AfterGrinding;
            else
                CoffeeRuntime.Instance.kitchenState = KitchenState.AfterBrewing;
        }

        
        if (hasRecipe)
        {
            // GAMEPLAY MODE (we came back from RecipeBook Begin)
            introBackground.SetActive(false);
            recipeBookButton.SetActive(false);

            gameplayBackground.SetActive(true);
            gameplayButtons.SetActive(true);

            // 🔹 Button logic
            SetupGameplayButtons();
        }
        else
        {
            // INTRO MODE (first time in Kitchen)
            introBackground.SetActive(true);
            recipeBookButton.SetActive(true);

            gameplayBackground.SetActive(false);
            gameplayButtons.SetActive(false);
        }
    }

    private void SetupGameplayButtons()
    {
        bool finishedScale = CoffeeRuntime.Instance.hasCompletedScale;
        bool finishedGrind = CoffeeRuntime.Instance.hasCompletedGrind;

        // Always allow scale after recipe begins
        scaleButton.interactable = true;

        // Grinder unlocks after scale
        grinderButton.interactable = finishedScale;

        // PourOver + Kettle unlock after grinding
        bool unlockNextTools = finishedScale && finishedGrind;

        pourOverButton.interactable = unlockNextTools;
        kettleButton.interactable = unlockNextTools;
    }

}

