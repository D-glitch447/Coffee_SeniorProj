// using NUnit.Framework.Constraints;
// using UnityEngine;

// public class KitchenSceneController : MonoBehaviour
// {
//     public GameObject introBackground;
//     public GameObject recipeBookButton;

//     public GameObject gameplayBackground;
//     public GameObject gameplayButtons;
//     public FadeController fade;


//     private void Start()
//     {
//         //Fade in after scene loads
//         StartCoroutine(fade.FadeOut());

//         //Determine if we're in Intro mode or Gameplay mode
//         if(CoffeeRuntime.Instance != null && CoffeeRuntime.Instance.activeRecipe != null)
//         {
//             //Recipe selected -> show gameplay mode
//             introBackground.SetActive(false);
//             recipeBookButton.SetActive(false);

//             gameplayBackground.SetActive(true);
//             gameplayButtons.SetActive(true);
//         } else
//         {
//             //First time opening kitchen -> intro mode 
//             introBackground.SetActive(true);
//             recipeBookButton.SetActive(true);

//             gameplayBackground.SetActive(false);
//             gameplayBackground.SetActive(false);
//         }
//     }
// }

using UnityEngine;
using UnityEngine.UI;
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

