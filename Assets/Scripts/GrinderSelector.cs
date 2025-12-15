using UnityEngine;

public class GrinderSelector : MonoBehaviour
{
    [Header("Objects to Turn ON")]
    [SerializeField] private GameObject fullGrinder;
    [SerializeField] private GameObject grinderHandle;
    [SerializeField] private GameObject[] otherObjectsToActivate;

    [Header("Dialogue")]
    [SerializeField] private DialogueData fineDialogue;
    [SerializeField] private DialogueData mediumDialogue;
    [SerializeField] private DialogueData coarseDialogue;

    [Header("Objects to Turn OFF")]
    // Drag the object you want to deactivate here (e.g., the label or arrow)
    [SerializeField] private GameObject objectToDeactivate; 

    private AudioSource audioSource;

    private bool hasShownDialogue = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnMouseDown()
    {
        if (TutorialManager.InputLocked)
            return;

        // --- 1. PLAY SOUND ---
        // We use PlayClipAtPoint so the sound finishes playing even if this object turns off immediately.
        if (audioSource != null && audioSource.clip != null)
        {
            // This creates a temporary audio object at the current position
            AudioSource.PlayClipAtPoint(audioSource.clip, transform.position, audioSource.volume);
        }

        // --- 2. ACTIVATE NEW PARTS ---
        if (fullGrinder != null) fullGrinder.SetActive(true);
        if (grinderHandle != null) grinderHandle.SetActive(true);

        foreach (GameObject obj in otherObjectsToActivate)
        {
            if (obj != null) obj.SetActive(true);
        }

        // --- 3. DEACTIVATE EXTRA OBJECT ---
        if (objectToDeactivate != null)
        {
            objectToDeactivate.SetActive(false);
        }

        // --- 4. DEACTIVATE SELF ---
        // This hides the empty grinder button
        gameObject.SetActive(false);

         // mark grind as intentionally started
        CoffeeRuntime.Instance.hasCompletedGrind = true;

        //SHOW GRIND DIALOGUE ONCE
        if (!hasShownDialogue)
         {
            ShowGrindDialogue();
            hasShownDialogue = true;
            return; // stop activation until dialogue finishes
        }
    }
    private void ShowGrindDialogue()
    {
        int grindIndex = CoffeeRuntime.Instance.playerSelectedGrindIndex;

        if (grindIndex <= 3)
            TutorialManager.Instance.StartDialogue(fineDialogue.messages);
        else if (grindIndex <= 4)
            TutorialManager.Instance.StartDialogue(mediumDialogue.messages);
        else if (grindIndex <= 7)
            TutorialManager.Instance.StartDialogue(coarseDialogue.messages);
    }

}