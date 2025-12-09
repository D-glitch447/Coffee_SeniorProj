using UnityEngine;

public class KettleDrag : MonoBehaviour
{
    public StoveBurner leftBurner;
    public StoveBurner rightBurner;

    private Rigidbody2D rb;
    private bool isDragging = false;
    private Vector3 offset;

    [Header("Snap Settings")]
    public float snapDistance = 1.25f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    void OnMouseDown()
    {
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
    }

    void Update()
    {
        if (isDragging)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 newPos = mousePos + offset;
            newPos.z = 0;
            rb.MovePosition(newPos);
        }
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
            rb.bodyType = RigidbodyType2D.Static;

            burner.SetKettlePresent(true);
            return true;
        }
        return false;
    }
}
