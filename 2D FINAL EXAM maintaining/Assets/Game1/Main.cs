using UnityEngine;
using UnityEngine.SceneManagement; // <-- Crucial: This is needed to change scenes!
using System.Collections;

public class Main : MonoBehaviour
{
    public static Main Instance;

    [Header("Victory Screen Setup")]
    // Drag your "You Win" GameObject here in the Unity Inspector
    public GameObject winPanel; 
    
    [Header("Scene Transition Settings")]
    // Type the exact name of your cutscene scene here in the inspector (e.g., FinalCutscene)
    public string cutsceneSceneName = "FinalCutscene"; 
    // How many seconds the victory screen stays on screen before loading the cutscene
    public float delayBeforeTransition = 3.5f; 

    private int onCount = 0;
    private bool gameEnded = false; // Prevents the transition from triggering multiple times

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // Keep the victory screen hidden when the minigame starts
        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }
    }

    public void SwitchChange(int points)
    {
        onCount += points;

        // Assuming there are 4 wires to connect total
        if (onCount >= 4 && !gameEnded)
        {
            gameEnded = true; // Locks it so it only runs once
            WinGame();
        }
    }

    private void WinGame()
    {
        Debug.Log("Game Won! Displaying Mission Accomplished Screen.");
        if (winPanel != null)
        {
            winPanel.SetActive(true); // Turns on your "You Win" canvas screen first!
        }

        // Starts a background timer to load the cutscene after a delay
        StartCoroutine(WaitAndLoadCutscene());
    }

    private IEnumerator WaitAndLoadCutscene()
    {
        // This pauses the code execution here for your set delay (e.g., 3.5 seconds)
        yield return new WaitForSeconds(delayBeforeTransition);

        Debug.Log("Transitioning to Final Cutscene!");
        // Loads your driving cutscene
        SceneManager.LoadScene(cutsceneSceneName);
    }
}