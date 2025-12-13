using UnityEngine;
using TMPro;

public class GrinderAdjustment : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI grindLabel; 

    [Header("The Objects to Rotate")]
    // 1. Drag the MAIN HANDLE here (GrinderHandle or emptyGrinderNoHandle_0)
    //    (NOTE: The screw attached to this handle will rotate automatically!)
    [SerializeField] private Transform handleObject; 
    
    // 2. Drag the EMPTY GRINDER SCREW here (the one inside 'emptyGrndr')
    //    (This ensures the screw stays synced even when the handle is hidden)
    [SerializeField] private Transform emptyGrinderScrew; 

    [Header("Settings")]
    [SerializeField] private float rotationAmount = 45f;
    
    private int currentGrindIndex = 1; 
    private int maxIndex = 2;
    private int minIndex = 0;

    private string[] grindNames = new string[] { "Medium-Fine", "Medium", "Medium-Coarse" };

    private void Start()
    {
        UpdateUI();
    }

    public void AdjustGrind(int direction)
    {
        int targetIndex = currentGrindIndex + direction;

        if (targetIndex >= minIndex && targetIndex <= maxIndex)
        {
            currentGrindIndex = targetIndex;

            // ROTATE THE HANDLE (and its attached child screw)
            if (handleObject != null)
            {
                handleObject.Rotate(0, 0, -direction * rotationAmount);
            }

            // ROTATE THE OTHER SCREW (the one on the empty grinder)
            if (emptyGrinderScrew != null)
            {
                emptyGrinderScrew.Rotate(0, 0, -direction * rotationAmount);
            }

            UpdateUI();
        }
        else
        {
            Debug.Log("Cannot adjust further in that direction.");
        }
    }

    private void UpdateUI()
    {
        if (grindLabel != null)
        {
            grindLabel.text = grindNames[currentGrindIndex];
        }
    }

    public int GetCurrentGrindIndex()
    {
        return currentGrindIndex;
    }

    public string GetCurrentGrindName()
    {
        if (currentGrindIndex >= 0 && currentGrindIndex < grindNames.Length)
        {
            return grindNames[currentGrindIndex];
        }
        return "Unknown";
    }
}