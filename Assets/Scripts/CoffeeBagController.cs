// using UnityEngine;

// public class CoffeeBagController : MonoBehaviour
// {
//     [Header("References")]
//     public Transform pourPoint;
//     public GameObject beanPrefab;

//     [Header("Pour Settings")]
//     public float pourStartAngle = 15f;
//     public float pourStopAngle = 80f;
//     public float minPourRate = 0.6f;
//     public float maxPourRate = 0.05f;

//     [Header("Physics Settings")]
//     public float gravityScale = 1.3f;
//     public float pickupLift = 0.05f;
//     public float visibleZ = 0f;

//     private Rigidbody2D rb;
//     private Collider2D bagCollider;
//     private Camera mainCam;
//     private bool isHolding = false;
//     private float lastPourTime = 0f;

//     void Awake()
//     {
//         rb = GetComponent<Rigidbody2D>();
//         if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();

//         bagCollider = GetComponent<Collider2D>();
//         if (bagCollider == null) bagCollider = gameObject.AddComponent<BoxCollider2D>();

//         rb.bodyType = RigidbodyType2D.Dynamic;
//         rb.gravityScale = gravityScale;
//         rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
//         rb.interpolation = RigidbodyInterpolation2D.Interpolate;
//     }

//     void Start()
//     {
//         mainCam = Camera.main;
//     }

//     void Update()
//     {
//         HandlePickupAndDrag();
//         HandlePouring();
//     }

//     private void HandlePickupAndDrag()
//     {
//         if (Input.GetMouseButtonDown(0))
//         {
//             Vector3 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
//             transform.position += Vector3.up * pickupLift;

//             isHolding = true;

//             rb.linearVelocity = Vector2.zero;
//             rb.angularVelocity = 0f;
//             rb.bodyType = RigidbodyType2D.Kinematic;
//         }

//         if (Input.GetMouseButtonUp(0))
//         {
//             isHolding = false;
//             rb.bodyType = RigidbodyType2D.Dynamic;
//             rb.gravityScale = gravityScale;
//         }

//         if (isHolding)
//         {
//             Vector3 mouseWorld = mainCam.ScreenToWorldPoint(Input.mousePosition);
//             mouseWorld.z = visibleZ;
//             transform.position = new Vector3(mouseWorld.x, mouseWorld.y + pickupLift, visibleZ);

//             float rotationInput = Input.GetAxis("Horizontal");
//             transform.Rotate(0, 0, -rotationInput * 100f * Time.deltaTime);
//         }
//     }

//     private void HandlePouring()
//     {
//         float zAngle = transform.eulerAngles.z;
//         if (zAngle > 180f) zAngle -= 360f;
//         float absAngle = Mathf.Abs(zAngle);

//         if (absAngle > pourStartAngle)
//         {
//             float t = Mathf.InverseLerp(pourStartAngle, pourStopAngle, absAngle);
//             float currentRate = Mathf.Lerp(minPourRate, maxPourRate, t);

//             if (Time.time - lastPourTime > currentRate)
//             {
//                 if (beanPrefab != null)
//                     Instantiate(beanPrefab, pourPoint.position, Quaternion.identity);
//                 else
//                     Debug.LogError("Bean prefab is missing!");

//                 lastPourTime = Time.time;
//             }
//         }
//     }

//     private void OnCollisionEnter2D(Collision2D collision)
//     {
//         if (collision.collider.CompareTag("Table"))
//         {
//             rb.angularVelocity = 0f;
//             rb.linearVelocity = Vector2.zero;
//         }
//     }
// }
using UnityEngine;

public class CoffeeBagController : MonoBehaviour
{
    [Header("References")]
    public Transform pourPoint;
    public GameObject beanPrefab;

    [Header("Pour Settings")]
    public float pourStartAngle = 15f;
    public float pourStopAngle = 80f;
    public float minPourRate = 0.6f;
    public float maxPourRate = 0.05f;

    [Header("Physics Settings")]
    public float gravityScale = 1.3f;
    public float pickupLift = 0.05f;
    public float visibleZ = 0f;

    [Header("Interaction Settings")]
    public float clickRange = 0.7f;   // mouse must be THIS close to bag
    public float dragSmooth = 20f;    // smoother dragging feel

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

    // -----------------------------------------------------------
    //  CLICK + PICKUP LOGIC (PRECISE WITH RAYCAST + DISTANCE)
    // -----------------------------------------------------------
    private void HandleClickPickup()
    {
        if (Input.GetMouseButtonDown(0))
        {
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

    // -----------------------------------------------------------
    //  DRAGGING LOGIC (SMOOTH AND CONTROLLED)
    // -----------------------------------------------------------
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
        transform.Rotate(0, 0, -rotationInput * 100f * Time.deltaTime);
    }

    // -----------------------------------------------------------
    //  POURING LOGIC (UNCHANGED, WORKS AS BEFORE)
    // -----------------------------------------------------------
    private void HandlePouring()
    {
        float zAngle = transform.eulerAngles.z;
        if (zAngle > 180f) zAngle -= 360f;
        float absAngle = Mathf.Abs(zAngle);

        if (absAngle > pourStartAngle)
        {
            float t = Mathf.InverseLerp(pourStartAngle, pourStopAngle, absAngle);
            float currentRate = Mathf.Lerp(minPourRate, maxPourRate, t);

            if (Time.time - lastPourTime > currentRate)
            {
                if (beanPrefab != null)
                    Instantiate(beanPrefab, pourPoint.position, Quaternion.identity);
                else
                    Debug.LogError("Bean prefab is missing!");

                lastPourTime = Time.time;
            }
        }
    }

    // -----------------------------------------------------------
    //  STABILIZE COLLISIONS
    // -----------------------------------------------------------
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Table"))
        {
            rb.angularVelocity = 0f;
            rb.linearVelocity = Vector2.zero;
        }
    }
}
