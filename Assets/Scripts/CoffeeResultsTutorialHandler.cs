using UnityEngine;

public class CoffeeResultsTutorialHandler : MonoBehaviour
{

    [Header("Dialogue")]
    public DialogueData introDialogue;

    [Header("Dialogue Box Layout")]
    public Vector2 dialogueSize = new Vector2(1500, 450);
    public Vector2 dialoguePosition = new Vector2(0, -350);

    private void Start()
    {
        // Resize & reposition the dialogue box for this scene
        var box = TutorialManager.Instance.dialogueBox;
        RectTransform rt = box.GetComponent<RectTransform>();
        rt.sizeDelta = dialogueSize;
        rt.anchoredPosition = dialoguePosition;

        // Trigger dialogue AFTER fade-out (same pattern as your other scenes)
        StartCoroutine(FadeController.Instance.FadeOutWithCallback(() =>
        {
            ShowIntroDialogue();
        }));
    }

    private void ShowIntroDialogue()
    {
        if (introDialogue == null)
        {
            Debug.LogWarning("[CoffeeResultsTutorialLoader] Intro dialogue not assigned.");
            return;
        }

        TutorialManager.Instance.StartDialogue(introDialogue.messages);
    }

}
