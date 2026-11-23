using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider volumeSlider;
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;
    public GameObject characterSelectPanel; // NEW

    [Header("Audio Settings")]
    public AudioMixer mainMixer;
    public AudioSource uiAudioSource;
    public AudioClip clickSound;

    private void Start()
    {
        mainMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);
        if (characterSelectPanel != null) characterSelectPanel.SetActive(false); // NEW

        CreateThirdCharacterButton();

        if (volumeSlider != null)
        {
            float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
            volumeSlider.value = savedVolume;
            SetVolume(savedVolume);
            Debug.Log($"[MainMenuManager] Volume loaded: {savedVolume:F2}");
        }
        else
        {
            Debug.LogWarning("[MainMenuManager] VolumeSlider is not assigned in the Inspector!");
        }
    }

    // --- MAIN MENU BUTTON FUNCTIONS ---

    // Start button now opens Character Select instead of loading the level directly
    public void OpenCharacterSelect()
    {
        mainMenuPanel.SetActive(false);
        characterSelectPanel.SetActive(true);
    }

    // Hook each character button to this, with a different index per button (0, 1, ...)
    public void SelectCharacterAndPlay(int characterIndex)
    {
        CharacterSelection.SetCharacter(characterIndex);
        Debug.Log($"[MainMenuManager] Character {characterIndex} selected. Loading game...");
        StartCoroutine(PlaySoundAndLoad());
    }

    IEnumerator PlaySoundAndLoad()
    {
        Debug.Log("[MainMenuManager] Sound starting...");

        if (uiAudioSource != null && clickSound != null)
        {
            uiAudioSource.loop = false;
            uiAudioSource.clip = clickSound;
            uiAudioSource.Play();
            yield return new WaitForSecondsRealtime(0.5f);
        }

        Debug.Log("[MainMenuManager] Loading scene now!");
        SceneManager.LoadScene(1);
    }

    public void OpenSettings()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    // Lets the player back out of Character Select to the main menu
    public void CloseCharacterSelect() // NEW
    {
        characterSelectPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    private void CreateThirdCharacterButton()
    {
        if (characterSelectPanel == null)
            return;

        // Do not create duplicates if this menu object is initialized more than once.
        if (characterSelectPanel.transform.Find("Character3Button") != null)
            return;

        Transform template = characterSelectPanel.transform.Find("Character2Button");
        if (template == null)
        {
            Debug.LogWarning("[MainMenuManager] Character2Button was not found; Character 3 could not be added.");
            return;
        }

        GameObject character3 = Instantiate(template.gameObject, characterSelectPanel.transform);
        character3.name = "Character3Button";

        RectTransform rect = character3.GetComponent<RectTransform>();
        if (rect != null)
            rect.anchoredPosition = new Vector2(400f, -266f);
            rect.localScale = new Vector3(1.2f, 1.2f, 1f); // 1.5x bigger

        Button button = character3.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => SelectCharacterAndPlay(2));
        }

        TMPro.TMP_Text label = character3.GetComponentInChildren<TMPro.TMP_Text>(true);
        if (label != null)
            label.text = "Character 2";

        Sprite preview = null;
        Sprite[] idleFrames = Resources.LoadAll<Sprite>("NewCharacter/idle");
        foreach (Sprite frame in idleFrames)
        {
            if (frame != null && frame.name.EndsWith("_0"))
            {
                preview = frame;
                break;
            }
        }
        if (preview == null && idleFrames.Length > 0)
            preview = idleFrames[0];

        if (preview != null)
        {
            Image[] images = character3.GetComponentsInChildren<Image>(true);
            foreach (Image image in images)
            {
                if (image.gameObject != character3)
                {
                    image.sprite = preview;
                    image.preserveAspect = true;
                    break;
                }
            }
        }
    }

    public void QuitGame()
    {
        Debug.Log("[MainMenuManager] Game is exiting...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // --- SETTINGS FUNCTIONS ---

    public void SetVolume(float volume)
    {
        float db = Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20f;
        mainMixer.SetFloat("MasterVolume", db);
        PlayerPrefs.SetFloat("MasterVolume", volume);
        PlayerPrefs.Save();
        Debug.Log($"[MainMenuManager] Volume set to {volume:F2} ({db:F1} dB)");
    }
}