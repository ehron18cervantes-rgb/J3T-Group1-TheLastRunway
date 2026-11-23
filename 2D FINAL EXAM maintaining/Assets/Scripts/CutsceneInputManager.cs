using UnityEngine;
using UnityEngine.SceneManagement; // Crucial for switching scenes

public class CutsceneInputManager : MonoBehaviour
{
    [Header("Scene Navigation")]
    [Tooltip("The name of your main menu scene.")]
    public string mainMenuSceneName = "mainmenu"; 

    void Update()
    {
        // Press R to Restart the Cutscene
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartCutscene();
        }

        // Press Escape to Go to Main Menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GoToMainMenu();
        }
    }

    void RestartCutscene()
    {
        // Reloads the currently active scene (restarts the cutscene)
        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.name);
    }

    void GoToMainMenu()
    {
        // Double-check to make sure the string isn't empty
        if (string.IsNullOrEmpty(mainMenuSceneName))
        {
            mainMenuSceneName = "mainmenu";
        }

        // Loads your "mainmenu" scene
        SceneManager.LoadScene(mainMenuSceneName);
    }
}