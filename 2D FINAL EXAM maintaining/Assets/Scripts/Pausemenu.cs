using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject pauseMenuPanel;
    public Slider volumeSlider;

    [Header("Audio Settings")]
    public AudioMixer mainMixer;

    [Header("Scene to load for Home button")]
    public string mainMenuSceneName = "mainmenu";

    private bool isPaused = false;

    void Start()
    {
        pauseMenuPanel.SetActive(false);

        if (volumeSlider != null)
        {
            // Match whatever volume was set on the main menu / settings screen
            float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
            volumeSlider.value = savedVolume;
            SetVolume(savedVolume);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    // Hook this up to the Back button's OnClick
    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    // Hook this up to the Home button's OnClick
    public void GoToMainMenu()
    {
        Time.timeScale = 1f; // reset so mainmenu isn't frozen when it loads
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // Hook this up to the Slider's OnValueChanged (float) - use the Dynamic float version
    public void SetVolume(float volume)
    {
        float db = Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20f;

        if (mainMixer == null)
        {
            Debug.LogError("[PauseMenu] mainMixer is not assigned in the Inspector!");
            return;
        }

        bool success = mainMixer.SetFloat("MasterVolume", db);
        Debug.Log($"[PauseMenu] SetVolume called with {volume:F2} ({db:F1} dB). SetFloat success: {success}");

        PlayerPrefs.SetFloat("MasterVolume", volume);
        PlayerPrefs.Save();
    }
}