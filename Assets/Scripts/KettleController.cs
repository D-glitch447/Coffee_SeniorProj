using UnityEngine;
using UnityEngine.UI; // Added in case you use the slider later

public class KettleController : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    private CoffeeBedManager coffeeManager;

    

    [Header("Sprites")]
    public Sprite PouringSprite; 
    public Sprite IdleSprite;    

    [Header("Pour Settings")]
    public Vector3 SpoutLocalOffset = Vector3.zero;
    
    [Header("Tilt Controls")]
    [Tooltip("How fast the kettle tilts when holding A or D.")]
    public float TiltSensitivity = 2.0f; // Adjusted for key holding speed
    
    [Tooltip("Minimum angle (slow pour)")]
    public float MinTiltAngle = 10f; 
    [Tooltip("Maximum angle (fast pour)")]
    public float MaxTiltAngle = 60f;

    [Header("UI Feedback")]
    public Slider FlowSlider; // Optional: Drag a UI Slider here to see the rate
    
    // 0.0 = Min Flow, 1.0 = Max Flow
    private float currentFlowFactor = 0.5f; 
    private float initialZRotation;

    void Start()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        coffeeManager = FindObjectOfType<CoffeeBedManager>();
        initialZRotation = transform.rotation.eulerAngles.z;

        if (coffeeManager == null) Debug.LogError("Kettle could not find CoffeeBedManager!");
    }

    void Update()
    {
        HandleMouseFollow();
        HandleTiltInput();
        HandlePouring();
    }

    void HandleMouseFollow()
    {
        if (Camera.main == null) return;
        Vector3 mousePosition = Input.mousePosition;
        float zDistance = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);
        mousePosition.z = zDistance;
        transform.position = Camera.main.ScreenToWorldPoint(mousePosition);
    }

    void HandleTiltInput()
    {
        float tiltDirection = 0f;

        // D key = Tilt Forward (Pour Faster)
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) 
        {
            tiltDirection = 1f;
        }
        // A key = Tilt Backward (Pour Slower)
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) 
        {
            tiltDirection = -1f;
        }

        if (tiltDirection != 0f)
        {
            // Use Time.deltaTime for smooth movement regardless of framerate
            currentFlowFactor += tiltDirection * TiltSensitivity * Time.deltaTime;
            currentFlowFactor = Mathf.Clamp01(currentFlowFactor);
        }

        // Update UI Slider if connected
        if (FlowSlider != null)
        {
            FlowSlider.value = currentFlowFactor;
        }
    }

    void HandlePouring()
    {
        if (Input.GetMouseButton(0)) // Left Click / Hold to actually pour
        {
            if (spriteRenderer != null) spriteRenderer.sprite = PouringSprite;

            float targetAngle = Mathf.Lerp(MinTiltAngle, MaxTiltAngle, currentFlowFactor);
            transform.rotation = Quaternion.Euler(0, 0, initialZRotation - targetAngle);

            if (coffeeManager != null)
            {
                Vector3 pourWorldPos = transform.TransformPoint(SpoutLocalOffset);
                // PASSING TWO ARGUMENTS (Requires the CoffeeBedManager fix below)
                coffeeManager.ApplyPour(pourWorldPos, currentFlowFactor);
            }
        }
        else 
        {
            if (spriteRenderer != null) spriteRenderer.sprite = IdleSprite;
            transform.rotation = Quaternion.Euler(0, 0, initialZRotation);
        }
    }
}