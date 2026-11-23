using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [Header("Player Reference")]
    public Transform playerTransform;

    [Header("Spawn Locations")]
    public Transform defaultCarSpawnPoint; // For starting the game / main entry
    public Transform cinemaExitSpawnPoint;  // For exiting the cinema

    void Start()
    {
        // Fallback if player reference isn't assigned
        if (playerTransform == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) playerTransform = p.transform;
        }

        // Check how the player arrived in this scene
        if (SceneTransition.lastExitUsed == "Cinema" && cinemaExitSpawnPoint != null)
        {
            // Player just walked out of the cinema!
            playerTransform.position = cinemaExitSpawnPoint.position;
        }
        else if (defaultCarSpawnPoint != null)
        {
            // Default spawn (Game start or normal entry next to yellow car)
            playerTransform.position = defaultCarSpawnPoint.position;
        }

        // Clear the exit string after spawning
        SceneTransition.lastExitUsed = "";
    }
}