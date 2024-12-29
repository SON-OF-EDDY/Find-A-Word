using UnityEngine;

public class LoadWheelPositioner : MonoBehaviour
{
    public GameObject loadWheel;
    public float widthPercentage = 0.9f; // 90% of screen width

    void Start()
    {
        PositionLoadWheel();
    }

    void PositionLoadWheel()
    {
        // Get the screen width in world units
        float screenWidth = Camera.main.orthographicSize * 2 * Camera.main.aspect;

        // Calculate the desired position
        Vector3 newPosition = loadWheel.transform.position;
        newPosition.x = (screenWidth * widthPercentage) - (screenWidth / 2); // Aligns to 90% width

        loadWheel.transform.position = newPosition;
    }
}

