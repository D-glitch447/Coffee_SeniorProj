using UnityEngine;

public class ScalingTutorialLoader : MonoBehaviour
{
    public DialogueData introDialogue;
    public Vector2 dialogueSize = new Vector2(1600, 450);

    public Vector2 dialoguePosition = new Vector2(0,-350);

    void Start()
    {
        var box = TutorialManager.Instance.dialogueBox;
        RectTransform rt = box.GetComponent<RectTransform>();
        rt.sizeDelta = dialogueSize;
        rt.anchoredPosition = dialoguePosition;
        // Hide hint until dialogue finishes
        HintController.Instance.HideHint();

        StartCoroutine(FadeController.Instance.FadeOutWithCallback(() =>
        {
            TutorialManager.Instance.StartDialogue(introDialogue.messages);
        }));
    }
}

