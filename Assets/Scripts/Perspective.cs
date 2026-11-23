using UnityEngine;

public class PerspectiveScale : MonoBehaviour
{
    // Adjust these in the Inspector to match your scene
    public float minYPos = -1.5f; // Position at the door
    public float maxYPos = -4.0f; // Position at the bottom of screen
    
    public float minScale = 0.4f; // Small size at the door
    public float maxScale = 1.0f; // Normal size at the front

    void Update()
    {
        // 1. Calculate where the player is between the min and max Y points (0 to 1 range)
        float t = Mathf.InverseLerp(minYPos, maxYPos, transform.position.y);

        // 2. Calculate the new scale based on that percentage
        float currentScale = Mathf.Lerp(minScale, maxScale, t);

        // 3. Apply the scale to the player
        transform.localScale = new Vector3(currentScale, currentScale, 1f);
    }
}