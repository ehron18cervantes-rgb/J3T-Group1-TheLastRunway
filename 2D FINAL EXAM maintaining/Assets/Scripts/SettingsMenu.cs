using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;
    public GameObject applyButton; // Reference to the button that pops up

    Resolution[] resolutions;
    int currentSelectedIndex;

    void Start()
    {
        // 1. Get system resolutions and clear placeholders
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        // 2. Build the list of resolution options
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            // Check which resolution matches the current screen
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);

        // 3. Load the saved resolution or default to current[cite: 1]
        int savedIndex = PlayerPrefs.GetInt("ResolutionIndex", currentResolutionIndex);
        savedIndex = Mathf.Clamp(savedIndex, 0, resolutions.Length - 1);

        currentSelectedIndex = savedIndex;
        resolutionDropdown.value = savedIndex;
        resolutionDropdown.RefreshShownValue();

        // 4. Hide the apply button when the game starts[cite: 1]
        if (applyButton != null) applyButton.SetActive(false);

        Debug.Log($"[SettingsMenu] Initialized. Current: {resolutions[savedIndex].width}x{resolutions[savedIndex].height}");
    }

    // LINK TO: ResolutionDropdown -> OnValueChanged (Int32)[cite: 1]
    public void OnDropdownValueChange(int index)
    {
        currentSelectedIndex = index;

        // Make the button pop up because a change was detected[cite: 1]
        if (applyButton != null) applyButton.SetActive(true);

        Resolution selected = resolutions[index];
        Debug.Log($"[SettingsMenu] User selected: {selected.width}x{selected.height}. Click Apply to confirm.");
    }

    // LINK TO: ApplyButton -> OnClick ()[cite: 1]
    public void ApplyResolution()
    {
        // Safety check to ensure index is valid[cite: 1]
        if (currentSelectedIndex < 0 || currentSelectedIndex >= resolutions.Length)
        {
            Debug.LogError("[SettingsMenu] Apply FAILED - index out of range!");
            return;
        }

        // Apply the physical resolution change[cite: 1]
        Resolution res = resolutions[currentSelectedIndex];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);

        // Save the choice so it stays the same next time you open the game[cite: 1]
        PlayerPrefs.SetInt("ResolutionIndex", currentSelectedIndex);
        PlayerPrefs.Save();

        Debug.Log($"[SettingsMenu] SUCCESS - Resolution applied: {res.width}x{res.height}");

        // Hide the button again after the setting is applied[cite: 1]
        if (applyButton != null) applyButton.SetActive(false);
    }
}