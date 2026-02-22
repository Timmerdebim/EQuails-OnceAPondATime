using UnityEngine;
using System.IO;
using DG.Tweening;
using UnityEngine.UI;
using System;
using Project.Menus;

public enum GameState { MainMenu, PauseMenu, Gameplay, Cutscene, Transitioning }

[RequireComponent(typeof(SceneLoader))]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public SaveData currentSaveData { get; private set; }
    private int currentSaveSlot = -1;
    public SceneLoader sceneLoader { get; private set; }
    public GameState state { get; private set; }
    [SerializeField] SceneGroup initialSceneGroup;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        sceneLoader = GetComponent<SceneLoader>();
        currentSaveData = new SaveData();
    }

    void Start()
    {
        if (initialSceneGroup == SceneGroup.MainMenu)
            SetGameState(GameState.MainMenu);
        else if (initialSceneGroup == SceneGroup.Pause)
            SetGameState(GameState.PauseMenu);
        else
            SetGameState(GameState.Gameplay);

        sceneLoader.FullLoadSceneGroup(initialSceneGroup);
    }

    public void SaveGame()
    {
        if (currentSaveData == null || currentSaveSlot < 0)
            return;

        currentSaveData.playerData = Player.Instance?.GetData();


        SaveSystem.SetSaveFile(currentSaveSlot, currentSaveData);
    }

    // ------------ Transitions ------------

    public void QuitToDesktop()
    {
        SetGameState(GameState.Transitioning);
        SaveGame();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void QuitToMainMenu()
    {
        SetGameState(GameState.Transitioning);
        SaveGame();
        MenuManager.Instance.ToMenu(null, false);
        CutsceneManager.Instance.QuitToMainMenu(() => SetGameState(GameState.MainMenu));
    }

    public void FinishGame()
    {
        SetGameState(GameState.Transitioning);
        SaveGame();
        CutsceneManager.Instance.FinishGame(() =>
        {
            SetGameState(GameState.MainMenu);
            // TODO: directly open credits menu
        });
    }

    public void PauseGame()
    {
        SetGameState(GameState.Transitioning);
        sceneLoader.LoadScenesByGroup(SceneGroup.Pause, () => { SetGameState(GameState.PauseMenu); });
    }

    public void ResumeGame()
    {
        SetGameState(GameState.Transitioning);
        MenuManager.Instance.ToMenu(null, false);
        sceneLoader.UnloadScenesByGroup(SceneGroup.Pause, () => { SetGameState(GameState.Gameplay); });
    }

    public void PlayNewGame(int slotIndex = -1)
    {
        if (slotIndex < 0)
        {
            slotIndex = 1; // First slot is 1 (not 0).
            while (SaveSystem.SaveFileExists(slotIndex) == true)
                slotIndex += 1; // Get the first free slot
        }
        currentSaveSlot = slotIndex;
        PlayerPrefs.SetInt("lastSlotIndexUsed", currentSaveSlot);
        PlayerPrefs.Save();
        currentSaveData = SaveSystem.GetSaveFile(currentSaveSlot);
        CutsceneManager.Instance.PlayNewGame();
    }

    public void PlayGame(int slotIndex = -1)
    {
        if (slotIndex < 0)
            slotIndex = PlayerPrefs.GetInt("lastSlotIndexUsed", -1);

        if (slotIndex < 0 || !SaveSystem.SaveFileExists(slotIndex))
        {
            // We instead start a new game if unable to load an old game
            PlayNewGame();
            return;
        }
        currentSaveSlot = slotIndex;
        PlayerPrefs.SetInt("lastSlotIndexUsed", currentSaveSlot);
        PlayerPrefs.Save();
        currentSaveData = SaveSystem.GetSaveFile(currentSaveSlot);
        CutsceneManager.Instance.PlayGame();
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
                break;
            case GameState.Transitioning:
                break;
        }
    }
}

