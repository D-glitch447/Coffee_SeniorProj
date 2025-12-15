using UnityEngine;

public class CoffeeBagController : MonoBehaviour
{
    [Header("References")]
    public Transform pourPoint;
    public GameObject beanPrefab;

    [Header("Pour Settings")]
    public float pourStartAngle = 40f;
    public float pourStopAngle = 80f;
    public float minPourRate = 0.6f;
    public float maxPourRate = 0.05f;

    [Header("Physics Settings")]
    public float gravityScale = 1.3f;
    public float pickupLift = 0.05f;
    public float visibleZ = 0f;

    [Header("Rotation Limits")]
    public float maxTiltAngle = 95f;   // how far it can tilt


    [Header("Interaction Settings")]
    public float clickRange = 0.7f;   // mouse must be THIS close to bag
    public float dragSmooth = 20f;    // smoother dragging feel

    [Header("Dialogue Open Sound")]
    public AudioSource pourAudio;

    public float minVolume = 0.05f;
    public float maxVolume = 0.6f;

    public float volumeSmoothSpeed = 8f;


    private Rigidbody2D rb;
    private Camera mainCam;
    private bool isHolding = false;
    private float lastPourTime = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = gravityScale;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        mainCam = Camera.main;
    }

    void Update()
    {
        HandleClickPickup();
        HandleHolding();
        HandlePouring();
    }

    private void HandleClickPickup()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if(TutorialManager.InputLocked) return;
            Vector2 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

            // 1. Raycast check (hit EXACTLY the bag's collider)
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == this.gameObject)
            {
                // 2. Optional distance check
                if (Vector2.Distance(mousePos, transform.position) <= clickRange)
                {
                    StartHolding(mousePos);
                }
            }
        }

        if (Input.GetMouseButtonUp(0) && isHolding)
        {
            StopHolding();
        }
    }

    private void UpdatePourAudio(float absAngle)
    {
        // Below pour angle → fade out & stop
        if (absAngle < pourStartAngle)
        {
            if (pourAudio.isPlaying)
            {
                pourAudio.volume = Mathf.Lerp(
                    pourAudio.volume,
                    0f,
                    Time.deltaTime * volumeSmoothSpeed
                );

                if (pourAudio.volume < 0.01f)
                    pourAudio.Stop();
            }
            return;
        }

        // Start audio if needed
        if (!pourAudio.isPlaying)
            pourAudio.Play();

        // Normalize tilt
        float t = Mathf.InverseLerp(pourStartAngle, pourStopAngle, absAngle);

        // Target volume based on tilt
        float targetVolume = Mathf.Lerp(minVolume, maxVolume, t);

        // Smooth volume change
        pourAudio.volume = Mathf.Lerp(
            pourAudio.volume,
            targetVolume,
            Time.deltaTime * volumeSmoothSpeed
        );

        // Optional subtle pitch variation (very nice)
        pourAudio.pitch = Mathf.Lerp(0.95f, 1.1f, t);
    }


    private void StartHolding(Vector2 mouse)
    {
        isHolding = true;

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0;

        // Tiny lift when grabbing
        transform.position += Vector3.up * pickupLift;
    }

    private void StopHolding()
    {
        isHolding = false;

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = gravityScale;
    }

    private void HandleHolding()
    {
        if (!isHolding) return;

        Vector3 mouseWorld = mainCam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = visibleZ;

        // Smooth dragging
        transform.position = Vector3.Lerp(
            transform.position,
            new Vector3(mouseWorld.x, mouseWorld.y + pickupLift, visibleZ),
            Time.deltaTime * dragSmooth
        );

        // Rotate left/right (optional)
        float rotationInput = Input.GetAxis("Horizontal");

        float currentZ = transform.eulerAngles.z;
        if (currentZ > 180f) currentZ -= 360f;

        // Calculate new rotation
        float targetZ = currentZ - rotationInput * 100f * Time.deltaTime;

        // Clamp rotation
        targetZ = Mathf.Clamp(targetZ, -maxTiltAngle, maxTiltAngle);

        // Apply rotation
        transform.rotation = Quaternion.Euler(0, 0, targetZ);

    }
    private void HandlePouring()
    {
        float zAngle = transform.eulerAngles.z;
        if (zAngle > 180f) zAngle -= 360f;
        float absAngle = Mathf.Abs(zAngle);

        if (absAngle > pourStartAngle)
        {
            float t = Mathf.InverseLerp(pourStartAngle, pourStopAngle, absAngle);
            float currentRate = Mathf.Lerp(minPourRate, maxPourRate, t);

            // Update pouring sound
            UpdatePourAudio(absAngle);

            if (Time.time - lastPourTime > currentRate)
            {
                if (beanPrefab != null) {
                    Instantiate(beanPrefab, pourPoint.position, Quaternion.identity);
                    lastPourTime = Time.time;
                }
            }
        }
        else
        {
            // Stop pouring sound if angle too small
            UpdatePourAudio(absAngle);
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Table"))
        {
            rb.angularVelocity = 0f;
            rb.linearVelocity = Vector2.zero;
        }
    }
}
