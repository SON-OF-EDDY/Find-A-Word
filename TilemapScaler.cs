using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapScaler : MonoBehaviour
{
    public float tilemapScreenPercentage; // Percentage of screen the tilemap should occupy (50% by default)
    private Vector3 originalScale;
    public Tilemap myFindAWordTilemap;

    void Start()
    {
        // Store the original scale to reset later if needed
        //originalScale = transform.localScale;

        // Check if the device is an iPad-style device
        if (IsIpadStyleDevice())
        {
            // Call the scaling function initially
            ScaleTilemap();
            Debug.Log("iPad style device detected");
        } else
        {
            Debug.Log("Not an iPad style device");
        }
    }

    void Update()
    {
        // Check if the device is an iPad-style device and scale if the screen size changes
       /* if (IsIpadStyleDevice() && (Screen.width != Screen.currentResolution.width || Screen.height != Screen.currentResolution.height))
        {
            ScaleTilemap();
        }*/
    }

    void ScaleTilemap()
    {
        myFindAWordTilemap.transform.localScale = new Vector3(tilemapScreenPercentage, tilemapScreenPercentage, 1f);

        // Center the tilemap
        OffsetTilemap(0.3f, 0.7f);
    }

    private void OffsetTilemap(float offsetX, float offsetY)
    {
        // Get the current position of the tilemap
        Vector3 currentPosition = myFindAWordTilemap.transform.position;

        // Set the new position based on the offsets
        myFindAWordTilemap.transform.position = new Vector3(currentPosition.x + offsetX, currentPosition.y + offsetY, currentPosition.z);
    }

    bool IsIpadStyleDevice()
    {
        // Check if the screen width and height fit the iPad aspect ratio
        float aspectRatio = (float)Screen.width / Screen.height;

        // iPads typically have an aspect ratio around 4:3 or 3:2
        return (aspectRatio >= 1.3f && aspectRatio <= 1.5f) || (Screen.width == 810 && Screen.height == 1080);
    }
}
