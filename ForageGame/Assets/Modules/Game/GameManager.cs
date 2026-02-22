using UnityEngine;
using System.IO;
using DG.Tweening;
using UnityEngine.UI;
using System;
using Project.Menus;

public enum GameState { MainMenu, PauseMenu, Gameplay, Cutscene }

[RequireComponent(typeof(SceneLoader))]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public SaveData currentSaveData;
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

    public void NewGame(int slotIndex = -1)
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
        GameStart(true);
    }

    public void LoadGame(int slotIndex = -1)
    {
        if (slotIndex < 0)
            slotIndex = PlayerPrefs.GetInt("lastSlotIndexUsed", -1);

        if (slotIndex < 0 || !SaveSystem.SaveFileExists(slotIndex))
        {
            // We instead start a new game if unable to load an old game
            NewGame();
            return;
        }
        currentSaveSlot = slotIndex;
        PlayerPrefs.SetInt("lastSlotIndexUsed", currentSaveSlot);
        PlayerPrefs.Save();
        currentSaveData = SaveSystem.GetSaveFile(currentSaveSlot);
        GameStart();
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
        SaveGame();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    public void QuitToMainMenu()
    {
        SaveGame();
        MenuManager.Instance.ToMenu(null, false);
        GameManager.Instance.sceneLoader.FullLoadSceneGroup(SceneGroup.MainMenu, () => SetGameState(GameState.MainMenu));
    }

    public void PauseGame()
    {
        sceneLoader.LoadScenesByGroup(SceneGroup.Pause, () => { SetGameState(GameState.PauseMenu); });
    }

    public void ResumeGame()
    {
        MenuManager.Instance.ToMenu(null, false);
        sceneLoader.UnloadScenesByGroup(SceneGroup.Pause, () => { SetGameState(GameState.Gameplay); });
    }

    private void SetGameState(GameState gameState)
    {
        state = gameState;

        switch (state)
        {
            case GameState.MainMenu:
                // Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
            case GameState.PauseMenu:
                // Time.timeScale = 0f;
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
        }
    }

    // ------------ Game Start ------------

    [Header("Cutscenes")]
    [SerializeField] private Image blackOverlay;
    [SerializeField] private Image introImage1;
    [SerializeField] private Image introImage2;
    [SerializeField] private Image introImage3;
    [SerializeField] private Image outroImage1;

    private Sequence cutsceneSeq;

    private bool isIntroCutsceneDone;
    private bool isIntroLoadingDone;

    private void GameStart(bool isNewGame = false)
    {
        isIntroCutsceneDone = false;
        isIntroLoadingDone = false;

        cutsceneSeq?.Kill();
        cutsceneSeq = DOTween.Sequence();
        cutsceneSeq.SetEase(Ease.InOutCubic);

        // Fade to black
        cutsceneSeq.Append(blackOverlay.DOFade(1, 1))

        .AppendCallback(() =>
        {
            sceneLoader.FullLoadSceneGroup(SceneGroup.World, () =>
            {
                isIntroLoadingDone = true;
                GameStartContinue();
            });
        });

        if (isNewGame)
        {
            cutsceneSeq.AppendInterval(1)
            .Append(introImage1.DOFade(1, 0.5f))
            .AppendInterval(2)
            .Append(introImage1.DOFade(0, 0.5f))
            .Append(introImage2.DOFade(1, 0.5f))
            .AppendInterval(2)
            .Append(introImage2.DOFade(0, 0.5f))
            .Append(introImage3.DOFade(1, 0.5f))
            .AppendInterval(2)
            .Append(introImage3.DOFade(0, 0.5f))
            .AppendInterval(1);
        }

        cutsceneSeq.AppendCallback(() =>
        {
            isIntroCutsceneDone = true;
            GameStartContinue();
        });
    }

    private void GameStartContinue()
    {
        if (!(isIntroCutsceneDone && isIntroLoadingDone))
            return;

        SetGameState(GameState.Gameplay);

        cutsceneSeq?.Kill();
        cutsceneSeq = DOTween.Sequence();
        cutsceneSeq.SetEase(Ease.InOutCubic);

        cutsceneSeq.Append(blackOverlay.DOFade(0, 1));
    }
}

