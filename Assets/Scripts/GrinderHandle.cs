
using UnityEngine;

public class GrinderHandle : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private CoffeeGrinder grindManager;
    [SerializeField] private Transform centerPivot;

    private float _initialOffset;
    private float _previousRotation;
    private Camera _mainCamera;
    private float grindStartTime;
    private bool isGrinding = false;
    private float totalRotation = 0f;


    private void Start()
    {
        _mainCamera = Camera.main;
        if (centerPivot == null) centerPivot = transform;
    }

    private void OnMouseDown()
    {
        if (!isGrinding)
        {
            isGrinding = true;
            grindStartTime = Time.time;
        }
        float distanceToScreen = _mainCamera.WorldToScreenPoint(centerPivot.position).z;
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = distanceToScreen;
        Vector3 mouseWorldPos = _mainCamera.ScreenToWorldPoint(mouseScreenPos);

        Vector3 direction = mouseWorldPos - centerPivot.position;
        float mouseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        float currentHandleAngle = transform.rotation.eulerAngles.z;
        _initialOffset = Mathf.DeltaAngle(mouseAngle, currentHandleAngle);

        _previousRotation = currentHandleAngle;
    }

    private void OnMouseDrag()
    {
        float distanceToScreen = _mainCamera.WorldToScreenPoint(centerPivot.position).z;
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = distanceToScreen;
        Vector3 mouseWorldPos = _mainCamera.ScreenToWorldPoint(mouseScreenPos);

        Vector3 direction = mouseWorldPos - centerPivot.position;
        float mouseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        float targetAngle = mouseAngle + _initialOffset;
        transform.rotation = Quaternion.Euler(0, 0, targetAngle);

        float degreesTurned = Mathf.Abs(Mathf.DeltaAngle(_previousRotation, targetAngle));
        totalRotation += degreesTurned;

        if (grindManager != null)
            grindManager.AddGrindProgress(degreesTurned);

        _previousRotation = targetAngle;
    }

    // ⭐ ADDED: call this from CoffeeGrinder when grinding completes
    public void FinalizeGrindStats()
    {
        CoffeeRuntime.Instance.playerGrindDuration = Time.time - grindStartTime;
        CoffeeRuntime.Instance.playerTotalGrindRotation = totalRotation;
    }
}
