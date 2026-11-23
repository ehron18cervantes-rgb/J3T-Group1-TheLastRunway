using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneSkip : MonoBehaviour
{
    [Header("Next Scene")]
    [Tooltip("Type the exact name of the scene to load after skipping")]
    public string nextSceneName = "lvl1interior";

    // Called when clicking the UI Button or pressing the key shortcut
    public void Skip()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nextSceneName);
    }

    void Update()
    {
        // Pressing Space or Escape will also trigger skip
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape))
        {
            Skip();
        }
    }
}