using UnityEngine;
using TMPro; // Needed for the UI text

public class CoffeeBedManager : MonoBehaviour
{
    // --- Configurable Variables ---
    [Header("Resolution & Physics")]
    [Tooltip("Higher number = Smaller squares. Try 64 for a smooth look.")]
    public int DesiredGridSize = 64; 
    
    [Tooltip("How fast a specific spot gets wet when hit.")]
    public float SpotPourRate = 50.0f; // Updated to 50 for realistic instant darkening
    
    [Tooltip("How fast the neighbor cells get wet.")]
    public float DiffusionRate = 0.2f; // Updated for better wicking

    [Tooltip("How fast the ENTIRE cup fills up/darkens when pouring anywhere.")]
    public float GlobalFillRate = 0.25f; // Updated for bloom timing

    // --- WEIGHT TRACKING ---
    [Header("Physics & Weight")]
    [Tooltip("How many grams of water are added per second when pouring.")]
    public float GramsPerSecond = 10.0f; 
    
    public float CurrentWaterInGrams { get; private set; } 

    // --- SCORING ---
    [Header("Scoring Settings")]
    public float UniformityWeight = 10f; 
    public float FinalScore { get; private set; }

    // --- DEBUGGING ---
    [Header("Live Stats")]
    [SerializeField] private float globalWaterLevel = 0f; 
    [SerializeField] private float currentMeanSaturation;
    
    [Header("Gesture Detection")]
    public PourTrajectoryAnalyzer patternAnalyzer; 

    // --- VISUALS ---
    [SerializeField]
    private Color WetColor = new Color(0.15f, 0.08f, 0.02f, 1.0f); 

    [Header("Visual Effects")]
    public BubbleManager bubbleManager; 
    [Range(0f, 1f)] 
    public float BubbleSpawnThreshold = 0.4f; 

    [Header("Ambient Bubbles (Seeping Effect)")]
    public float AmbientSpawnInterval = 0.1f; 
    public int AmbientCheckCount = 5; 
    private float ambientTimer = 0f;

    [Header("Draining & Absorption")]
    [Tooltip("How fast the standing water drains.")]
    public float DrainRate = 0.2f; 

    [Tooltip("How much wetness the grounds keep after water drains (0 to 1).")]
    public float GroundRetention = 0.6f;

    // --- PHASE MANAGEMENT (THE NEW STUFF) ---
    [Header("UI References")]
    public TextMeshProUGUI PhaseText; // Drag your UI Text here
    
    public enum CoffeePhase { Idle, BloomPour, BloomWait, MainPour, Drawdown, Finished }
    
    [Header("Phase State")]
    public CoffeePhase currentPhase = CoffeePhase.Idle;
    public string CurrentPhaseName; // For Inspector debug

    [Header("Phase Targets")]
    public float BloomTargetWeight = 40.0f; 
    public float BloomDuration = 30.0f;     
    public float FinalTargetWeight = 250.0f;
    
    private float stateTimer = 0.0f;

    // --- SCORING TRACKERS ---
    private int totalPourFrames = 0;
    private int goodTechniqueFrames = 0;
    private float maxSaturationDiff = 0f; // To track unevenness

    // --- PRIVATE INTERNAL ---
    private int gridSize; 
    private float[,] saturationGrid;
    private Texture2D coffeeTexture;         
    private SpriteRenderer spriteRenderer;
    private Color[] initialDryColors; 
    private int textureWidth;  
    private int textureHeight; 

    void Start()
    {
        InitializeTexture();

        // Auto-initialize analyzer if attached
        if(patternAnalyzer != null && spriteRenderer != null)
        {
            patternAnalyzer.Initialize(spriteRenderer.bounds.center);
        }
    }

    // --- CORE LOOP (STATE MACHINE) ---
    void Update()
    {
        // 1. Monitor Inputs
        bool isPouring = Input.GetMouseButton(0);
        
        // 2. State Machine Logic
        switch (currentPhase)
        {
            case CoffeePhase.Idle:
                UpdatePhaseUI("Ready to Brew");
                if (isPouring) ChangePhase(CoffeePhase.BloomPour);
                break;

            case CoffeePhase.BloomPour:
                UpdatePhaseUI($"Bloom: Pour to {BloomTargetWeight}g");
                
                ApplyPhysicsLogic();

                if (CurrentWaterInGrams >= BloomTargetWeight)
                {
                    ChangePhase(CoffeePhase.BloomWait);
                }
                break;

            case CoffeePhase.BloomWait:
                stateTimer += Time.deltaTime;
                float timeRemaining = Mathf.Max(0, BloomDuration - stateTimer);
                UpdatePhaseUI($"Blooming... Wait {timeRemaining:F0}s");

                ApplyPhysicsLogic(); 
                
                if (stateTimer >= BloomDuration)
                {
                    ChangePhase(CoffeePhase.MainPour);
                }
                break;

            case CoffeePhase.MainPour:
                UpdatePhaseUI($"Pour to {FinalTargetWeight}g");
                
                ApplyPhysicsLogic();

                if (CurrentWaterInGrams >= FinalTargetWeight)
                {
                    ChangePhase(CoffeePhase.Drawdown);
                }
                break;

            case CoffeePhase.Drawdown:
                UpdatePhaseUI("Waiting for Drawdown...");
                
                ApplyPhysicsLogic();

                // If water level is basically zero (drained)
                if (globalWaterLevel <= 0.01f)
                {
                    CalculateFinalScore();
                    ChangePhase(CoffeePhase.Finished);
                }
                break;

            case CoffeePhase.Finished:
                UpdatePhaseUI($"Done! Score: {FinalScore}");
                break;
        }
        
        CurrentPhaseName = currentPhase.ToString(); 
    }

    // --- HELPER FUNCTIONS ---

    private void ChangePhase(CoffeePhase newPhase)
    {
        currentPhase = newPhase;
        stateTimer = 0f; 
    }

    private void UpdatePhaseUI(string message)
    {
        if (PhaseText != null) PhaseText.text = message;
    }

    private void ApplyPhysicsLogic()
    {
        HandleDraining();
        DiffuseSaturation();
        UpdateVisuals();
        UpdateLiveStats();
        HandleAmbientBubbles();
    }

    // --- PHYSICS & LOGIC ---

    public void ApplyPour(Vector3 worldPourPosition, float flowStrength)
    {
        if (saturationGrid == null) return;

        // 1. CIRCULAR DETECTION CHECK
        Vector3 center = spriteRenderer.bounds.center;
        float radius = spriteRenderer.bounds.extents.x; 
        if (Vector3.Distance(worldPourPosition, center) > radius) return;

        // 2. WEIGHT TRACKING
        // CHANGE 2: Multiply by flowStrength
        float currentGramsPerSecond = GramsPerSecond * flowStrength;
        CurrentWaterInGrams += currentGramsPerSecond * Time.deltaTime;
        
        globalWaterLevel = Mathf.Clamp01(globalWaterLevel + (GlobalFillRate * flowStrength * Time.deltaTime));

        // --- NEW: TRAJECTORY TRACKING ---
        if (patternAnalyzer != null)
        {
            totalPourFrames++;
            string gesture = patternAnalyzer.GetGesture();
            
            // Give points for controlled circular motions or steady center pours
            if (gesture == "Circular Motion" || gesture == "Spot Pour / Center")
            {
                goodTechniqueFrames++;
            }
        }

        // 3. WETNESS LOGIC
        Bounds bounds = spriteRenderer.bounds;
        float nX = (worldPourPosition.x - bounds.min.x) / bounds.size.x;
        float nY = (worldPourPosition.y - bounds.min.y) / bounds.size.y;

        if (nX >= 0f && nX <= 1f && nY >= 0f && nY <= 1f) 
        {
            int gridX = Mathf.FloorToInt(nX * gridSize);
            int gridY = Mathf.FloorToInt(nY * gridSize);
            
            gridX = Mathf.Clamp(gridX, 0, gridSize - 1);
            gridY = Mathf.Clamp(gridY, 0, gridSize - 1);

            saturationGrid[gridX, gridY] = Mathf.Clamp01(saturationGrid[gridX, gridY] + (SpotPourRate * flowStrength) * Time.deltaTime);

            if (bubbleManager != null)
            {
                float effectiveWetness = Mathf.Max(saturationGrid[gridX, gridY], globalWaterLevel);
                if (effectiveWetness >= BubbleSpawnThreshold && IsGridCellValid(gridX, gridY))
                {
                    bubbleManager.TrySpawnBubble(worldPourPosition);
                }
            }
        }

        if (patternAnalyzer != null)
        {
            patternAnalyzer.AddSamplePoint(worldPourPosition);
        }
    }

    private void HandleDraining()
    {
        // 1. ABSORPTION
        if (globalWaterLevel > 0.01f)
        {
            float absorbedWetness = globalWaterLevel * GroundRetention;
            
            for (int x = 0; x < gridSize; x++)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    if (absorbedWetness > saturationGrid[x, y])
                    {
                        saturationGrid[x, y] = absorbedWetness;
                    }
                }
            }
        }

        // 2. DRAIN
        bool isPouring = Input.GetMouseButton(0);
        if (!isPouring && globalWaterLevel > 0)
        {
            globalWaterLevel -= DrainRate * Time.deltaTime;
            globalWaterLevel = Mathf.Max(0f, globalWaterLevel);
        }
    }

    private void DiffuseSaturation()
    {
        if (saturationGrid == null) return;
        float[,] next = (float[,])saturationGrid.Clone();

        for (int x = 1; x < gridSize - 1; x++)
        {
            for (int y = 1; y < gridSize - 1; y++)
            {
                float avg = (saturationGrid[x-1, y] + saturationGrid[x+1, y] + saturationGrid[x, y-1] + saturationGrid[x, y+1]) / 4f;
                float diff = (avg - saturationGrid[x, y]) * DiffusionRate;
                next[x, y] += diff;
            }
        }
        saturationGrid = next;
    }

    private void UpdateVisuals()
    {
        if (coffeeTexture == null) return;

        float blockW = (float)textureWidth / gridSize;
        float blockH = (float)textureHeight / gridSize;

        for (int gx = 0; gx < gridSize; gx++)
        {
            for (int gy = 0; gy < gridSize; gy++)
            {
                float localWetness = saturationGrid[gx, gy];
                float effectiveWetness = Mathf.Max(localWetness, globalWaterLevel);

                if (effectiveWetness <= 0.01f) continue;

                int startX = (int)(gx * blockW);
                int startY = (int)(gy * blockH);
                int endX = (int)((gx + 1) * blockW);
                int endY = (int)((gy + 1) * blockH);

                for (int x = startX; x < endX; x++)
                {
                    for (int y = startY; y < endY; y++)
                    {
                        if (x >= textureWidth || y >= textureHeight) continue;

                        int idx = y * textureWidth + x;
                        Color original = initialDryColors[idx];

                        if (original.a < 0.1f) continue;

                        coffeeTexture.SetPixel(x, y, Color.Lerp(original, WetColor, effectiveWetness));
                    }
                }
            }
        }
        coffeeTexture.Apply();
    }

    private void UpdateLiveStats()
    {
        if (saturationGrid == null) return;
        float sum = 0f;
        for (int x = 0; x < gridSize; x++)
            for (int y = 0; y < gridSize; y++)
                sum += saturationGrid[x, y];
        
        currentMeanSaturation = sum / (gridSize * gridSize);
    }

    private void HandleAmbientBubbles()
    {
        if (saturationGrid == null || bubbleManager == null) return;

        ambientTimer += Time.deltaTime;
        if (ambientTimer < AmbientSpawnInterval) return;
        ambientTimer = 0f;

        Bounds bounds = spriteRenderer.bounds;

        for (int i = 0; i < AmbientCheckCount; i++)
        {
            int randX = Random.Range(0, gridSize);
            int randY = Random.Range(0, gridSize);

            if (!IsGridCellValid(randX, randY)) continue;

            float localWetness = saturationGrid[randX, randY];
            float effectiveWetness = Mathf.Max(localWetness, globalWaterLevel);

            if (effectiveWetness >= BubbleSpawnThreshold)
            {
                float nX = (randX + 0.5f) / (float)gridSize; 
                float nY = (randY + 0.5f) / (float)gridSize;

                Vector3 spawnPos = new Vector3(
                    Mathf.Lerp(bounds.min.x, bounds.max.x, nX),
                    Mathf.Lerp(bounds.min.y, bounds.max.y, nY),
                    bounds.center.z
                );

                bubbleManager.TrySpawnBubble(spawnPos);
            }
        }
    }

    private void InitializeTexture()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null || spriteRenderer.sprite == null) return;

        Sprite initialSprite = spriteRenderer.sprite;
        textureWidth = (int)initialSprite.textureRect.width;
        textureHeight = (int)initialSprite.textureRect.height;

        if (initialSprite.texture.isReadable)
        {
            initialDryColors = initialSprite.texture.GetPixels(
                (int)initialSprite.textureRect.x, (int)initialSprite.textureRect.y, textureWidth, textureHeight);
        }
        else
        {
            Debug.LogError("Enable Read/Write on your Coffee Texture in Import Settings!");
            return;
        }

        coffeeTexture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false);
        coffeeTexture.filterMode = FilterMode.Bilinear;
        coffeeTexture.SetPixels(initialDryColors);
        coffeeTexture.Apply();

        spriteRenderer.sprite = Sprite.Create(coffeeTexture, new Rect(0, 0, textureWidth, textureHeight), Vector2.one * 0.5f, initialSprite.pixelsPerUnit);

        gridSize = Mathf.Min(DesiredGridSize, textureWidth);
        saturationGrid = new float[gridSize, gridSize];
    }

    private bool IsGridCellValid(int gridX, int gridY)
    {
        float nX = gridX / (float)gridSize;
        float nY = gridY / (float)gridSize;

        int texX = Mathf.FloorToInt(nX * textureWidth);
        int texY = Mathf.FloorToInt(nY * textureHeight);
        int index = texY * textureWidth + texX;

        if (index < 0 || index >= initialDryColors.Length) return false;
        return initialDryColors[index].a > 0.1f;
    }

    private void CalculateFinalScore()
    {
        // 1. WEIGHT SCORE (40% of Total)
        // Perfect if within 5g. Deduct points for every gram off.
        float weightError = Mathf.Abs(CurrentWaterInGrams - FinalTargetWeight);
        float weightScore = Mathf.Clamp(100f - weightError, 0f, 100f);

        // 2. SATURATION SCORE (40% of Total)
        // Check how evenly wet the bed is. We want a high Mean Saturation.
        float sumSaturation = 0f;
        int wetCells = 0;
        
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                // Only count cells that represent actual coffee (alpha > 0)
                if (IsGridCellValid(x, y)) 
                {
                    sumSaturation += saturationGrid[x, y];
                    wetCells++;
                }
            }
        }
        
        float averageSaturation = wetCells > 0 ? sumSaturation / wetCells : 0f;
        // Map 0.0-1.0 saturation to 0-100 score
        float saturationScore = averageSaturation * 100f; 

        // 3. TRAJECTORY SCORE (20% of Total)
        // Percentage of time spent using "Good" gestures vs "Bad/None"
        float trajectoryScore = 0f;
        if (totalPourFrames > 0)
        {
            trajectoryScore = ((float)goodTechniqueFrames / totalPourFrames) * 100f;
        }

        // 4. FINAL CALCULATION
        // Weighted Average: 40% Weight, 40% Saturation, 20% Technique
        FinalScore = (weightScore * 0.4f) + (saturationScore * 0.4f) + (trajectoryScore * 0.2f);
        
        // Round to 1 decimal place for neatness
        FinalScore = Mathf.Round(FinalScore * 10f) / 10f;

        Debug.Log($"Scoring Breakdown -- Weight: {weightScore}, Saturation: {saturationScore}, Tech: {trajectoryScore}");
    }
}