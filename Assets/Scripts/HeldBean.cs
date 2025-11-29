using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class HeldBean : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D col;
    private Camera cam;

    private bool isHeld = true;
    private float spawnTime;
    private Vector3 prevMouseWorld;
    private Vector3 currMouseWorld;

    public static bool AnyBeanHeld = false;

    [Header("Hold / Throw")]
    public float holdZ = 0f;              // Your 2D plane Z
    public float followSmoothing = 40f;   // Higher = snappier follow
    public float throwMultiplier = 8f;    // Strength
    public float throwUnlockDelay = 0.12f;// Debounce so initial click can't insta-throw

    // Optional: layer to avoid all collisions while held
    public string heldLayerName = "HeldBean";   // create this layer (Project Settings > Tags & Layers)
    private int originalLayer = -1;

    void Awake()
    {
        rb  = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        cam = Camera.main;

        // Mark that a bean is being held
        AnyBeanHeld = true;
        
        // Freeze physics while held
        rb.bodyType     = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;

        // Disable collisions while held (and/or move to a non-colliding layer)
        originalLayer = gameObject.layer;
        int heldLayer = LayerMask.NameToLayer(heldLayerName);
        if (heldLayer != -1) gameObject.layer = heldLayer; // ignore if layer not created

        col.enabled = false;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;

        spawnTime = Time.time;

        // Initialize mouse sample
        prevMouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        prevMouseWorld.z = holdZ;
        currMouseWorld = prevMouseWorld;

        // Ensure this won't be eaten by the cup trigger
        gameObject.tag = "HeldBean";
    }

    void Update()
    {
        if (!isHeld) return;

        // Follow cursor smoothly
        currMouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        currMouseWorld.z = holdZ;
        Vector3 next = Vector3.Lerp(transform.position, currMouseWorld, Time.deltaTime * followSmoothing);
        rb.MovePosition(next);

        // Only allow throwing AFTER the initial click has been released (MouseButtonUp)
        // and after a tiny delay to avoid same-frame spawn/throw.
        if (Time.time - spawnTime >= throwUnlockDelay && Input.GetMouseButtonUp(0))
        {
            Throw();
        }
    }

    private void Throw()
    {
        isHeld = false;
        AnyBeanHeld = false; // reset when thrown
        // Restore collisions and physics
        col.enabled = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 1f;
        if (originalLayer != -1) gameObject.layer = originalLayer;

        gameObject.tag = "CoffeeBean";

        // Compute a simple flick velocity from the last mouse delta
        Vector2 delta = (Vector2)(currMouseWorld - prevMouseWorld);
        Vector2 v = delta / Mathf.Max(Time.deltaTime, 0.0001f);
        rb.linearVelocity = v * throwMultiplier;

        Destroy(gameObject, 5f); // tidy up later
    }

    void LateUpdate()
    {
        // Keep previous sample for next frame velocity estimate
        prevMouseWorld = currMouseWorld;
    }
}
