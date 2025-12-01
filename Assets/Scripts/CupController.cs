using Unity.VisualScripting;
using UnityEngine;

public class CupController : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ClearCup();
        }
    }
    void ClearCup()
    {
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Bean"))
            {
                Destroy(child.gameObject);
            }
        }
        ScaleController.Instance.ClearWeight();
    }
}