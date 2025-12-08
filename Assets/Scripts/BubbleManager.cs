using UnityEngine;
using System.Collections.Generic;

public class BubbleManager : MonoBehaviour
{
    [Header("Bubble Settings")]
    public GameObject bubblePrefab;  // The sprite to spawn
    public int poolSize = 50;        // Max bubbles allowed at once
    public float spawnRate = 0.05f;  // How fast to spawn (lower = more bubbles)
    
    [Header("Visuals")]
    public float minSize = 0.5f;
    public float maxSize = 1.2f;
    public float bubbleLifeTime = 1.0f; // How long they last before popping

    // Internal State
    private List<BubbleObj> pool;
    private float spawnTimer;

    // A simple class to track individual bubble data
    private class BubbleObj
    {
        public GameObject obj;
        public Transform trans;
        public float timer;
        public Vector3 originalScale;
    }

    void Start()
    {
        // 1. Initialize the Object Pool
        pool = new List<BubbleObj>();
        
        // Create a container to keep the Hierarchy clean
        GameObject container = new GameObject("Bubble_Pool_Container");
        container.transform.parent = this.transform;

        for (int i = 0; i < poolSize; i++)
        {
            GameObject go = Instantiate(bubblePrefab, container.transform);
            go.SetActive(false);
            
            BubbleObj b = new BubbleObj();
            b.obj = go;
            b.trans = go.transform;
            b.originalScale = go.transform.localScale;
            pool.Add(b);
        }
    }

    public void TrySpawnBubble(Vector3 position)
    {
        // 1. Rate Limiting
        spawnTimer += Time.deltaTime;
        if (spawnTimer < spawnRate) return;
        spawnTimer = 0f;

        // 2. Find an inactive bubble in the pool
        BubbleObj bubble = null;
        for (int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].obj.activeInHierarchy)
            {
                bubble = pool[i];
                break;
            }
        }

        // If pool is full, do nothing (or you could recycle the oldest one)
        if (bubble == null) return;

        // 3. Reset and Activate
        bubble.obj.SetActive(true);
        bubble.trans.position = position + (Vector3)(Random.insideUnitCircle * 0.2f); // Add slight randomness to position
        bubble.timer = bubbleLifeTime;

        // Randomize Rotation and Size for variety
        bubble.trans.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
        float randomScale = Random.Range(minSize, maxSize);
        bubble.originalScale = Vector3.one * randomScale;
        bubble.trans.localScale = bubble.originalScale;
    }

    void Update()
    {
        // animate all active bubbles
        for (int i = 0; i < pool.Count; i++)
        {
            BubbleObj b = pool[i];
            if (b.obj.activeInHierarchy)
            {
                b.timer -= Time.deltaTime;

                // Animation: Pop (Scale down rapidly at the end of life)
                float lifePercent = b.timer / bubbleLifeTime;
                
                // If last 20% of life, shrink to zero
                if (lifePercent < 0.2f)
                {
                    float shrink = lifePercent / 0.2f; 
                    b.trans.localScale = b.originalScale * shrink;
                }

                // Kill if time is up
                if (b.timer <= 0)
                {
                    b.obj.SetActive(false);
                }
            }
        }
    }
}