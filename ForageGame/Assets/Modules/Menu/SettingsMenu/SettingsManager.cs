using UnityEngine;

public static class SettingsManager
{
    // ------------ SAVE ------------

    public static void Save()
    {
        PlayerPrefs.Save();
    }

    // ------------ GRAPHICS ------------

    public static int ResolutionIndex
    {
        get => PlayerPrefs.GetInt("ResolutionIndex", 0);
        set => PlayerPrefs.SetInt("ResolutionIndex", value);
    }

    public static bool Fullscreen
    {
        get => PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        set => PlayerPrefs.SetInt("Fullscreen", value ? 1 : 0);
    }

    public static int QualityIndex
    {
        get => PlayerPrefs.GetInt("QualityIndex", 2);
        set => PlayerPrefs.SetInt("QualityIndex", value);
    }

    public static bool VSync
    {
        get => PlayerPrefs.GetInt("VSync", 1) == 1;
        set => PlayerPrefs.SetInt("VSync", value ? 1 : 0);
    }

    // ------------ AUDIO ------------

    public static float MasterVolume
    {
        get => PlayerPrefs.GetFloat("MasterVolume", 1f);
        set => PlayerPrefs.SetFloat("MasterVolume", value);
    }

    public static float MusicVolume
    {
        get => PlayerPrefs.GetFloat("MusicVolume", 1f);
        set => PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public static float SFXVolume
    {
        get => PlayerPrefs.GetFloat("SFXVolume", 1f);
        set => PlayerPrefs.SetFloat("SFXVolume", value);
    }
}
