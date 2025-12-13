using UnityEngine;

public class BeanSelfDestruct : MonoBehaviour
{
    [Header("Tags the bean should disappear on contact with")]
    public string[] destroyOnTags = { "Counter", "Scale", "Table" };

    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (string tag in destroyOnTags)
        {
            if (collision.collider.CompareTag(tag))
            {
                Destroy(gameObject);
                return;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        foreach (string tag in destroyOnTags)
        {
            if (collision.CompareTag(tag))
            {
                Destroy(gameObject);
                return;
            }
        }
    }
}
