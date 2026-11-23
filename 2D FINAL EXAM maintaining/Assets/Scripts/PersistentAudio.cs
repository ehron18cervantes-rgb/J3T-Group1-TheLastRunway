using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

[System.Serializable]
public class SceneMusicEntry
{
    public string sceneName;
    public AudioClip musicClip;
}

public class PersistentAudio : MonoBehaviour
{
    private static PersistentAudio instance;

    [Header("Assign the music clip for each scene by name")]
    public List<SceneMusicEntry> sceneMusicList;

    private AudioSource audioSource;
    private Dictionary<string, AudioClip> sceneMusicMap;
    private AudioClip currentClip;

    void Awake()
    {
        Debug.Log($"[PersistentAudio] Awake() running on GameObject '{gameObject.name}' (InstanceID: {GetInstanceID()})");

        // If an instance already exists, destroy this one so music doesn't double up
        if (instance != null)
        {
            Debug.LogWarning($"[PersistentAudio] Another instance already exists (InstanceID: {instance.GetInstanceID()}). " +
                $"Destroying this duplicate GameObject '{gameObject.name}'.");
            Destroy(gameObject);
            return;
        }

        instance = this;
        // This keeps the object alive when loading Level 2 or 3
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;

        // NOTE: volume is NOT handled here. MainMenuManager.SetVolume already
        // controls this via the AudioMixer's "MasterVolume" parameter, as long
        // as this AudioSource's Output is routed to that same Mixer Group.

        // Build a lookup table from the list you fill in the Inspector
        sceneMusicMap = new Dictionary<string, AudioClip>();
        foreach (var entry in sceneMusicList)
        {
            if (!string.IsNullOrEmpty(entry.sceneName) && !sceneMusicMap.ContainsKey(entry.sceneName))
                sceneMusicMap.Add(entry.sceneName, entry.musicClip);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        // Handles the very first scene (mainmenu) since sceneLoaded won't fire for it
        PlayMusicForScene(SceneManager.GetActiveScene().name);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForScene(scene.name);
    }

    void PlayMusicForScene(string sceneName)
    {
        Debug.Log($"[PersistentAudio] PlayMusicForScene called for scene: '{sceneName}'");

        if (!sceneMusicMap.TryGetValue(sceneName, out AudioClip clip) || clip == null)
        {
            Debug.LogWarning($"[PersistentAudio] No music clip mapped for scene '{sceneName}'. " +
                "Check that Scene Music List has an entry with this exact scene name.");
            return;
        }

        // Don't restart if the same track is already playing - keeps
        // lvl1interior -> lvl1street feeling seamless instead of chopping the music
        if (clip == currentClip && audioSource.isPlaying)
        {
            Debug.Log($"[PersistentAudio] '{clip.name}' already playing, skipping restart.");
            return;
        }

        currentClip = clip;
        audioSource.clip = clip;
        audioSource.Play();
        Debug.Log($"[PersistentAudio] Now playing '{clip.name}' for scene '{sceneName}'. " +
            $"isPlaying={audioSource.isPlaying}, volume={audioSource.volume}, mute={audioSource.mute}");
    }
}