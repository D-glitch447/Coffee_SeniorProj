using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // <-- ADD THIS LINE!

public class CoffeeGrinder : MonoBehaviour
{
    

    [Header("Grind Settings")]
    [Tooltip("The total amount of rotation (in degrees) required to finish grinding. 3600 = 10 full rotations.")]
    public float totalGrindRequired = 3600f;

    [Header("UI (Optional)")]
    [Tooltip("A UI Image set to 'Filled' (e.g., Radial 360) to show progress.")]
    public Image progressFillImage;

    [Header("Numeric UI")] // <-- NEW HEADER
    [Tooltip("The TextMeshPro object to display the current grind amount.")]
    public TextMeshProUGUI grindAmountText; // <-- NEW VARIABLE

    // Private state variables
    [SerializeField] private float currentGrindProgress = 0f;
    private float previousAngle = 0f;
    private Vector3 centerScreenPos;
    private Camera mainCamera;

    [SerializeField] private float minDist = 20f;
    [SerializeField] private bool isGrinding = false;
    private float currentVisualAngle = 0f;

    // --- NEW VARIABLE ---
    // This tracks if we've hit the optimal goal at least once.
    private bool hasReachedOptimalGrind = false; // <-- NEW

    void Start()
    {
        mainCamera = Camera.main;
        centerScreenPos = mainCamera.WorldToScreenPoint(transform.position);
        currentVisualAngle = transform.eulerAngles.z;

        if (progressFillImage != null)
        {
            progressFillImage.fillAmount = 0;
        }
    }

    public void StartGrinding()
    {
        // We no longer check if we are "done", so the user can
        // always start grinding, even if the goal is met.
        isGrinding = true;
        previousAngle = GetMouseAngle();
    }

   void Update()
    {
        // --- THIS CHECK IS REMOVED ---
        // We remove the "return" statement that was at the top.
        // We WANT the script to keep running.

        if (isGrinding)
        {
            if (Input.GetMouseButtonUp(0))
            {
                isGrinding = false;
                return;
            }

            if (Vector2.Distance(Input.mousePosition, centerScreenPos) > minDist)
            {
                float currentAngle = GetMouseAngle();
                float deltaAngle = Mathf.DeltaAngle(previousAngle, currentAngle);

                // Visual Feedback (Relative Rotation)
                currentVisualAngle += deltaAngle;
                transform.eulerAngles = new Vector3(0, 0, currentVisualAngle);

                // Progress Calculation (uses the same deltaAngle)
                if (deltaAngle > 0)
                {
                    currentGrindProgress += deltaAngle;

                    // The UI progress bar will *keep* filling.
                    // If you want it to *stop* at 100%, change this logic.
                    if (progressFillImage != null)
                    {
                        progressFillImage.fillAmount = currentGrindProgress / totalGrindRequired;
                    }
                    UpdateGrindText(); // <-- CALL THE NEW FUNCTION HERE
                }

                previousAngle = currentAngle;

                // --- LOGIC MODIFIED HERE ---
                // We now check if progress is met AND we haven't already
                // triggered the "complete" event.
                if (currentGrindProgress >= totalGrindRequired && !hasReachedOptimalGrind)
                {
                    // This block will now only run ONCE.
                    hasReachedOptimalGrind = true; // Set the flag
                    OnGrindComplete();
                }
            }
        }
    }
    
    private float GetMouseAngle()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector2 direction = new Vector2(mousePos.x - centerScreenPos.x, mousePos.y - centerScreenPos.y);
        return (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) - 90f;
    }

    /// <summary>
    /// Called ONCE when the optimal grind is first reached.
    /// </summary>
    private void OnGrindComplete()
    {
        Debug.Log("Optimal Grinding Complete!");
        
        // --- THIS IS REMOVED ---
        // this.enabled = false; // <-- REMOVED!

        // --- ADD YOUR "OPTIMAL" LOGIC HERE ---
        // For example:
        // - Play a "Ding!" sound effect
        // - Show a green checkmark
        // - Call a function in your GameManager: GameManager.instance.SetGrindComplete(true);
    }

    /// <summary>
    /// Public function for other scripts to check the final grind value.
    /// </summary>
    public float GetCurrentGrindAmount()
    {
        return currentGrindProgress;
    }
    
    private void UpdateGrindText()
    {
        if (grindAmountText != null)
        {
            // Convert rotation (degrees) into a meaningful unit (e.g., "Grams")
            // Assuming totalGrindRequired (3600) = 20.0 Grams (A common espresso dose)
            // You can adjust the 20.0f to be a variable if you want the target to change.
            float targetGrams = 20.0f; 
            float grams = (currentGrindProgress / totalGrindRequired) * targetGrams; 

            // Safety clamp to prevent huge numbers
            grams = Mathf.Clamp(grams, 0f, 100f); 

            // Update the text. ":F1" formats the number to one decimal place (e.g., "7.5g")
            grindAmountText.text = $"{grams:F1}g";
        }
    }
}