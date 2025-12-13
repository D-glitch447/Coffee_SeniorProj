using UnityEngine;

[CreateAssetMenu(fileName = "DialogueData", menuName = "Coffee/Dialogue")]
public class DialogueData : ScriptableObject
{
    [TextArea(2,5)]
    public string[] messages;
}
