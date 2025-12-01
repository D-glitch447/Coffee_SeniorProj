using UnityEngine;

public class TinCupCatcher : MonoBehaviour
{
    public float beanWeight = 0.5f;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bean"))
        {
            // Add weight to scale
            ScaleController.Instance.AddWeight(beanWeight); // example weight

            // Remove bean
            Destroy(collision.gameObject);
        }
    }
}
