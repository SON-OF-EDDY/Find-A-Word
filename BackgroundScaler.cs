// File path: Assets/Scripts/BackgroundScaler.cs

using UnityEngine;

public class BackgroundScaler : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        // Find the GameObject with the "Background" tag
        GameObject backgroundObject = GameObject.FindGameObjectWithTag("Background");

        if (backgroundObject != null)
        {
            // Get the SpriteRenderer component from the background GameObject
            spriteRenderer = backgroundObject.GetComponent<SpriteRenderer>();
            AdjustBackgroundScale();
        }
        else
        {
            Debug.LogWarning("Background object with tag 'Background' not found.");
        }
    }

    void AdjustBackgroundScale()
    {
        if (spriteRenderer == null)
        {
            Debug.LogWarning("SpriteRenderer component not found.");
            return;
        }

        Debug.LogWarning("Background size has been adjusted based on screen size...");

        // Calculate the screen dimensions
        float screenWidth = Camera.main.orthographicSize * 2.0f * Screen.width / Screen.height;
        float screenHeight = Camera.main.orthographicSize * 2.0f;

        // Get the sprite dimensions
        float spriteWidth = spriteRenderer.sprite.bounds.size.x;
        float spriteHeight = spriteRenderer.sprite.bounds.size.y;

        // Calculate the scale factors for both width and height
        float scaleX = screenWidth / spriteWidth;
        float scaleY = screenHeight / spriteHeight;

        // Apply the larger scale factor to ensure the image covers the entire screen
        float scale = Mathf.Max(scaleX, scaleY);

        Vector3 newScale = transform.localScale;
        newScale.x = scale;
        newScale.y = scale;

        transform.localScale = newScale;
    }

}
