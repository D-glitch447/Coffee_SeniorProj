using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoffeeGrinder : MonoBehaviour
{
    [Header("Grind Settings")]
    [Tooltip("The total amount of rotation (in degrees) required to finish grinding.")]
    public float totalGrindRequired = 3600f;

    [Tooltip("The target weight in grams.")]
    public float targetGrams = 20.0f;

    [Header("Visuals (GameObjects)")]
    [Tooltip("Assign the 'fullGrndr' GameObject here. It will be hidden when done.")]
    public GameObject fullGrinderObject;

    [Tooltip("Assign the 'emptyGrinderNoHandle' GameObject here. It will be shown when done.")]
    public GameObject emptyGrinderObject;

    [Header("Audio")] // <-- NEW SECTION
    [Tooltip("Assign an AudioSource component here.")]
    public AudioSource audioSource;
    [Tooltip("The sound of beans grinding.")]
    public AudioClip grindingSound;
    [Tooltip("The sound of an empty grinder spinning.")]
    public AudioClip emptyGrinderSound;

    [Header("UI")]
    [Tooltip("A UI Image set to 'Filled' (e.g., Radial 360) to show progress.")]
    public Image progressFillImage;

    [Tooltip("The TextMeshPro object to display the current grind amount.")]
    public TextMeshProUGUI grindAmountText;

    // Private state variables
    [SerializeField] private float currentGrindProgress = 0f;
    private float previousAngle = 0f;
    private Vector3 centerScreenPos;
    private Camera mainCamera;

    [SerializeField] private float minDist = 20f;
    [SerializeField] private bool isGrinding = false;
    private float currentVisualAngle = 0f;
    private bool hasReachedOptimalGrind = false;

    void Start()
    {
        mainCamera = Camera.main;
        centerScreenPos = mainCamera.WorldToScreenPoint(transform.position);
        currentVisualAngle = transform.eulerAngles.z;

        if (progressFillImage != null) progressFillImage.fillAmount = 0;

        // Ensure audio loops so it doesn't cut out while dragging
        if (audioSource != null) audioSource.loop = true;

        UpdateGrindText();
    }

    public void StartGrinding()
    {
        isGrinding = true;
        previousAngle = GetMouseAngle();
    }

    void Update()
    {
        if (isGrinding)
        {
            // If user lets go of the mouse button, stop grinding/sound
            if (Input.GetMouseButtonUp(0))
            {
                StopGrinding();
                return;
            }

            if (Vector2.Distance(Input.mousePosition, centerScreenPos) > minDist)
            {
                float currentAngle = GetMouseAngle();
                float deltaAngle = Mathf.DeltaAngle(previousAngle, currentAngle);

                // Visual Feedback (Handle Rotation always happens)
                currentVisualAngle += deltaAngle;
                transform.eulerAngles = new Vector3(0, 0, currentVisualAngle);

                // Check if user is actually moving the mouse (grinding)
                if (deltaAngle > 0.1f) // Small threshold to prevent jitter
                {
                    HandleProgress(deltaAngle);
                    HandleAudio(true); // Playing sound
                }
                else
                {
                    HandleAudio(false); // Silence
                }

                previousAngle = currentAngle;
            }
        }
    }

    private void HandleProgress(float deltaAngle)
    {
        // Only increase progress if we haven't finished yet
        if (!hasReachedOptimalGrind)
        {
            currentGrindProgress += deltaAngle;

            // CAP THE PROGRESS: Prevent going over 100%
            if (currentGrindProgress >= totalGrindRequired)
            {
                currentGrindProgress = totalGrindRequired; // Hard cap
                hasReachedOptimalGrind = true;
                OnGrindComplete();
            }

            // Update UI
            if (progressFillImage != null)
            {
                progressFillImage.fillAmount = currentGrindProgress / totalGrindRequired;
            }
            UpdateGrindText();
        }
    }

    private void HandleAudio(bool isMoving)
    {
        if (audioSource == null) return;

        if (isMoving)
        {
            // Determine which clip should play based on progress
            AudioClip correctClip = hasReachedOptimalGrind ? emptyGrinderSound : grindingSound;

            // If the wrong clip is loaded, swap it
            if (audioSource.clip != correctClip)
            {
                audioSource.clip = correctClip;
                audioSource.Play();
            }
            // If nothing is playing, start playing
            else if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            // If user is holding handle but not moving, silence.
            if (audioSource.isPlaying)
            {
                audioSource.Pause(); 
            }
        }
    }

    private void StopGrinding()
    {
        isGrinding = false;
        if (audioSource != null) audioSource.Stop();
    }

    private float GetMouseAngle()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector2 direction = new Vector2(mousePos.x - centerScreenPos.x, mousePos.y - centerScreenPos.y);
        return (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) - 90f;
    }

    private void OnGrindComplete()
    {
        Debug.Log("Optimal Grinding Complete!");

        // Switch Visuals
        if (fullGrinderObject != null) fullGrinderObject.SetActive(false);
        if (emptyGrinderObject != null) emptyGrinderObject.SetActive(true);
    }

    private void UpdateGrindText()
    {
        if (grindAmountText != null)
        {
            float currentGrams = (currentGrindProgress / totalGrindRequired) * targetGrams;
            
            // Format: "20.0g / 20.0g"
            grindAmountText.text = $"{currentGrams:F1}g / {targetGrams:F1}g";
        }
    }
}