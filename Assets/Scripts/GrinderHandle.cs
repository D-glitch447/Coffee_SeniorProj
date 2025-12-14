using UnityEngine;

public class GrinderHandle : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private CoffeeGrinder grindManager; // Drag your FullGrinder here!
    [SerializeField] private Transform centerPivot;
    
    private float _initialOffset;
    private float _previousRotation;
    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main;
        if (centerPivot == null) centerPivot = transform;
    }

    private void OnMouseDown()
    {
        // 1. Setup the Offset (Prevents Jumping)
        float distanceToScreen = _mainCamera.WorldToScreenPoint(centerPivot.position).z;
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = distanceToScreen; 
        Vector3 mouseWorldPos = _mainCamera.ScreenToWorldPoint(mouseScreenPos);

        Vector3 direction = mouseWorldPos - centerPivot.position;
        float mouseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        float currentHandleAngle = transform.rotation.eulerAngles.z;
        _initialOffset = Mathf.DeltaAngle(mouseAngle, currentHandleAngle);
        
        // Track rotation to calculate speed
        _previousRotation = currentHandleAngle;
    }

    private void OnMouseDrag()
    {
        // 1. Calculate Mouse Angle
        float distanceToScreen = _mainCamera.WorldToScreenPoint(centerPivot.position).z;
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = distanceToScreen;
        Vector3 mouseWorldPos = _mainCamera.ScreenToWorldPoint(mouseScreenPos);

        Vector3 direction = mouseWorldPos - centerPivot.position;
        float mouseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 2. Apply Visual Rotation
        float targetAngle = mouseAngle + _initialOffset;
        transform.rotation = Quaternion.Euler(0, 0, targetAngle);

        // 3. Calculate how much we turned since last frame
        // We use DeltaAngle to handle the wrap-around from 360 to 0
        float degreesTurned = Mathf.DeltaAngle(_previousRotation, targetAngle);
        
        // Take absolute value so counter-clockwise still counts as progress
        float progress = Mathf.Abs(degreesTurned);

        // 4. Report to Manager
        if (grindManager != null)
        {
            grindManager.AddGrindProgress(progress);
        }

        _previousRotation = targetAngle;
    }
}