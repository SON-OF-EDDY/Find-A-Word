using UnityEngine;

public class LoadWheelMain : MonoBehaviour
{
    public GameObject loadWheel;
    private float widthPercentage = 0.5f; // Horizontal center (50% of screen width)
    private float verticalOffset = 0.15f; // Offset from bottom (percentage of screen height)

    void Start()
    {
        PositionLoadWheel();
    }

    void PositionLoadWheel()
    {
        // Get the screen dimensions in world units
        float screenWidth = Camera.main.orthographicSize * 2 * Camera.main.aspect;
        float screenHeight = Camera.main.orthographicSize * 2;

        // Calculate the desired position
        Vector3 newPosition = loadWheel.transform.position;
        newPosition.x = (screenWidth * widthPercentage) - (screenWidth / 2); // Horizontal center
        newPosition.y = -(screenHeight / 2) + (screenHeight * verticalOffset); // Bottom of the screen with offset

        loadWheel.transform.position = newPosition;
    }
}

