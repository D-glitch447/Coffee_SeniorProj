using UnityEngine;

public class KettleDrag : MonoBehaviour
{
    public StoveBurner leftBurner;
    public StoveBurner rightBurner;
    private Vector3 startPosition;

    private Rigidbody2D rb;
    private bool isDragging = false;
    private bool isSnapped = false;
    private Vector3 offset;
    public AudioSource KettleTap;
    public AudioClip KettleTapClip;

    [Header("Snap Settings")]
    public float snapDistance = 1.25f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;

        // Prevent ALL rotation forever
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        //keep track of the kettle position in the beginning
        startPosition = transform.position;
    }

    void OnMouseDown()
    {
        if(TutorialManager.InputLocked) return;

        if(isSnapped) 
            return;

        isDragging = true;
        offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);

        leftBurner.SetKettlePresent(false);
        rightBurner.SetKettlePresent(false);

        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    void OnMouseUp()
    {
        isDragging = false;

        bool snappedLeft = TrySnapToBurner(leftBurner);
        bool snappedRight = TrySnapToBurner(rightBurner);

        if(snappedLeft || snappedRight)
        {
            isSnapped = true;
        }
    }

    void Update()
    {
        if (isDragging)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 newPos = mousePos + offset;
            newPos.z = 0;

            rb.MovePosition(newPos);
            rb.linearVelocity = Vector2.zero; // prevents drifting
        }
    }
    bool IsOffScreen()
    {
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);
        return viewportPos.y < -0.2f || viewportPos.y > 1.2f ||
            viewportPos.x < -0.2f || viewportPos.x > 1.2f;
    }

    void LateUpdate()
    {
        if (IsOffScreen())
        {
            ResetKettle();
        }
    }

    private void ResetKettle()
    {
        isDragging = false;
        isSnapped = false;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        rb.bodyType = RigidbodyType2D.Static;
        transform.position = startPosition;

        rb.bodyType = RigidbodyType2D.Dynamic;
    }


    private bool TrySnapToBurner(StoveBurner burner)
    {
        if (burner == null || burner.dropPoint == null)
            return false;

        float dist = Vector2.Distance(transform.position, burner.dropPoint.position);

        if (dist < snapDistance)
        {
            transform.position = burner.dropPoint.position;

            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f; //
            rb.bodyType = RigidbodyType2D.Static;

            KettleTap.PlayOneShot(KettleTapClip);
            burner.SetKettlePresent(true);
            return true;
        }
        return false;
    }

}
