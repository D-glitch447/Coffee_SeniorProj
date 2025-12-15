using UnityEngine;
using UnityEngine.UI;
using TMPro; // NEW: Required for Text Mesh Pro

public class KettleController : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    private CoffeeBedManager coffeeManager;
    private AudioSource audioSource;

    [Header("Sprites")]
    public Sprite PouringSprite; 
    public Sprite IdleSprite;    

    [Header("Pour Settings")]
    public Vector3 SpoutLocalOffset = Vector3.zero;
    
    [Header("Tilt Controls")]
    public float TiltSensitivity = 2.0f;
    public float MinTiltAngle = 10f; 
    public float MaxTiltAngle = 60f;

    [Header("UI Feedback")]
    public Slider FlowSlider;
    public TMP_Text FlowText; // NEW: Drag your TextMeshPro object here
    
    // 0.0 = Min Flow, 1.0 = Max Flow
    private float currentFlowFactor = 0.5f; 
    private float initialZRotation;
    private bool allowMouseFollow = false;


    void Start()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        coffeeManager = FindFirstObjectByType<CoffeeBedManager>();
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        initialZRotation = transform.rotation.eulerAngles.z;

        if (coffeeManager == null) Debug.LogError("Kettle could not find CoffeeBedManager!");
    }

    void Update()
    {
        if (!TutorialManager.InputLocked)
            allowMouseFollow = true;
        HandleMouseFollow();
        HandleTiltInput();
        HandlePouring();
        UpdateUI(); // NEW: Separate function to keep things clean
    }

    void HandleMouseFollow()
    {
        if (TutorialManager.InputLocked)
            return;
        if (!allowMouseFollow)
            return;
        if (Camera.main == null) return;
        Vector3 mousePosition = Input.mousePosition;
        float zDistance = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);
        mousePosition.z = zDistance;
        transform.position = Camera.main.ScreenToWorldPoint(mousePosition);
    }

    void HandleTiltInput()
    {
        if (TutorialManager.InputLocked)
            return;

        float tiltDirection = 0f;

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) tiltDirection = 1f;
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) tiltDirection = -1f;

        if (tiltDirection != 0f)
        {
            currentFlowFactor += tiltDirection * TiltSensitivity * Time.deltaTime;
            currentFlowFactor = Mathf.Clamp01(currentFlowFactor);
        }

        if (FlowSlider != null) FlowSlider.value = currentFlowFactor;
    }

    void UpdateUI()
    {
        if (TutorialManager.InputLocked)
            return;

        if (FlowText != null)
        {
            // Convert 0.0-1.0 factor to a realistic 0-10 g/s readout
            float displayValue = currentFlowFactor * 10f; 
            
            // "F1" formats it to 1 decimal place (e.g., "5.2 g/s")
            FlowText.text = displayValue.ToString("F1") + " g/s";
        }
    }

    void HandlePouring()
    {
        if (TutorialManager.InputLocked)
            return;

        if (Input.GetMouseButton(0)) 
        {
            float targetAngle = Mathf.Lerp(MinTiltAngle, MaxTiltAngle, currentFlowFactor);
            transform.rotation = Quaternion.Euler(0, 0, initialZRotation - targetAngle);

            if (currentFlowFactor <= 0.01f)
            {
                if (spriteRenderer != null) spriteRenderer.sprite = IdleSprite;
                if (audioSource != null && audioSource.isPlaying) audioSource.Stop();
            }
            else
            {
                if (spriteRenderer != null) spriteRenderer.sprite = PouringSprite;
                
                if (audioSource != null && !audioSource.isPlaying) audioSource.Play();
                
                // Dynamic pitch adjustment
                if (audioSource != null) audioSource.pitch = 0.8f + (currentFlowFactor * 0.4f);

                if (coffeeManager != null)
                {
                    Vector3 pourWorldPos = transform.TransformPoint(SpoutLocalOffset);
                    coffeeManager.ApplyPour(pourWorldPos, currentFlowFactor);
                }
            }
        }
        else 
        {
            if (spriteRenderer != null) spriteRenderer.sprite = IdleSprite;
            transform.rotation = Quaternion.Euler(0, 0, initialZRotation);
            if (audioSource != null && audioSource.isPlaying) audioSource.Stop();
        }
    }
}