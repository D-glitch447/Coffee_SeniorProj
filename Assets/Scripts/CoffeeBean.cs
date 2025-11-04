using System.Threading;
using UnityEngine;

public class CoffeeBean : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite[] beanSprites;

    [Header("Settings")]
    public float weight = 0.5f;
    private bool inCup = false;

    void Start()
    {
        if (beanSprites.Length > 0)
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            sr.sprite = beanSprites[Random.Range(0, beanSprites.Length)];
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Cup") && !inCup)
        {
            inCup = true;
            ScaleController.Instance.AddWeight(weight);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("TinCup"))
        {
            inCup = false;
            ScaleController.Instance.AddWeight(-weight);
        }
    }
}
