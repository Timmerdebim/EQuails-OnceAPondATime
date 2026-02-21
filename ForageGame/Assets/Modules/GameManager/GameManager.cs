using UnityEngine;
using System.IO;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private SaveData currentSaveData;
    private int currentSaveSlot = -1;
    [SerializeField] private string gameSceneName;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
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
        if (currentSaveSlot < 0 || currentSaveData == null)
            return;
        SaveSystem.SetSaveFile(currentSaveSlot, currentSaveData);
    }

    // void OnEnable()
    // {
    //     SceneManager.sceneLoaded += OnSceneLoaded;
    // }

    // void OnDisable()
    // {
    //     SceneManager.sceneLoaded -= OnSceneLoaded;
    // }

    // void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    // {
    //     switch (scene.name)
    //     {
    //         case gameSceneName:
    //             InitMenu();
    //             break;

    //         case menuSceneName:
    //             LoadSaveData();
    //             break;
    //     }
    // }

    // ------------ Game Start ------------

    [Header("Cutscenes")]
    [SerializeField] private Image blackOverlay;
    [SerializeField] private Image introImage1;
    [SerializeField] private Image introImage2;
    [SerializeField] private Image introImage3;
    [SerializeField] private Image outroImage1;

    private Sequence cutsceneSeq;
    private AsyncOperation loadingOp;

    private void GameStart(bool isNewGame = false)
    {
        // cutsceneSeq?.Kill();
        // cutsceneSeq = DOTween.Sequence();

        // // Fade to black
        // cutsceneSeq.Append(blackOverlay.DOFade(1, 1))

        // // Start loading scene (but don't activate yet)
        // .AppendCallback(() =>
        // {
        //     loadingOp = SceneManager.LoadSceneAsync(gameSceneName);
        //     loadingOp.allowSceneActivation = false;
        //     loadingOp.completed += _ => DoThing();
        // })

        // // Wait until scene is loaded (progress >= 0.9)
        // .Append(
        //     DOTween.To(
        //         () => loadingOp.progress,
        //         x => { }, // no setter needed
        //         0.9f,
        //         10f // acts like a "max wait time" but finishes early
        //     ).SetEase(Ease.Linear)
        // );

        // // Activate scene once ready
        // seq.AppendCallback(() =>
        // {
        //     loadingOp.allowSceneActivation = true;
        // });

        // // Wait one frame so scene fully switches
        // seq.AppendInterval(0.1f);

        // // Run your logic AFTER load
        // seq.AppendCallback(() =>
        // {
        //     DoThing();
        // });

        // // Fade back in
        // seq.Append(fadeImage.DOFade(0f, fadeDuration));


    }

    private void OnGameStart()
    {

    }

    private void InitializeGame()
    {
        // TODO: Load game scene with saved data
    }
}