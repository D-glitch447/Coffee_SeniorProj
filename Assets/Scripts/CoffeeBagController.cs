using UnityEngine;

public class CoffeeBagController : MonoBehaviour
{
    public Transform bagPivot;     // top-level rotation handle
    public Transform pourPoint;    // mouth where beans come out
    public GameObject beanPrefab;
    public float pourThreshold = 15f;
    public float pourRate = 0.3f;

    private bool isHolding = false;
    private float lastPourTime = 0f;

    void Update()
    {
        // Pick up bag
        if(Input.GetKeyDown(KeyCode.B))
        {
            Instantiate(beanPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        }
        if (Input.GetMouseButtonDown(0)) isHolding = true;
        else if (Input.GetMouseButtonUp(0)) isHolding = false;

        // Move with mouse (moves the bagPivot position)
        if (isHolding)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            bagPivot.position = mousePos;
        }

        // Tilt/Rotate the bagPivot (A/D or arrows)
        float rotationInput = Input.GetAxis("Horizontal");
        bagPivot.Rotate(0, 0, -rotationInput * 100 * Time.deltaTime);

        // Get the current tilt angle
        float currentAngle = bagPivot.eulerAngles.z;
        if (currentAngle > pourThreshold && Time.time - lastPourTime > pourRate)
        {
            // Spawn bean from pour point
            Debug.Log("Spawning bean at: " + pourPoint.position);
            Instantiate(beanPrefab, pourPoint.position, Quaternion.identity);
            lastPourTime = Time.time;
        }
    }
}
