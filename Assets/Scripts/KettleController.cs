
using UnityEngine;

/// <summary>
/// KettleController - Follows the mouse and notifies the CoffeeBedManager when pouring.
/// Improvements:
/// - Uses Camera.main to convert screen->world with a safe z value
/// - Caches CoffeeBedManager and SpriteRenderer
/// - Does not assume kettle and coffee bed share z-depths; it keeps kettle z unchanged after mapping
/// </summary>
public class KettleController : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite PouringSprite; // Assign the tilted/pouring kettle sprite in the Inspector
    public Sprite IdleSprite;    // Assign the idle/untilted kettle sprite

    private CoffeeBedManager coffeeManager;

    // Optional: an offset to set the pour position relative to kettle transform (e.g., spout)
    [Header("Pour Offset (local)")]
    public Vector3 SpoutLocalOffset = Vector3.zero;

    void Start()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        coffeeManager = FindObjectOfType<CoffeeBedManager>();
        if (coffeeManager == null) Debug.LogWarning("No CoffeeBedManager found in scene.");
    }

    void Update()
    {
        // 1. Mouse Follow Logic
        Vector3 mousePosition = Input.mousePosition;
        // set a safe z based on camera - using a value that maps to plane z of this object
        Camera cam = Camera.main;
        if (cam == null) return;

        // We'll compute a z value so ScreenToWorldPoint returns a point that lies on the object's z plane
        float zForScreenToWorld = Mathf.Abs(cam.transform.position.z - transform.position.z);
        mousePosition.z = zForScreenToWorld;
        Vector3 worldPosition = cam.ScreenToWorldPoint(mousePosition);

        // Keep original z (to avoid moving the kettle off its intended z)
        worldPosition.z = transform.position.z;
        transform.position = worldPosition;

        // 2. Pouring and Sprite/Logic Toggle
        if (Input.GetMouseButton(0))
        {
            if (spriteRenderer != null && PouringSprite != null) spriteRenderer.sprite = PouringSprite;

            if (coffeeManager != null)
            {
                // Calculate world position of the pour point (optionally offset from kettle transform)
                Vector3 pourWorldPos = transform.TransformPoint(SpoutLocalOffset);
                coffeeManager.ApplyPour(pourWorldPos);
            }
        }
        else
        {
            if (spriteRenderer != null && IdleSprite != null) spriteRenderer.sprite = IdleSprite;
            
        }

        if (Input.GetMouseButton(0))
        {
            if (spriteRenderer != null && PouringSprite != null) spriteRenderer.sprite = PouringSprite;

                if (coffeeManager != null)
                {
                    // --- ADD THIS LOG ---
                    // Debug.Log("Kettle is calling ApplyPour!"); 
                    
                    Vector3 pourWorldPos = transform.TransformPoint(SpoutLocalOffset);
                    coffeeManager.ApplyPour(pourWorldPos);
                }
                else
                {
                // --- ADD THIS LOG ---
                Debug.LogError("Kettle cannot find CoffeeManager! Is it attached to the CoffeeGroundsManager object?");
            }
        }

    }
}
