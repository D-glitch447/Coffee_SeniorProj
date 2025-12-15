using UnityEngine;

public class GrinderIntroTutorialHandler : MonoBehaviour
{
    public DialogueData introDialogue;

    public Vector2 dialogueSize = new Vector2(1500, 450);
    public Vector2 dialoguePosition = new Vector2(0, -350);

    private void Start()
    {
        // Resize & reposition dialogue box
        var box = TutorialManager.Instance.dialogueBox;
        RectTransform rt = box.GetComponent<RectTransform>();
        rt.sizeDelta = dialogueSize;
        rt.anchoredPosition = dialoguePosition;

        // Trigger after fade
        StartCoroutine(FadeController.Instance.FadeOutWithCallback(() =>
        {
            TutorialManager.Instance.StartDialogue(introDialogue.messages);
        }));
    }
}
