using UnityEngine;

public class WireGameManager : MonoBehaviour
{
    [Header("UI Panels")]
    // Drag your "You Win" GameObject here in the inspector
    public GameObject winPanel; 

    void Start()
    {
        // Make sure the victory screen is hidden when the mini-game starts
        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }
    }

    // Call this function when all wires are correctly connected
    public void CompleteWireGame()
    {
        if (winPanel != null)
        {
            winPanel.SetActive(true); // This makes the Mission Accomplished image appear!
        }
    }
}