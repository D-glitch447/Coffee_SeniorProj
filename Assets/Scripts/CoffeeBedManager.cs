using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class CoffeeBedManager : MonoBehaviour
{
    // --- Configurable Variables ---
    [Header("Resolution & Physics")]
    public int DesiredGridSize = 64; 
    public float SpotPourRate = 50.0f; 
    public float DiffusionRate = 0.2f; 
    public float GlobalFillRate = 0.25f; 

    [Header("Physics & Weight")]
    public float GramsPerSecond = 10.0f; 
    public float CurrentWaterInGrams { get; private set; } 

    // --- SCORING SETTINGS ---
    [Header("Strict Scoring Settings")]
    public float WeightPenaltyPerGram = 2.0f;
    public float UnevennessPenalty = 20.0f;

    [Header("Time & Patience")]
    public float TargetTotalTime = 180.0f; // ideal brew time in seconds [ends when brew reaches target weight]
    public float TimeBuffer = 10.0f; // adjusts perfect window (i.e , no penalty if within ±10s)
    public float TimePenaltyPerSecond = 1.0f; // penalty rate beyond buffer
    public float ImpatiencePenaltyRate = 10.0f; // penalty per second during bloom wait

    [Header("Flow Rate Monitoring")]
    public float MaxHighFlowDuration = 2.0f;
    public float HighFlowPenaltyRate = 10.0f;
    
    // --- EXPOSED SCORES FOR RUNTIME ---
    public float FinalScore { get; private set; }
    public float LastWeightScore { get; private set; }
    public float LastSaturationScore { get; private set; }
    public float LastTechniqueScore { get; private set; }
    public float LastTimeScore { get; private set; }
    public float LastImpatiencePenalty { get; private set; }

    // --- DEBUGGING & VISUALS ---
    [Header("Live Stats")]
    [SerializeField] private float globalWaterLevel = 0f; 
    [SerializeField] private float currentMeanSaturation;
    public PourTrajectoryAnalyzer patternAnalyzer; 
    [SerializeField] private Color WetColor = new Color(0.15f, 0.08f, 0.02f, 1.0f); 
    public BubbleManager bubbleManager; 
    [Range(0f, 1f)] public float BubbleSpawnThreshold = 0.4f; 
    public float AmbientSpawnInterval = 0.1f; 
    public int AmbientCheckCount = 5; 
    private float ambientTimer = 0f;

    [Header("Draining & Absorption")]
    public float DrainRate = 0.2f; 
    public float GroundRetention = 0.6f;

    // --- PHASE MANAGEMENT ---
    [Header("UI References")]
    public TextMeshProUGUI PhaseText; 
    public TextMeshProUGUI TimerText; 

    

    [Header("Technique Settings")]
    // CHANGE 3: Add a threshold variable.
    [Range(0.1f, 1.0f)] public float TechniqueForgivenessThreshold = 0.9f;
    
    public enum CoffeePhase { Idle, BloomPour, BloomWait, MainPour, Drawdown, Finished }
    
    public CoffeePhase currentPhase = CoffeePhase.Idle;
    public float BloomTargetWeight = 40.0f; 
    public float BloomDuration = 30.0f;     
    public float FinalTargetWeight = 250.0f;
    private float stateTimer = 0.0f;

    // --- TRACKERS ---
    private int totalPourFrames = 0;
    private int goodTechniqueFrames = 0;
    private float currentHighFlowTimer = 0f;
    private float accumulatedFlowPenalty = 0f;
    
    // NEW TRACKERS
    public float brewTimer { get; private set; } = 0f;
    private bool isTimerRunning = false;
    private float accumulatedImpatiencePenalty = 0f;

    // --- INTERNAL ---
    private int gridSize; 
    private float[,] saturationGrid;
    private Texture2D coffeeTexture;         
    private SpriteRenderer spriteRenderer;
    private Color[] initialDryColors; 
    private int textureWidth;  
    private int textureHeight; 

    // Add a variable to store the required pattern for checking later
    private PourPattern targetPattern = PourPattern.Any;
    private bool needsGentlePour = false;

void Start()
{
    InitializeTexture();
    if(patternAnalyzer != null && spriteRenderer != null)
        patternAnalyzer.Initialize(spriteRenderer.bounds.center);

    // --- SYNC WITH RECIPE ---
    if (CoffeeRuntime.Instance != null && CoffeeRuntime.Instance.activeRecipe != null)
    {
        var r = CoffeeRuntime.Instance.activeRecipe;
        
        TargetTotalTime = r.brewTimeSeconds;
        FinalTargetWeight = r.waterWeightGrams;
        
        // Load the new technique requirements
        targetPattern = r.optimalPattern;
        needsGentlePour = r.requiresGentlePour;
    }
}

    void Update()
    {
        bool isPouring = Input.GetMouseButton(0);

        // TIMER LOGIC
        if (isTimerRunning)
        {
            brewTimer += Time.deltaTime;
            UpdateTimerUI();
        }
        
        // STATE MACHINE
        switch (currentPhase)
        {
            case CoffeePhase.Idle:
                UpdatePhaseUI("Ready to Brew");
                if (isPouring) 
                {
                    isTimerRunning = true; 
                    ChangePhase(CoffeePhase.BloomPour);
                }
                break;

            case CoffeePhase.BloomPour:
                UpdatePhaseUI($"Bloom: Pour to {BloomTargetWeight}g");
                ApplyPhysicsLogic();
                if (CurrentWaterInGrams >= BloomTargetWeight) ChangePhase(CoffeePhase.BloomWait);
                break;

            case CoffeePhase.BloomWait:
                stateTimer += Time.deltaTime;
                float timeRemaining = Mathf.Max(0, BloomDuration - stateTimer);
                UpdatePhaseUI($"Blooming... Wait {timeRemaining:F0}s");
                ApplyPhysicsLogic(); 
                if (stateTimer >= BloomDuration) ChangePhase(CoffeePhase.MainPour);
                break;

            case CoffeePhase.MainPour:
                UpdatePhaseUI($"Pour to {FinalTargetWeight}g");
                ApplyPhysicsLogic();
                if (CurrentWaterInGrams >= FinalTargetWeight) ChangePhase(CoffeePhase.Drawdown);
                break;

            case CoffeePhase.Drawdown:
                UpdatePhaseUI("Waiting for Drawdown...");
                ApplyPhysicsLogic();
                if (globalWaterLevel <= 0.01f)
                {
                    isTimerRunning = false;
                    CalculateFinalScore();
                    ChangePhase(CoffeePhase.Finished);
                }
                break;

            case CoffeePhase.Finished:
                UpdatePhaseUI($"Done! Score: {FinalScore}");
                break;
        }
    }

    // --- HELPERS ---
    private void UpdateTimerUI()
    {
        if (TimerText != null)
        {
            int minutes = Mathf.FloorToInt(brewTimer / 60F);
            int seconds = Mathf.FloorToInt(brewTimer % 60F);
            TimerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    private void ChangePhase(CoffeePhase newPhase)
    {
        currentPhase = newPhase;
        stateTimer = 0f; 
    }

    private void UpdatePhaseUI(string message) { if (PhaseText != null) PhaseText.text = message; }

    private void ApplyPhysicsLogic()
    {
        HandleDraining();
        DiffuseSaturation();
        UpdateVisuals();
        UpdateLiveStats();
        HandleAmbientBubbles();
    }

    // --- CORE LOGIC ---
    // --- CORE LOGIC ---
    public void ApplyPour(Vector3 worldPourPosition, float flowStrength)
    {
        if (saturationGrid == null) return;

        // 1. Basic Checks
        Vector3 center = spriteRenderer.bounds.center;
        float radius = spriteRenderer.bounds.extents.x; 
        if (Vector3.Distance(worldPourPosition, center) > radius) return;

        // 2. Penalties
        if (currentPhase == CoffeePhase.BloomWait)
        {
            accumulatedImpatiencePenalty += ImpatiencePenaltyRate * Time.deltaTime;
        }

        if (flowStrength > 0.9f)
        {
            currentHighFlowTimer += Time.deltaTime;
            if (currentHighFlowTimer > MaxHighFlowDuration)
            {
                accumulatedFlowPenalty += HighFlowPenaltyRate * Time.deltaTime;
            }
        }
        else
        {
            currentHighFlowTimer = 0f; 
        }

        // 3. Add Water to System
        float currentGramsPerSecond = GramsPerSecond * flowStrength;
        CurrentWaterInGrams += currentGramsPerSecond * Time.deltaTime;
        globalWaterLevel = Mathf.Clamp01(globalWaterLevel + (GlobalFillRate * flowStrength * Time.deltaTime));

        // 4. GRID PHYSICS (Wetness & Bubbles)
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

        // 5. TECHNIQUE ANALYZER (The Fix!)
        if (patternAnalyzer != null)
        {
            // [CRITICAL FIX] FEED DATA TO THE ANALYZER FIRST!
            patternAnalyzer.AddSamplePoint(worldPourPosition); 

            totalPourFrames++;
            string currentGesture = patternAnalyzer.GetGesture();

            // Check 1: Flow Rate
            bool isFlowGood = !needsGentlePour || (flowStrength < 0.95f || currentHighFlowTimer < MaxHighFlowDuration);

            // Check 2: Gesture Pattern
            bool isGestureGood = false;

            if (targetPattern == PourPattern.Any)
            {
                isGestureGood = (currentGesture == "Circular Motion" || currentGesture == "Spot Pour / Center");
            }
            else if (targetPattern == PourPattern.Circular)
            {
                isGestureGood = (currentGesture == "Circular Motion");
            }
            else if (targetPattern == PourPattern.Center)
            {
                isGestureGood = (currentGesture == "Spot Pour / Center");
            }

            // Only increment score if BOTH are correct
            if (isFlowGood && isGestureGood)
            {
                goodTechniqueFrames++;
            }
        }
    }
    private void HandleDraining()
    {
        if (globalWaterLevel > 0.01f)
        {
            float absorbedWetness = globalWaterLevel * GroundRetention;
            for (int x = 0; x < gridSize; x++)
                for (int y = 0; y < gridSize; y++)
                    if (absorbedWetness > saturationGrid[x, y]) saturationGrid[x, y] = absorbedWetness;
        }

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
                next[x, y] += (avg - saturationGrid[x, y]) * DiffusionRate;
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
                float eff = Mathf.Max(saturationGrid[gx, gy], globalWaterLevel);
                if (eff <= 0.01f) continue;

                int sx = (int)(gx * blockW), sy = (int)(gy * blockH);
                int ex = (int)((gx + 1) * blockW), ey = (int)((gy + 1) * blockH);

                for (int x = sx; x < ex; x++)
                {
                    for (int y = sy; y < ey; y++)
                    {
                        if (x >= textureWidth || y >= textureHeight) continue;
                        int idx = y * textureWidth + x;
                        Color orig = initialDryColors[idx];
                        if (orig.a < 0.1f) continue;
                        coffeeTexture.SetPixel(x, y, Color.Lerp(orig, WetColor, eff));
                    }
                }
            }
        }
        coffeeTexture.Apply();
    }

    private void UpdateLiveStats()
    {
        float sum = 0f;
        for (int x = 0; x < gridSize; x++) for (int y = 0; y < gridSize; y++) sum += saturationGrid[x, y];
        currentMeanSaturation = sum / (gridSize * gridSize);
    }

    private void HandleAmbientBubbles()
    {
        if (saturationGrid == null || bubbleManager == null) return;
        ambientTimer += Time.deltaTime;
        if (ambientTimer < AmbientSpawnInterval) return;
        ambientTimer = 0f;
        Bounds b = spriteRenderer.bounds;

        for (int i = 0; i < AmbientCheckCount; i++)
        {
            int rx = Random.Range(0, gridSize), ry = Random.Range(0, gridSize);
            if (!IsGridCellValid(rx, ry)) continue;
            if (Mathf.Max(saturationGrid[rx, ry], globalWaterLevel) >= BubbleSpawnThreshold)
            {
                Vector3 p = new Vector3(Mathf.Lerp(b.min.x, b.max.x, (rx + 0.5f)/gridSize), Mathf.Lerp(b.min.y, b.max.y, (ry + 0.5f)/gridSize), b.center.z);
                bubbleManager.TrySpawnBubble(p);
            }
        }
    }

    private void InitializeTexture()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null || spriteRenderer.sprite == null) return;
        Sprite s = spriteRenderer.sprite;
        textureWidth = (int)s.textureRect.width;
        textureHeight = (int)s.textureRect.height;
        if (s.texture.isReadable) initialDryColors = s.texture.GetPixels((int)s.textureRect.x, (int)s.textureRect.y, textureWidth, textureHeight);
        else { Debug.LogError("Enable Read/Write on Coffee Texture!"); return; }

        coffeeTexture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false);
        coffeeTexture.filterMode = FilterMode.Bilinear;
        coffeeTexture.SetPixels(initialDryColors);
        coffeeTexture.Apply();
        spriteRenderer.sprite = Sprite.Create(coffeeTexture, new Rect(0, 0, textureWidth, textureHeight), Vector2.one * 0.5f, s.pixelsPerUnit);
        gridSize = Mathf.Min(DesiredGridSize, textureWidth);
        saturationGrid = new float[gridSize, gridSize];
    }

    private bool IsGridCellValid(int gx, int gy)
    {
        int tx = Mathf.FloorToInt((gx/(float)gridSize) * textureWidth);
        int ty = Mathf.FloorToInt((gy/(float)gridSize) * textureHeight);
        int idx = ty * textureWidth + tx;
        if (idx < 0 || idx >= initialDryColors.Length) return false;
        return initialDryColors[idx].a > 0.1f;
    }

    // --- GRADING WITH TIME ---
    public void CalculateFinalScore()
{
    // 1. WEIGHT SCORE (30%)
    float weightError = Mathf.Abs(CurrentWaterInGrams - FinalTargetWeight);
    LastWeightScore = Mathf.Clamp(100f - (weightError * WeightPenaltyPerGram), 0f, 100f);

    // 2. SATURATION SCORE (30%)
    List<float> validCells = new List<float>();
    for (int x = 0; x < gridSize; x++)
        for (int y = 0; y < gridSize; y++)
            if (IsGridCellValid(x, y)) validCells.Add(saturationGrid[x, y]);
    
    float sum = 0f; foreach(float v in validCells) sum += v;
    float mean = validCells.Count > 0 ? sum / validCells.Count : 0f;
    
    // Simplified Saturation: If mean > 0.95 (mostly wet), give 100.
    LastSaturationScore = mean >= 0.95f ? 100f : (mean * 100f);

    // 3. TECHNIQUE SCORE (20%) - NEW CURVED LOGIC
    float rawTechniquePct = totalPourFrames > 0 ? (float)goodTechniqueFrames / totalPourFrames : 0f;
    
    // Apply Curve: Normalize so that 0.7 (70%) becomes 1.0 (100%)
    // This allows for 30% "bad frames" (jerky mouse starts/stops) without penalty.
    float curvedTechnique = rawTechniquePct / TechniqueForgivenessThreshold;
    
    LastTechniqueScore = Mathf.Clamp((curvedTechnique * 100f) - accumulatedFlowPenalty, 0f, 100f);

    // 4. TIME SCORE (20%)
    float timeDiff = Mathf.Abs(brewTimer - TargetTotalTime);
    float timeDeduction = 0f;
    
    // Only apply penalty if outside the larger buffer
    if (timeDiff > TimeBuffer)
    {
        timeDeduction = (timeDiff - TimeBuffer) * TimePenaltyPerSecond;
    }
    LastTimeScore = Mathf.Clamp(100f - timeDeduction, 0f, 100f);
    LastImpatiencePenalty = accumulatedImpatiencePenalty;

    // 5. FINAL CALCULATION
    float rawScore = (LastWeightScore * 0.3f) + 
                     (LastSaturationScore * 0.3f) + 
                     (LastTechniqueScore * 0.2f) + 
                     (LastTimeScore * 0.2f);
    
    FinalScore = Mathf.Max(0f, rawScore - accumulatedImpatiencePenalty);
    FinalScore = Mathf.Round(FinalScore * 10f) / 10f;

    Debug.Log($"Grading | Weight: {LastWeightScore} | Sat: {LastSaturationScore} | Tech: {LastTechniqueScore} (Raw: {rawTechniquePct:P0}) | Time: {LastTimeScore}");
}
}