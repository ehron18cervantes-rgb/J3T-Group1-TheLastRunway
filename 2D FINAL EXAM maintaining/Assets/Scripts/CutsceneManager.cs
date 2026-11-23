using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement; // Essential for switching scenes

public class CutsceneManager : MonoBehaviour
{
    public PlayableDirector director;
    public string nextSceneName; // Type "lvl1street" here in the Inspector

    void Start()
    {
        if (director != null)
        {
            // Set the timeline to play as soon as this scene opens
            director.Play();
            
            // Listen for when the cutscene ends
            director.stopped += OnCutsceneFinished;
        }
    }

    void OnCutsceneFinished(PlayableDirector aDirector)
    {
        Debug.Log("Cutscene over! Loading gameplay...");
        
        // Unsubscribe to stay clean
        director.stopped -= OnCutsceneFinished;

        // Jump to the gameplay scene
        SceneManager.LoadScene(nextSceneName);
    }
    
    // Keep this for your Main Menu button to get here in the first place
    public void LoadThisCutscene(string sceneToLoad)
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}