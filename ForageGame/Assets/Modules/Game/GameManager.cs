using UnityEngine;
using System.IO;
using DG.Tweening;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(SceneLoader))]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private SaveData currentSaveData;
    private int currentSaveSlot = -1;
    public SceneLoader sceneLoader { get; private set; }
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
    }

    void Start()
    {
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
        SaveSystem.SetSaveFile(currentSaveSlot, currentSaveData);
    }

    public void QuitGame()
    {
        SaveGame();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
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

        // Fade to black
        cutsceneSeq.Append(blackOverlay.DOFade(1, 1))

        // Start loading scene (but don't activate yet)
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
            .Append(introImage1.DOFade(1, 0.5f).SetEase(Ease.InOutCubic))
            .AppendInterval(2)
            .Append(introImage1.DOFade(0, 0.5f).SetEase(Ease.InOutCubic))
            .Append(introImage2.DOFade(1, 0.5f).SetEase(Ease.InOutCubic))
            .AppendInterval(2)
            .Append(introImage2.DOFade(0, 0.5f).SetEase(Ease.InOutCubic))
            .Append(introImage3.DOFade(1, 0.5f).SetEase(Ease.InOutCubic))
            .AppendInterval(2)
            .Append(introImage2.DOFade(0, 0.5f).SetEase(Ease.InOutCubic));
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

        cutsceneSeq?.Kill();
        cutsceneSeq = DOTween.Sequence();

        cutsceneSeq.Append(blackOverlay.DOFade(0, 1));
    }
}