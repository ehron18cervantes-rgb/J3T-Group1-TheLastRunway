using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;
    public GameObject applyButton; // The button that pops up when a change is made

    private Resolution[] resolutions;
    private int currentSelectedIndex;

    void Start()
    {
        if (resolutionDropdown == null)
        {
            Debug.LogError("[SettingsMenu] Resolution Dropdown is not assigned in the Inspector!");
            return;
        }

        // 1. Build a list of unique resolutions (Screen.resolutions repeats each size once per refresh rate)
        List<Resolution> unique = new List<Resolution>();
        foreach (Resolution r in Screen.resolutions)
        {
            if (unique.Count == 0 ||
                unique[unique.Count - 1].width != r.width ||
                unique[unique.Count - 1].height != r.height)
            {
                unique.Add(r);
            }
        }
        resolutions = unique.ToArray();

        if (resolutions.Length == 0)
        {
            Debug.LogWarning("[SettingsMenu] No screen resolutions were found.");
            if (applyButton != null) applyButton.SetActive(false);
            return;
        }

        // 2. Fill the dropdown and find the resolution that matches the current screen
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            options.Add(resolutions[i].width + " x " + resolutions[i].height);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);

        // 3. Load the saved resolution, or default to the current one
        int savedIndex = PlayerPrefs.GetInt("ResolutionIndex", currentResolutionIndex);
        savedIndex = Mathf.Clamp(savedIndex, 0, resolutions.Length - 1);

        currentSelectedIndex = savedIndex;

        // SetValueWithoutNotify avoids firing OnDropdownValueChange (and showing Apply) during setup
        resolutionDropdown.SetValueWithoutNotify(savedIndex);
        resolutionDropdown.RefreshShownValue();

        // 4. Apply the saved resolution on launch (only if the player saved one before)
        if (PlayerPrefs.HasKey("ResolutionIndex"))
        {
            Resolution saved = resolutions[savedIndex];
            Screen.SetResolution(saved.width, saved.height, Screen.fullScreenMode);
        }

        // 5. Hide the apply button when the menu starts
        if (applyButton != null) applyButton.SetActive(false);

        Debug.Log($"[SettingsMenu] Initialized. Current: {resolutions[savedIndex].width}x{resolutions[savedIndex].height}");
    }

    // LINK TO: ResolutionDropdown -> OnValueChanged (Int32)
    public void OnDropdownValueChange(int index)
    {
        if (resolutions == null || index < 0 || index >= resolutions.Length) return;

        currentSelectedIndex = index;

        // Show the Apply button because a change was made
        if (applyButton != null) applyButton.SetActive(true);

        Resolution selected = resolutions[index];
        Debug.Log($"[SettingsMenu] User selected: {selected.width}x{selected.height}. Click Apply to confirm.");
    }

    // LINK TO: ApplyButton -> OnClick ()
    public void ApplyResolution()
    {
        if (resolutions == null || currentSelectedIndex < 0 || currentSelectedIndex >= resolutions.Length)
        {
            Debug.LogError("[SettingsMenu] Apply FAILED - index out of range!");
            return;
        }

        Resolution res = resolutions[currentSelectedIndex];
        Screen.SetResolution(res.width, res.height, Screen.fullScreenMode);

        // Save the choice so it is remembered next time the game opens
        PlayerPrefs.SetInt("ResolutionIndex", currentSelectedIndex);
        PlayerPrefs.Save();

        Debug.Log($"[SettingsMenu] SUCCESS - Resolution applied: {res.width}x{res.height}");

        // Hide the button again after applying
        if (applyButton != null) applyButton.SetActive(false);
    }
}