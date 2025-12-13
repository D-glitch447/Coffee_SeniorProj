using UnityEngine;

public class PrepRoomController : MonoBehaviour
{
    public Collider2D sinkCollider;
    public Collider2D stoveCollider;

    void Start()
    {
        Debug.Log("PrepRoomTutorialLoader START CALLED");
        Debug.Log("PrepRoomTutorialLoader: Starting dialogue for state " + CoffeeRuntime.Instance.prepRoomState);
        // Turn everything off by default
        sinkCollider.enabled = false;
        stoveCollider.enabled = false;

        switch (CoffeeRuntime.Instance.prepRoomState)
        {
            case PrepRoomState.FirstTime:
                sinkCollider.enabled = true;
                break;

            case PrepRoomState.AfterSink:
                stoveCollider.enabled = true;
                break;
        }
    }
}
