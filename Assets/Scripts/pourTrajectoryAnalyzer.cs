using UnityEngine;

public class PourTrajectoryAnalyzer : MonoBehaviour
{
    // --- Configuration ---
    [Header("Detection Settings")]
    public float RotationThreshold = 90f; 
    public float CenterZoneRadius = 0.5f; 
    
    // --- Live Debugging ---
    [Header("Live Feedback")]
    [SerializeField] private string currentGesture = "None";
    [SerializeField] private float currentAngularSpeed; 
    [SerializeField] private float distanceToCenter;

    // --- State ---
    private Vector3 centerPosition;
    private Vector3 lastPosition;
    private float lastAngle;

    // This is the method Unity was saying was missing!
    public void Initialize(Vector3 center)
    {
        centerPosition = center;
        currentGesture = "Ready";
    }

    public void AddSamplePoint(Vector3 worldPos)
    {
        // 1. Calculate Distance to Center
        distanceToCenter = Vector3.Distance(worldPos, centerPosition);

        // 2. Calculate Angle relative to Center
        Vector3 offset = worldPos - centerPosition;
        float newAngle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;

        // 3. Calculate Angular Change 
        float deltaAngle = Mathf.DeltaAngle(lastAngle, newAngle);
        
        float instantaneousSpeed = Mathf.Abs(deltaAngle) / Time.deltaTime;
        currentAngularSpeed = Mathf.Lerp(currentAngularSpeed, instantaneousSpeed, 0.1f);

        // --- THE LOGIC ---
        if (distanceToCenter < CenterZoneRadius)
        {
            currentGesture = "Spot Pour / Center";
        }
        else if (currentAngularSpeed > RotationThreshold)
        {
            currentGesture = "Circular Motion";
        }
        else
        {
            currentGesture = "Slow / Linear";
        }

        // Update state for next frame
        lastAngle = newAngle;
        lastPosition = worldPos;
    }

    public string GetGesture()
    {
        return currentGesture;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(centerPosition, CenterZoneRadius);
        Gizmos.DrawLine(centerPosition, lastPosition);
    }
}