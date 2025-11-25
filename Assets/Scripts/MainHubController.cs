using UnityEngine;

public class MainHubController : MonoBehaviour
{
    [Header("References")]
    public GameObject bookObject;
    public GameObject kitchenGroup;

    public GameObject background1;
    public GameObject background2;

    void Start()
    {
        // Check the global state
        if (globalData.Instance.isKitchenUnlocked)
        {
            // If we are coming back from the other scene:
            bookObject.SetActive(false);
            kitchenGroup.SetActive(true);
            background1.SetActive(false);
            background2.SetActive(true);
        }
        else
        {
            // Default start:
            bookObject.SetActive(true);
            kitchenGroup.SetActive(false);
            background1.SetActive(true);
            background2.SetActive(false);
        }
    }
}