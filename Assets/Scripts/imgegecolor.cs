using UnityEngine;

public class imgegecolor : MonoBehaviour
{
    public SpriteRenderer spriteRenderer; // Assign your SpriteRenderer here
    public float radius = 20f;            // Brush radius in pixels
    public float darkenAmount = 0.5f;     // 0.5 = 50% darker

    public Vector2 offset = Vector2.zero; // Optional offset for brush position

    private Texture2D originalTexture;
    private Texture2D workingCopy;
    private Sprite originalSprite;

    

    void Start()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        originalSprite = spriteRenderer.sprite;
        originalTexture = originalSprite.texture;

        // 1. Create a writable copy of the texture
        // We use the texture width/height to ensure we get the full original data
        workingCopy = new Texture2D(originalTexture.width, originalTexture.height, TextureFormat.RGBA32, false);
        
        // 2. Copy the pixels
        // NOTE: This requires "Read/Write Enabled" on the texture import settings!
        workingCopy.SetPixels(originalTexture.GetPixels());
        workingCopy.Apply();

        // 3. Calculate the Normalized Pivot (0.0 to 1.0)
        // This fixes the "disappearing" issue
        Vector2 pivot = new Vector2(
            originalSprite.pivot.x / originalSprite.rect.width, 
            originalSprite.pivot.y / originalSprite.rect.height
        );

        // 4. Create the new Sprite with the CORRECT pivot
        Sprite newSprite = Sprite.Create(
            workingCopy, 
            originalSprite.rect, 
            pivot, // Use the calculated normalized pivot
            originalSprite.pixelsPerUnit
        );

        // 5. Assign to renderer
        spriteRenderer.sprite = newSprite;
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            // Convert mouse screen position to World Position
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPos.z = transform.position.z; // Ensure Z matches object

            // Convert World Position to Local Position (handles rotation/scale)
            Vector2 localPos = transform.InverseTransformPoint(worldPos) + (Vector3)offset;

            if (PointInsideSprite(localPos))
            {
                Vector2 uv = LocalPointToUV(localPos);
                DarkenArea(uv);
            }
        }
    }

    bool PointInsideSprite(Vector2 localPos)
    {
        // Get the bounds of the sprite in local units (pixels / PPU)
        float widthUnits = workingCopy.width / originalSprite.pixelsPerUnit;
        float heightUnits = workingCopy.height / originalSprite.pixelsPerUnit;

        // Calculate the local bounding box relative to the pivot
        // Note: localPos (0,0) is where the Pivot is.
        float pivotX = originalSprite.pivot.x / originalSprite.pixelsPerUnit;
        float pivotY = originalSprite.pivot.y / originalSprite.pixelsPerUnit;

        float minX = -pivotX;
        float maxX = widthUnits - pivotX;
        float minY = -pivotY;
        float maxY = heightUnits - pivotY;

        return localPos.x >= minX && localPos.x <= maxX &&
               localPos.y >= minY && localPos.y <= maxY;
    }

    Vector2 LocalPointToUV(Vector2 localPos)
    {
        // Convert local units back to actual pixel coordinates
        float pixelX = (localPos.x * originalSprite.pixelsPerUnit) + originalSprite.pivot.x;
        float pixelY = (localPos.y * originalSprite.pixelsPerUnit) + originalSprite.pivot.y;

        // Normalize to 0-1 UV range
        return new Vector2(pixelX / workingCopy.width, pixelY / workingCopy.height);
    }

    void DarkenArea(Vector2 uv)
    {
        int centerX = Mathf.RoundToInt(uv.x * workingCopy.width);
        int centerY = Mathf.RoundToInt(uv.y * workingCopy.height);

        int r = Mathf.RoundToInt(radius);

        // Optimization: Define the bounding box of the brush to avoid checking every pixel
        int minX = Mathf.Clamp(centerX - r, 0, workingCopy.width);
        int maxX = Mathf.Clamp(centerX + r, 0, workingCopy.width);
        int minY = Mathf.Clamp(centerY - r, 0, workingCopy.height);
        int maxY = Mathf.Clamp(centerY + r, 0, workingCopy.height);

        bool dirty = false; // Only Apply if we actually changed a pixel

        for (int x = minX; x < maxX; x++)
        {
            for (int y = minY; y < maxY; y++)
            {
                // Circle check
                if ((x - centerX) * (x - centerX) + (y - centerY) * (y - centerY) <= r * r)
                {
                    Color c = workingCopy.GetPixel(x, y);
                    
                    // Optional: Don't darken transparent pixels
                    if (c.a > 0) 
                    {
                        c.r *= (1f - darkenAmount);
                        c.g *= (1f - darkenAmount);
                        c.b *= (1f - darkenAmount);
                        // keeping alpha same
                        
                        workingCopy.SetPixel(x, y, c);
                        dirty = true;
                    }
                }
            }
        }

        if (dirty)
        {
            workingCopy.Apply();
        }
    }
}