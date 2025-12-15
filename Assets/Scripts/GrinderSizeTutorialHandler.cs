using UnityEngine;

public class GrinderSizeTutorialHandler : MonoBehaviour
{
    public DialogueData fineDialogue;
    public DialogueData mediumDialogue;
    public DialogueData coarseDialogue;

    private bool hasShown = false;

    public void ShowDialogueForCurrentGrind()
    {
        if (hasShown) return;
        if (TutorialManager.InputLocked) return;

        hasShown = true;

        int grindIndex = CoffeeRuntime.Instance.playerSelectedGrindIndex;

        if (grindIndex <= 2)
        {
            TutorialManager.Instance.StartDialogue(fineDialogue.messages);
        }
        else if (grindIndex <= 4)
        {
            TutorialManager.Instance.StartDialogue(mediumDialogue.messages);
        }
        else
        {
            TutorialManager.Instance.StartDialogue(coarseDialogue.messages);
        }
    }
}
