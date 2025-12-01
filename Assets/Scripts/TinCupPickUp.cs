using UnityEngine;

public class TinCupPickup : MonoBehaviour
{
    public GameObject beanPrefab;     
    public float beanWeight = 0.1f;    

    private GameObject heldBean;
    private Rigidbody2D heldRB;
    private bool isHolding = false;

    void Update()
    {
        HandleClickPickup();
        HandleHolding();
    }

    void HandleClickPickup()
    {
        // Left mouse click
        if (Input.GetMouseButtonDown(0) && !isHolding)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Raycast to whatever is under the mouse
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null)
            {
                // Debug: show what was clicked
                Debug.Log("Clicked: " + hit.collider.name);

                // Only react if THIS object was clicked
                if (hit.collider.gameObject == this.gameObject)
                {
                    PickUpBean();
                }
            }
        }
    }

    void PickUpBean()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        heldBean = Instantiate(beanPrefab, mousePos, Quaternion.identity);
        heldRB = heldBean.GetComponent<Rigidbody2D>();

        heldRB.bodyType = RigidbodyType2D.Kinematic;  // freeze physics while holding

        isHolding = true;

        // Subtract weight
        ScaleController.Instance.RemoveWeight(beanWeight);

        Debug.Log("Picked up bean — weight decreased.");
    }

    void HandleHolding()
    {
        if (!isHolding) return;

        // Bean follows mouse
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = 0;
        heldBean.transform.position = mouse;

        // Right click to throw bean
        if (Input.GetMouseButtonDown(1))
        {
            ThrowHeldBean();
        }
    }

    void ThrowHeldBean()
    {
        heldRB.bodyType = RigidbodyType2D.Dynamic; // resume physics

        Vector2 dir = (Camera.main.ScreenToWorldPoint(Input.mousePosition)
                       - heldBean.transform.position);
        dir.Normalize();

        float force = 8f;
        heldRB.AddForce(dir * force, ForceMode2D.Impulse);

        Debug.Log("Bean thrown!");

        heldBean = null;
        heldRB = null;
        isHolding = false;
    }
}
