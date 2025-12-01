
using UnityEngine;

public class CameraSlide : MonoBehaviour
{
    public Transform cameraTarget; 
    public float slideSpeed = 5f;

    // Your scene positions
    public float stoveX = 0f;        // center of stove
    public float sinkX = 2f;        // center of sink

    private float targetX;

    void Start()
    {
        targetX = stoveX;  // start in stove scene
        SetCameraToTarget(); 
    }

    void Update()
    {
        Vector3 pos = cameraTarget.position;
        pos.x = Mathf.Lerp(pos.x, targetX, Time.deltaTime * slideSpeed);
        cameraTarget.position = pos;
    }

    public void MoveRight()
    {
        targetX = sinkX;   // go to sink
    }

    public void MoveLeft()
    {
        targetX = stoveX;  // go back to stove
    }

    void SetCameraToTarget()
    {
        cameraTarget.position = new Vector3(targetX, cameraTarget.position.y, cameraTarget.position.z);
    }
}
