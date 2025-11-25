using UnityEngine;

public class CoffeeBedManager : MonoBehaviour
{
    // --- Configurable Variables ---
    [Header("Grid Settings")]
    public int DesiredGridSize = 32; 
    public float PourRate = 0.5f;           
    public float DiffusionRate = 0.03f;     

    // --- Phase & Time Management ---
    private enum PourState { Idle, Blooming, PouringFinished }
    private PourState currentState = PourState.Idle;

    [Header("Phase Settings")]
    public float BloomTime = 5.0f;          
    public float TotalPourDuration = 30.0f; 
    private float phaseTimer = 0.0f;
    private float totalPourMass = 0.0f;      

    // --- Scoring Variables ---
    [Header("Scoring Settings")]
    public float UniformityWeight = 1000f; 

    // Inspector Debugging
    [SerializeField] private float currentMeanSaturation;
    [SerializeField] private float currentSigma;
    public float FinalScore { get; private set; }

    [Header("Debug")]
    public bool LogScoreOnFinish = true;

    // --- Private Variables ---
    private int gridSize; 
    private float[,] saturationGrid;
    private Texture2D coffeeTexture;         
    private SpriteRenderer spriteRenderer;
    private Color[] initialDryColors; // Stores your original coffee texture
    private Color WetColor = new Color(0.2f, 0.1f, 0.05f, 1.0f); // Dark Brown

    void Start()
    {
        InitializeTexture();
    }

    private void InitializeTexture()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null || spriteRenderer.sprite == null) return;

        Sprite initialSprite = spriteRenderer.sprite;
        
        // 1. Get exact dimensions of the original sprite
        int width = (int)initialSprite.textureRect.width;
        int height = (int)initialSprite.textureRect.height;

        // 2. Capture the ORIGINAL pixels (so it looks like your coffee image)
        if (initialSprite.texture.isReadable)
        {
            initialDryColors = initialSprite.texture.GetPixels(
                (int)initialSprite.textureRect.x, 
                (int)initialSprite.textureRect.y, 
                width, 
                height
            );
        }
        else
        {
            Debug.LogError("Read/Write is NOT enabled on the Coffee Sprite texture settings!");
            return;
        }

        // 3. Create the dynamic texture
        coffeeTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        coffeeTexture.filterMode = FilterMode.Point;
        coffeeTexture.wrapMode = TextureWrapMode.Clamp;
        
        // Fill it with the original image data
        coffeeTexture.SetPixels(initialDryColors);
        coffeeTexture.Apply();

        // 4. Create the new Sprite, FORCING Pivot to Center (0.5, 0.5)
        // This is crucial for the ApplyPour math to work with your 200x scale.
        spriteRenderer.sprite = Sprite.Create(
            coffeeTexture, 
            new Rect(0, 0, width, height), 
            Vector2.one * 0.5f, 
            initialSprite.pixelsPerUnit
        );

        // Set Grid Size
        gridSize = Mathf.Min(DesiredGridSize, width);
        saturationGrid = new float[gridSize, gridSize];

        Debug.Log($"Initialized. Grid Size: {gridSize}x{gridSize}");
    }

    public void ApplyPour(Vector3 worldPourPosition)
{
    // Check 1: Do we have data?
    if (saturationGrid == null || spriteRenderer == null) 
    {
        Debug.LogError("ApplyPour failed: Grid or SpriteRenderer is null.");
        return;
    }

    // Check 2: Visual Debugging
    Debug.DrawLine(worldPourPosition, new Vector3(worldPourPosition.x, worldPourPosition.y, 10), Color.yellow, 0.1f);

    Bounds bounds = spriteRenderer.bounds;

    // Check 3: Math
    float normalizedX = (worldPourPosition.x - bounds.min.x) / bounds.size.x;
    float normalizedY = (worldPourPosition.y - bounds.min.y) / bounds.size.y;

    // --- THE LOUD LOG ---
    // This will print 60 times a second, so you WILL see if this function runs.
    Debug.Log($"Pour Input -> World: {worldPourPosition} | Norm: {normalizedX:F2}, {normalizedY:F2}"); 

    // Check 4: Bounds
    if (normalizedX < 0f || normalizedX > 1f || normalizedY < 0f || normalizedY > 1f) 
    {
        Debug.LogWarning("Pouring OUTSIDE bounds!"); // Tells you if you missed
        return; 
    }

    int gridX = Mathf.FloorToInt(normalizedX * gridSize);
    int gridY = Mathf.FloorToInt(normalizedY * gridSize);

    gridX = Mathf.Clamp(gridX, 0, gridSize - 1);
    gridY = Mathf.Clamp(gridY, 0, gridSize - 1);

    // Check 5: Applying
    saturationGrid[gridX, gridY] = Mathf.Clamp01(saturationGrid[gridX, gridY] + PourRate * Time.deltaTime);
    
    // Confirm Hit
    Debug.Log($"HIT Grid [{gridX},{gridY}] - Saturation: {saturationGrid[gridX, gridY]}");
}

    private void DiffuseSaturation()
    {
        if (saturationGrid == null) return;
        float[,] next = (float[,])saturationGrid.Clone();

        for (int x = 1; x < gridSize - 1; x++)
        {
            for (int y = 1; y < gridSize - 1; y++)
            {
                float neighborSum = saturationGrid[x - 1, y] + saturationGrid[x + 1, y] + saturationGrid[x, y - 1] + saturationGrid[x, y + 1];
                float avg = neighborSum / 4f;
                float diff = (avg - saturationGrid[x, y]) * DiffusionRate;
                next[x, y] = Mathf.Clamp01(next[x, y] + diff);
            }
        }
        saturationGrid = next;
    }

    private void UpdateVisuals()
    {
        if (coffeeTexture == null || initialDryColors == null) return;

        int texWidth = coffeeTexture.width;
        int texHeight = coffeeTexture.height;
        int blockW = texWidth / gridSize;
        int blockH = texHeight / gridSize;

        // Visual Update Loop
        for (int gx = 0; gx < gridSize; gx++)
        {
            for (int gy = 0; gy < gridSize; gy++)
            {
                float wetness = saturationGrid[gx, gy];
                
                // Only update pixels if there is some wetness to show change
                if (wetness <= 0.01f) continue; 

                int startX = gx * blockW;
                int startY = gy * blockH;

                for (int px = 0; px < blockW; px++)
                {
                    for (int py = 0; py < blockH; py++)
                    {
                        int x = startX + px;
                        int y = startY + py;
                        
                        if(x >= texWidth || y >= texHeight) continue;

                        int index = y * texWidth + x;
                        Color originalColor = initialDryColors[index];
                        
                        // Blend Original Image with Wet Color
                        Color finalColor = Color.Lerp(originalColor, WetColor, wetness);
                        coffeeTexture.SetPixel(x, y, finalColor);
                    }
                }
            }
        }
        coffeeTexture.Apply();
    }

    void Update()
    {
    bool isPouring = Input.GetMouseButton(0);

    // Start the game on the very first click
    if (currentState == PourState.Idle && isPouring) 
    {
        currentState = PourState.Blooming;
    }

    // If game is active (Blooming), update timer and stats
    if (currentState == PourState.Blooming)
    {
        phaseTimer += Time.deltaTime;
        
        // Run simulation logic
        DiffuseSaturation();
        UpdateVisuals();     // (Might not be visible yet)
        UpdateLiveStats();   // Updates the Inspector numbers

        // End game ONLY if time runs out
        if (phaseTimer >= TotalPourDuration)
        {
            currentState = PourState.PouringFinished;
            CalculateFinalScore();
            Debug.Log($"Time's up! Final Score: {FinalScore:F1}");
        }
        }
    }

    private void CalculateFinalScore()
    {
        if (saturationGrid == null) return;

        float sum = 0f;
        int n = gridSize * gridSize;
        for (int x = 0; x < gridSize; x++)
            for (int y = 0; y < gridSize; y++)
                sum += saturationGrid[x, y];

        float mean = sum / n;
        currentMeanSaturation = mean;

        float sumSq = 0f;
        for (int x = 0; x < gridSize; x++)
            for (int y = 0; y < gridSize; y++)
                sumSq += (saturationGrid[x, y] - mean) * (saturationGrid[x, y] - mean);

        float sigma = Mathf.Sqrt(sumSq / n);
        currentSigma = sigma;

        float uniformityScore = 1f / (1f + UniformityWeight * sigma);
        float saturationScore = mean; 
        FinalScore = Mathf.Clamp01(uniformityScore * saturationScore) * 100f;
    }

    private void UpdateLiveStats()
    {
    if (saturationGrid == null) return;

    float sum = 0f;
    int n = gridSize * gridSize;
    
    // 1. Calculate Mean
    for (int x = 0; x < gridSize; x++)
        for (int y = 0; y < gridSize; y++)
            sum += saturationGrid[x, y];

    float mean = sum / n;
    currentMeanSaturation = mean; // Updates the Inspector field

    // 2. Calculate Sigma (Standard Deviation)
    float sumSq = 0f;
    for (int x = 0; x < gridSize; x++)
        for (int y = 0; y < gridSize; y++)
            sumSq += (saturationGrid[x, y] - mean) * (saturationGrid[x, y] - mean);

    float sigma = Mathf.Sqrt(sumSq / n);
    currentSigma = sigma; // Updates the Inspector field
    }

   void OnDrawGizmos()
{
    // Only draw if grid exists
    if (saturationGrid == null || spriteRenderer == null) return;

    // 1. Get the ACTUAL world boundaries of the sprite (handles scale automatically)
    Bounds bounds = spriteRenderer.bounds;
    
    // 2. Calculate cell width/height based on those bounds
    float cellWidth = bounds.size.x / gridSize;
    float cellHeight = bounds.size.y / gridSize;

    // 3. Start drawing from the bottom-left corner (Min)
    Vector3 origin = bounds.min;
    // We need to shift right/up by half a cell to get the center of the first cell
    Vector3 startCenter = origin + new Vector3(cellWidth / 2f, cellHeight / 2f, 0);

    for (int x = 0; x < gridSize; x++)
    {
        for (int y = 0; y < gridSize; y++)
        {
            float wetness = saturationGrid[x, y];
            
            // Color: Red = Dry, Green = Wet
            Gizmos.color = Color.Lerp(new Color(1, 0, 0, 0.3f), new Color(0, 1, 0, 0.8f), wetness);

            // Calculate center position for this cell
            Vector3 centerPos = startCenter + new Vector3(x * cellWidth, y * cellHeight, 0);
            
            // Draw the box (slightly smaller than the cell to see grid lines)
            Gizmos.DrawCube(centerPos, new Vector3(cellWidth * 0.9f, cellHeight * 0.9f, 0.1f));
        }
    }
}
}