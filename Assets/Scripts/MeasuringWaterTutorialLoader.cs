using UnityEngine;

public class MeasuringWaterTutorialLoader : MonoBehaviour
{
    public DialogueData introDialogue;
    public DialogueData afterSinkDialogue;
    public Vector2 dialogueSize = new Vector2(1500, 450);
    public Vector2 dialoguePosition = new Vector2(0, -350);

    void Start()
    {
        // Resize & reposition the dialogue box for this scene
        var box = TutorialManager.Instance.dialogueBox;
        RectTransform rt = box.GetComponent<RectTransform>();
        rt.sizeDelta = dialogueSize;
        rt.anchoredPosition = dialoguePosition;

        // Trigger dialogue AFTER fade-out
        StartCoroutine(FadeController.Instance.FadeOutWithCallback(() =>
        {
            ShowCorrectDialogue();
            // TutorialManager.Instance.StartDialogue(introDialogue.messages);
        }));
    }

    void ShowCorrectDialogue()
    {
        var state = CoffeeRuntime.Instance.prepRoomState;

        switch(state)
        {
            case PrepRoomState.FirstTime:
                TutorialManager.Instance.StartDialogue(introDialogue.messages);
                break;
            case PrepRoomState.AfterSink:
                TutorialManager.Instance.StartDialogue(afterSinkDialogue.messages);
                break;
        }
    }
}
