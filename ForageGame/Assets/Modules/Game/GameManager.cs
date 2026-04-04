using UnityEngine;
using System.IO;
using DG.Tweening;
using UnityEngine.UI;
using System;
using Project.Menus;
using Project.SceneLoading;
using TDK.SaveSystem;

public enum GameState { MainMenu, PauseMenu, Gameplay, Cutscene, Transitioning }

[RequireComponent(typeof(SceneLoader))]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public SceneLoader sceneLoader { get; private set; }
    [SerializeField] public GameState state = GameState.Gameplay;
    [SerializeField] private SceneGroup initialSceneGroup;

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
        sceneLoader.FullLoadSceneGroup(initialSceneGroup);
    }

    // ------------ Transitions ------------

    public void QuitToDesktop()
    {
        SetGameState(GameState.Transitioning);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void QuitToMainMenu()
    {
        SetGameState(GameState.Transitioning);
        SaveManager.Instance.SaveWorld();
        MenuManager.Instance.ToMenu(null, false);
        CutsceneManager.Instance.QuitToMainMenu(() => SetGameState(GameState.MainMenu));
    }

    public void FinishGame()
    {
        SetGameState(GameState.Transitioning);
        SaveManager.Instance.SaveWorld();
        CutsceneManager.Instance.FinishGame(() =>
        {
            SetGameState(GameState.MainMenu);
            // TODO: directly open credits menu
        });
    }

    public void Sleep()
    {
        SetGameState(GameState.Transitioning);
        CutsceneManager.Instance.PlayGame(() =>
        {
            SaveManager.Instance.SaveWorld();
            SetGameState(GameState.Gameplay);
        });
    }

    public void Death()
    {
        SetGameState(GameState.Cutscene);
        // TODO: add duck falling and eating shit?
        CutsceneManager.Instance.PlayGame(() =>
        {
            SaveManager.Instance.SaveWorld();
            SetGameState(GameState.Gameplay);
        });
    }

    public void PauseGame()
    {
        SetGameState(GameState.Transitioning);
        sceneLoader.LoadScene(pauseScene, () => SetGameState(GameState.PauseMenu));
    }

    public void ResumeGame()
    {
        SetGameState(GameState.Transitioning);
        MenuManager.Instance.ToMenu(null, false);
        sceneLoader.UnloadScene(pauseScene, () => SetGameState(GameState.Gameplay));
    }

    private readonly string[] _worldIds = { "1", "2", "3" };

    public void PlayNewGame(string worldId = null)
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
            SetGameState(GameState.Gameplay));
    }

    public void PlayGame(string worldId = null)
    {
        worldId ??= PlayerPrefs.GetString("lastWorldUsed", null);
        if (worldId == null || !SaveServices.ExistsWorld(worldId))
        {
            PlayNewGame();
            return;
        }
        SaveManager.Instance.SelectWorld(worldId);
        CutsceneManager.Instance.PlayGame(() =>
        {
            SaveManager.Instance.LoadWorld();
            SetGameState(GameState.Gameplay);
        });
    }


    // ------------ Other Functions ------------

    private void SetGameState(GameState gameState)
    {
        state = gameState;

        switch (state)
        {
            case GameState.MainMenu:
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
            case GameState.PauseMenu:
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
            case GameState.Gameplay:
                Time.timeScale = 1f;
                // Cursor.lockState = CursorLockMode.Locked;
                // Cursor.visible = false;
                break;
            case GameState.Cutscene:
                Time.timeScale = 0f;
                break;
            case GameState.Transitioning:
                Time.timeScale = 0f;
                break;
        }
    }
}

