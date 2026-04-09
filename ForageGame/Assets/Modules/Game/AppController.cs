using UnityEngine;
using System.IO;
using DG.Tweening;
using UnityEngine.UI;
using System;
using Project.Menus;
using Project.SceneLoading;
using TDK.SaveSystem;


[RequireComponent(typeof(SceneLoader))]
public class AppController : MonoBehaviour
{
    public enum State { MainMenu, Gameplay, Cutscene, Transitioning }
    public static AppController Instance { get; private set; }
    public SceneLoader sceneLoader { get; private set; }
    [SerializeField] public State state = State.Transitioning;
    [SerializeField] private SceneGroup initialSceneGroup = null;

    [Header("Important Scenes")]
    [SerializeField] private SceneInfo pauseScene;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        sceneLoader = GetComponent<SceneLoader>();
    }

    void Start()
    {
        if (initialSceneGroup == null) return;
        sceneLoader.FullLoadSceneGroup(initialSceneGroup);
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

    public void ToMainMenu()
    {
        SetGameState(State.Transitioning);
        SaveManager.Instance.SaveWorld();
        MenuManager.Instance.ToMenu(null, false);
        CutsceneManager.Instance.QuitToMainMenu(() => SetGameState(State.MainMenu));
    }

    public void ToCreditsSequence()
    {
        SetGameState(State.Transitioning);
        CutsceneManager.Instance.FinishGame(() =>
        {
            SetGameState(State.MainMenu);
            // TODO: directly open credits menu
        });
    }

    private readonly string[] _worldIds = { "1", "2", "3" };

    public void ToNewWorld(string worldId = null)
    {
        if (worldId == null || SaveServices.ExistsWorld(worldId))
            worldId = SaveServices.GetFreeWorldId(_worldIds);

        if (worldId == null)
        {
            Debug.LogWarning("Main: Cannot make any new worlds; ruh oh!.");
            return;
        }
        SaveManager.Instance.SelectWorld(worldId);
        CutsceneManager.Instance.PlayNewGame(() =>
            SetGameState(State.Gameplay));
    }

    public void ToWorld(string worldId = null)
    {
        worldId ??= PlayerPrefs.GetString("lastWorldUsed", null);
        if (worldId == null || !SaveServices.ExistsWorld(worldId))
        {
            ToNewWorld();
            return;
        }
        SaveManager.Instance.SelectWorld(worldId);
        CutsceneManager.Instance.PlayGame(() =>
        {
            SaveManager.Instance.LoadWorld();
            SetGameState(State.Gameplay);
        });
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

