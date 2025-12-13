using UnityEngine;

public class GrinderSelector : MonoBehaviour
{
    [Header("Objects to Turn ON")]
    [SerializeField] private GameObject fullGrinder;
    [SerializeField] private GameObject grinderHandle;
    [SerializeField] private GameObject[] otherObjectsToActivate;

    [Header("Objects to Turn OFF")]
    // Drag the object you want to deactivate here (e.g., the label or arrow)
    [SerializeField] private GameObject objectToDeactivate; 

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnMouseDown()
    {
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
    }
}