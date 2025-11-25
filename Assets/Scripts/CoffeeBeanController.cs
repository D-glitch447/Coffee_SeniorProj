using UnityEngine;

public class CoffeeBeanController : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite[] beanSprites;  // assign your different bean images here

    [Header("Settings")]
    public float weight = 0.5f;     // grams per bean
    //private bool inCup = false;

    void Start()
    {
        // Grab the SpriteRenderer attached to this object
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        // Pick a random sprite from the array
        if (beanSprites != null && beanSprites.Length > 0)
        {
            sr.sprite = beanSprites[Random.Range(0, beanSprites.Length)];
        }
        else
        {
            Debug.LogWarning("No bean sprites assigned to CoffeeBeanController!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trigger detected with: " + collision.name);
        // if(collision.CompareTag("TinCup"))
        // {
        //     ScaleController.Instance.AddWeight(weight);
        //     Destroy(gameObject);
        // }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Trigger detected with: " + collision.collider.name);
        // if (collision.gameObject.CompareTag("TinCup") && !inCup)
        // {
        //     inCup = true;
        //     ScaleController.Instance.AddWeight(weight);
        // }
    }
}
