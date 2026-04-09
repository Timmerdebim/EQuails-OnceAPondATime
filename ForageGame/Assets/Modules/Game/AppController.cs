using UnityEngine;
using System.IO;
using DG.Tweening;
using UnityEngine.UI;
using System;
using Project.Menus;
using Project.SceneLoading;
using TDK.SaveSystem;
using Eflatun.SceneReference;
using TDK.SceneSystem;
using System.Threading.Tasks;

public class AppController : MonoBehaviour
{
    public enum State { MainMenu, Gameplay, Cutscene, Transitioning }
    public static AppController Instance { get; private set; }
    [SerializeField] public State state = State.Transitioning;


    [Header("Scenes")]
    [SerializeField] private SceneReference _mainMenuScene;
    [SerializeField] private SceneReference _worldScene;
    [SerializeField] private ImageCutsceneController _cutscene;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
#if UNITY_EDITOR
        return;
#endif
        _ = ToMainMenu();
    }

    // ------------ Transitions ------------

    public void Quit()
    {
        SetGameState(State.Transitioning);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public async Task ToMainMenu()
    {
        SetGameState(State.Transitioning);
        await SceneServices.UnloadAllScenes();
        await SceneServices.LoadScene(_mainMenuScene);
        SetGameState(State.MainMenu);
    }

    public async Task ToCreditsSequence()
    {
        SetGameState(State.Transitioning);
        await SceneServices.UnloadAllScenes();
        await SceneServices.LoadScene(_mainMenuScene);
        SetGameState(State.MainMenu);
    }

    private readonly string[] _worldIds = { "1", "2", "3" };

    public async Task ToNewWorld(string worldId = null)
    {
        if (worldId == null || SaveServices.ExistsWorld(worldId))
            worldId = SaveServices.GetFreeWorldId(_worldIds);

        if (worldId == null)
        {
            Debug.LogWarning("Main: Cannot make any new worlds; ruh oh!.");
            return;
        }
        SaveManager.Instance.SelectWorld(worldId);
        await SceneServices.LoadScene(_worldScene);
        SetGameState(State.Gameplay);
    }

    public async Task ToWorld(string worldId = null)
    {
        worldId ??= PlayerPrefs.GetString("lastWorldUsed", null);
        if (worldId == null || !SaveServices.ExistsWorld(worldId))
        {
            await ToNewWorld();
            return;
        }
        SaveManager.Instance.SelectWorld(worldId);
        await SceneServices.LoadScene(_worldScene);
        SaveManager.Instance.LoadWorld();
        SetGameState(State.Gameplay);
    }

    // ------------ Other Functions ------------

    private void SetGameState(State newState)
    {
        state = newState;

        switch (state)
        {
            case State.MainMenu:
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
            case State.Gameplay:
                Time.timeScale = 1f;
                // Cursor.lockState = CursorLockMode.Locked;
                // Cursor.visible = false;
                break;
            case State.Cutscene:
                Time.timeScale = 0f;
                break;
            case State.Transitioning:
                Time.timeScale = 0f;
                break;
        }
    }
}

// public class MenuController : MonoBehaviour
// {
//     public void ToMainMenu()
//     {
//         SetGameState(State.Transitioning);
//         MenuManager.Instance.ToMenu(null, false);
//         CutsceneManager.Instance.QuitToMainMenu(() => SetGameState(State.MainMenu));
//     }
// }