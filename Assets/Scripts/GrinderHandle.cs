using UnityEngine;

public class GrinderHandle : MonoBehaviour
{
    // This will hold the main script from the parent object
    private CoffeeGrinder grinderScript;

    void Start()
    {
        // Find the CoffeeGrinder script on our parent object (the pivot)
        grinderScript = GetComponentInParent<CoffeeGrinder>();
    }

    // This is a built-in Unity function that runs
    // when a Collider 2D is clicked
    private void OnMouseDown()
    {
        // Tell the main script to start grinding
        grinderScript.StartGrinding();
    }
}