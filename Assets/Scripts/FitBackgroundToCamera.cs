using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FitBackgroundToCamera : MonoBehaviour
{
    void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        float worldHeight = Camera.main.orthographicSize * 2f;
        float worldWidth = worldHeight * Camera.main.aspect;

        transform.localScale = new Vector3(
            worldWidth / sr.bounds.size.x,
            worldHeight / sr.bounds.size.y,
            1
        );
    }

}
