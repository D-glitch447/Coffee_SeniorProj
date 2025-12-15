using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public DialogueData introDialogue;
    public Vector2 dialogueSize = new Vector2(1500, 450);
    public Vector2 dialoguePosition = new Vector2(0, -350);

    private void Start()
    {
        // Adjust dialogue UI
        var box = TutorialManager.Instance.dialogueBox;
        RectTransform rt = box.GetComponent<RectTransform>();
        rt.sizeDelta = dialogueSize;
        rt.anchoredPosition = dialoguePosition;

        // Play dialogue after fade
        StartCoroutine(FadeController.Instance.FadeOutWithCallback(() =>
        {
            TutorialManager.Instance.StartDialogue(introDialogue.messages);
        }));
    }

}
