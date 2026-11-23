using UnityEngine;

public static class CharacterSelection
{
    private const string KEY = "SelectedCharacter";

    public static void SetCharacter(int index)
    {
        PlayerPrefs.SetInt(KEY, index);
        PlayerPrefs.Save();
    }

    public static int GetCharacter()
    {
        return PlayerPrefs.GetInt(KEY, 0); // 0 = default character if none picked yet
    }
}