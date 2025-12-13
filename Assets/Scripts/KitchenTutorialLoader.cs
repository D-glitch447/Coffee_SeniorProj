using UnityEngine;

public class KitchenTutorialLoader : MonoBehaviour
{
    public DialogueData firstTimeDialogue;
    public DialogueData afterRecipeDialogue;
    public DialogueData afterScalingDialogue;
    public DialogueData afterGrindingDialogue;
    public DialogueData afterBrewingDialogue;

    void Start()
    {
        // Wait for the fade to complete before showing dialogue
        StartCoroutine(FadeController.Instance.FadeOutWithCallback(ShowKitchenDialogue));
    }

    private void ShowKitchenDialogue()
    {
        Debug.Log("KitchenTutorialLoader: Showing dialogue for state = " +
                  CoffeeRuntime.Instance.kitchenState);

        switch (CoffeeRuntime.Instance.kitchenState)
        {
            case KitchenState.FirstTime:
                TutorialManager.Instance.StartDialogue(firstTimeDialogue.messages);
                break;

            case KitchenState.AfterRecipeSelected:
                TutorialManager.Instance.StartDialogue(afterRecipeDialogue.messages);
                break;

            case KitchenState.AfterScaling:
                TutorialManager.Instance.StartDialogue(afterScalingDialogue.messages);
                break;

            case KitchenState.AfterGrinding:
                TutorialManager.Instance.StartDialogue(afterGrindingDialogue.messages);
                break;

            case KitchenState.AfterBrewing:
                TutorialManager.Instance.StartDialogue(afterBrewingDialogue.messages);
                break;

            default:
                Debug.Log("❗ Kitchen state unhandled by KitchenTutorialLoader.");
                break;
        }
    }
}
