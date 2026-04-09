using UnityEngine;
using System.IO;
using DG.Tweening;
using UnityEngine.UI;
using System;
using Project.Menus;
using Project.SceneLoading;
using TDK.SaveSystem;

[RequireComponent(typeof(SceneLoader))]
public class GameplayController : MonoBehaviour
{
    public enum State { Paused, Playing, Transitioning }
    public static GameplayController Instance { get; private set; }
    public SceneLoader sceneLoader { get; private set; }
    [SerializeField] public State state = State.Transitioning;

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

    // ------------ Transitions ------------

    public void QuitToDesktop()
    {
        SetGameState(State.Transitioning);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void QuitToMainMenu()
    {
        SetGameState(State.Transitioning);
        SaveManager.Instance.SaveWorld();
        MenuManager.Instance.ToMenu(null, false);
        CutsceneManager.Instance.QuitToMainMenu();
    }

    public void FinishGame()
    {
        SetGameState(State.Transitioning);
        SaveManager.Instance.SaveWorld();
        CutsceneManager.Instance.FinishGame(() =>
        {
            // TODO: directly open credits menu
        });
    }

    public void Sleep()
    {
        SetGameState(State.Transitioning);
        CutsceneManager.Instance.PlayGame(() =>
        {
            SaveManager.Instance.SaveWorld();
            SetGameState(State.Playing);
        });
    }

    public void Death()
    {
        SetGameState(State.Transitioning);
        // TODO: add duck falling and eating shit?
        CutsceneManager.Instance.PlayGame(() =>
        {
            SaveManager.Instance.SaveWorld();
            SetGameState(State.Playing);
        });
    }

    public void Escape()
    {
        if (state == State.Paused) ResumeGame();
        else if (state == State.Playing) PauseGame();
    }

    public void PauseGame()
    {
        SetGameState(State.Transitioning);
        sceneLoader.LoadScene(pauseScene, () => SetGameState(State.Paused));
    }

    public void ResumeGame()
    {
        SetGameState(State.Transitioning);
        MenuManager.Instance.ToMenu(null, false);
        sceneLoader.UnloadScene(pauseScene, () => SetGameState(State.Playing));
    }

    // ------------ Other Functions ------------

    private void SetGameState(State gameState)
    {
        state = gameState;

        switch (state)
        {
            case State.Paused:
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
            case State.Playing:
                Time.timeScale = 1f;
                // Cursor.lockState = CursorLockMode.Locked;
                // Cursor.visible = false;
                break;
            case State.Transitioning:
                Time.timeScale = 0f;
                break;
        }
    }
}

