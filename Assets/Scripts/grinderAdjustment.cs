// using UnityEngine;
// using TMPro;
// using UnityEngine.LightTransport;

// public class GrinderAdjustment : MonoBehaviour
// {
//     [Header("UI Components")]
//     [SerializeField] private TextMeshProUGUI grindLabel; 

//     [Header("The Objects to Rotate")]
//     // 1. Drag the MAIN HANDLE here (GrinderHandle or emptyGrinderNoHandle_0)
//     //    (NOTE: The screw attached to this handle will rotate automatically!)
//     [SerializeField] private Transform handleObject; 
    
//     // 2. Drag the EMPTY GRINDER SCREW here (the one inside 'emptyGrndr')
//     //    (This ensures the screw stays synced even when the handle is hidden)
//     [SerializeField] private Transform emptyGrinderScrew; 

//     [Header("Settings")]
//     [SerializeField] private float rotationAmount = 45f;

//     private GrindSize currentGrind = GrindSize.Medium;

//     private readonly string[] grindNames =
//     {
//         "Extra-Fine",
//         "Fine",
//         "Medium-Fine",
//         "Medium",
//         "Medium-Coarse",
//         "Coarse",
//         "Very Coarse",
//         "Extra Coarse",
//     };
    
//     private int currentGrindIndex = 1; 
//     private int maxIndex = 2;
//     private int minIndex = 0;

//     // private string[] grindNames = new string[] { "Medium-Fine", "Medium", "Medium-Coarse" };

//     private void Start()
//     {
//         UpdateUI();
//         CommitSelection();
//     }

//     public void AdjustGrind(int direction)
//     {
//         int targetIndex = currentGrindIndex + direction;

//         if (targetIndex >= minIndex && targetIndex <= maxIndex)
//         {
//             currentGrindIndex = targetIndex;

//             // ROTATE THE HANDLE (and its attached child screw)
//             if (handleObject != null)
//             {
//                 handleObject.Rotate(0, 0, -direction * rotationAmount);
//             }

//             // ROTATE THE OTHER SCREW (the one on the empty grinder)
//             if (emptyGrinderScrew != null)
//             {
//                 emptyGrinderScrew.Rotate(0, 0, -direction * rotationAmount);
//             }

//             UpdateUI();
//         }
//         else
//         {
//             Debug.Log("Cannot adjust further in that direction.");
//         }
//     }

//     private void UpdateUI()
//     {
//             grindLabel.text = grindNames[(int)currentGrind - 1];
//         // if (grindLabel != null)
//         // {
//         //     grindLabel.text = grindNames[currentGrindIndex];
//         // }
//     }
//     private void CommitSelection()
//     {
//         if (!CoffeeRuntime.Instance) return;
//         CoffeeRuntime.Instance.playerSelectedGrindIndex = (int)currentGrind;
//     }

//     // public int GetCurrentGrindIndex()
//     // {
//     //     return currentGrindIndex;
//     // }

//     // public string GetCurrentGrindName()
//     // {
//     //     if (currentGrindIndex >= 0 && currentGrindIndex < grindNames.Length)
//     //     {
//     //         return grindNames[currentGrindIndex];
//     //     }
//     //     return "Unknown";
//     // }
// }

// using UnityEngine;
// using TMPro;
// using Unity.VisualScripting;

// public class GrinderAdjustment : MonoBehaviour
// {
//     [Header("UI Components")]
//     [SerializeField] private TextMeshProUGUI grindLabel;

//     [Header("The Objects to Rotate")]
//     [SerializeField] private Transform handleObject;
//     [SerializeField] private Transform emptyGrinderScrew;

//     [Header("Settings")]
//     [SerializeField] private float rotationAmount = 45f;

//     private GrindSize currentGrind = GrindSize.Medium;
//     private int currentGrindIndex = 4;
//     private bool hasConfirmedSelection = false;
//     [SerializeField] private GameObject emptyGrinder;
//     [SerializeField] private CoffeeGrinder coffeeGrinder;
//     [SerializeField] private GameObject fullGrinder;

//     private readonly string[] grindNames =
//     {
//         "Extra Fine",
//         "Fine",
//         "Medium-Fine",
//         "Medium",
//         "Medium-Coarse",
//         "Coarse",
//         "Very Coarse",
//         "Extra Coarse"
//     };

//     private void OnEnable()
//     {
//         hasConfirmedSelection = false;

//         if (emptyGrinder != null)
//             emptyGrinder.SetActive(true);

//         if (fullGrinder != null)
//             fullGrinder.SetActive(false);

//         // UpdateUI();

//     }
//     private void Start()
//     {
//         UpdateUI();
//     }

//     public void AdjustGrind(int direction)
//     {
//         if(hasConfirmedSelection) return;

//         Debug.Log($"AdjustGrind called | confirmed = {hasConfirmedSelection}");

//         int next = currentGrindIndex + direction;
//         if (next < 1 || next > grindNames.Length) return;

//         currentGrindIndex = next;

//         handleObject?.Rotate(0,0,-direction * rotationAmount);
//         emptyGrinderScrew?.Rotate(0,0,-direction * rotationAmount);

//         // if (handleObject != null)
//         //     handleObject.Rotate(0, 0, -direction * rotationAmount);

//         // if (emptyGrinderScrew != null)
//         //     emptyGrinderScrew.Rotate(0, 0, -direction * rotationAmount);

//         UpdateUI();
//     }

//     public void ConfirmSelection()
//     {
//         if(hasConfirmedSelection) return;
        
//         hasConfirmedSelection = true;
//         if(CoffeeRuntime.Instance != null)
//         {
//             CoffeeRuntime.Instance.playerSelectedGrindIndex = currentGrindIndex;
//             CoffeeRuntime.Instance.playerGrindSizeName = grindNames[currentGrindIndex - 1];
//         }
//         Debug.Log($"[Grinder] Ideal grind locked: {grindNames[currentGrindIndex - 1]}");

//         if (emptyGrinder != null)
//             emptyGrinder.SetActive(false);

//         if (fullGrinder != null)
//             fullGrinder.SetActive(true);

//         if (coffeeGrinder != null)
//             coffeeGrinder.enabled = true;
//     }


//     private void UpdateUI()
//     {
//         if (grindLabel != null)
//             grindLabel.text = grindNames[(int)currentGrind - 1];
//     }
//     public int GetCurrentGrindIndex()
//     {
//         return (int)currentGrind;
//     }

//     public string GetCurrentGrindName()
//     {
//         return grindNames[(int)currentGrind - 1];
//     }

// }

using UnityEngine;
using TMPro;

public class GrinderAdjustment : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI grindLabel;
    // KEEP your existing names
    private readonly string[] grindNames =
    {
        "Extra Fine",
        "Fine",
        "Medium-Fine",
        "Medium",
        "Medium-Coarse",
        "Coarse",
        "Very Coarse",
        "Extra Coarse"
    };

    // KEEP your existing default
    private int currentIndex = 4;
    // private GrinderSizeTutorialHandler tutorialLoader;
    private void Start()
    {
        // tutorialLoader = GetComponent<GrinderSizeTutorialHandler>();
        if(CoffeeRuntime.Instance != null && CoffeeRuntime.Instance.playerSelectedGrindIndex > 0)
        {
            currentIndex = CoffeeRuntime.Instance.playerSelectedGrindIndex;
        }
        UpdateUI();
        SaveSelection(); 
    }

    public void AdjustGrind(int direction)
    {
        currentIndex = Mathf.Clamp(currentIndex + direction, 1, grindNames.Length);
        UpdateUI();
        SaveSelection(); 
    }

    private void UpdateUI()
    {
        if (grindLabel != null)
            grindLabel.text = grindNames[currentIndex - 1];
    }

    // ✅ ADD (small, explicit, safe)
    private void SaveSelection()
    {
        if (TutorialManager.InputLocked) return;


        if (CoffeeRuntime.Instance != null)
            CoffeeRuntime.Instance.playerSelectedGrindIndex = currentIndex;
        if(CoffeeRuntime.Instance.playerSelectedGrindIndex != currentIndex)
            CoffeeRuntime.Instance.playerSelectedGrindIndex = currentIndex;    
    }
}
