using UnityEngine;
using Project.Menus;
using TDK.SaveSystem;
using System.Threading.Tasks;
using TDK.SceneSystem;
using Eflatun.SceneReference;
using TDK.PlayerSystem;

public class GameplayController : MonoBehaviour
{
    public static GameplayController Instance { get; private set; }

    public enum State { Paused, Playing, Busy, Initial }
    public State _state { get; private set; } = State.Initial;
    [SerializeField] private TransitionScreenController _tsc;
    [SerializeField] public SaveManager _saveManager;

    [Header("Scenes")]
    [SerializeField] private SceneReference _worldScene;
    [SerializeField] private SceneReference _pauseScene;
    [SerializeField] private SceneReference _cutscene;

    [Header("Story Flags")]
    [SerializeField] private StoryFlag firstNight;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // ------------ Transitions ------------

    public void QuitToDesktop()
    {
        SetGameState(State.Busy);
        SaveManager.Instance.SaveWorld();
        AppController.Instance.Quit();
    }

    public async Task QuitToMainMenu()
    {
        if (_state == State.Busy) return;
        SetGameState(State.Busy);
        await UnloadWorld();
        await AppController.Instance.ToMainMenu();
    }

    public async Task FinishGame()
    {
        if (_state == State.Busy) return;
        SetGameState(State.Busy);
        await UnloadWorld();
        await AppController.Instance.ToCreditsSequence();
    }

    public async Task Sleep()
    {
        if (_state == State.Busy) return;
        SetGameState(State.Busy);
        Debug.Log("[Gameplay] Player sleeping.");

        Player.Instance.playerController.IsSleeping(true);

        await UnloadWorld();

        if (!StoryFlagManager.Instance.FlagActive(firstNight)) // first night cutscene
        {
            await SceneServices.LoadScene(_cutscene);
            await AwaitPadding();
            await ImageCutsceneController.Instance.PlayFirstNightSequence();
            await AwaitPadding();
            await SceneServices.UnloadScene(_cutscene);
        }

        Player.Instance.playerController.IsSleeping(false);
        await LoadWorld();
        StoryFlagManager.Instance.AddFlag(this.firstNight);// it does not matter if we keep adding the flag after the first night, nothing will change: this is here
        Player.Instance.energy.TakeDamage(-9999);
        Player.Instance.energy.AddEnergy(9999);
        SetGameState(State.Playing);
        Debug.Log("[Gameplay] Player slept.");

    }

    public async Task Death()
    {
        if (_state == State.Busy) return;
        SetGameState(State.Busy);
        Debug.Log("[Gameplay] Player dying.");


        Player.Instance._playerAnimator.IsDead(true);
        await Task.Delay(Mathf.CeilToInt(0.5f * 1000)); //animation (1 sec.) (cut at 0.5 sec.)

        await UnloadWorld();
        await LoadWorld();

        Player.Instance.energy.TakeDamage(-9999);
        Player.Instance.energy.AddEnergy(9999);
        SetGameState(State.Playing);
        Debug.Log("[Gameplay] Player died.");
    }

    private bool _isCutsceneActive = false;

    public async Task InGameCutsceneStart(Animator cutsceneAnimator, string cutsceneName, bool lockInputs, bool pauseTime, bool useTransitionScreen)
    {
        if (_state == State.Busy) return;
        SetGameState(State.Busy);
        _isCutsceneActive = true;
        AppController.Instance.SetInputsActive(lockInputs);
        Time.timeScale = pauseTime ? 0 : 1;
        if (useTransitionScreen)
        {
            await _tsc.FadeOutAsync();
            await AwaitPadding();
        }
        cutsceneAnimator.Play(cutsceneName);
    }

    public async Task InGameCutsceneStop(bool useTransitionScreen)
    {
        if (!_isCutsceneActive) return;
        _isCutsceneActive = false;
        SetGameState(State.Playing);
        if (useTransitionScreen)
        {
            await AwaitPadding();
            _tsc.FadeIn();
        }
    }

    public void Escape()
    {
        if (_state == State.Paused) PauseMenuController.Instance.Escape();
        else if (_state == State.Playing) _ = PauseGame();
    }

    public async Task PauseGame()
    {
        if (_state == State.Busy) return;
        SetGameState(State.Busy);
        Debug.Log("[Gameplay] Pausing game.");
        await SceneServices.LoadScene(_pauseScene);
        await AwaitSaftey();
        SetGameState(State.Paused);
        Debug.Log("[Gameplay] Paused game.");
    }

    public async Task ResumeGame()
    {
        if (_state == State.Busy) return;
        SetGameState(State.Busy);
        Debug.Log("[Gameplay] Resuming game.");
        await SceneServices.UnloadScene(_pauseScene);
        SetGameState(State.Playing);
        Debug.Log("[Gameplay] Resumed game.");
    }

    public async Task LoadWorld(string worldId = null)
    {
        if (_state == State.Busy) return;
        SetGameState(State.Busy);
        if (worldId != null) _saveManager.SelectWorld(worldId);
        await LoadWorld();
        _tsc.FadeIn();
        SetGameState(State.Playing);
    }

    public async Task LoadDebug()
    {
        if (_state == State.Busy) return;
        SetGameState(State.Busy);
        _saveManager.SelectWorld("-1");
        await AwaitPadding();
        _tsc.FadeIn();
        SetGameState(State.Playing);
    }

    // PRIVATE TASKS

    private async Task LoadWorld() // PRIVATE == skip saftey check
    {
        Debug.Log("[Gameplay] Loading World.");
        await SceneServices.LoadScene(_worldScene);
        await AwaitSaftey();
        SaveManager.Instance.LoadWorld();
        await AwaitSaftey();
        await AwaitPadding();
        _tsc.FadeIn();
        Debug.Log("[Gameplay] Loaded World.");
    }

    private async Task UnloadWorld() // PRIVATE == skip saftey check
    {
        Debug.Log("[Gameplay] Unloading World.");
        await _tsc.FadeOutAsync();
        await AwaitPadding();
        StoryFlagManager.Instance.OnTimePassing();
        SaveManager.Instance.SaveWorld();
        await SceneServices.UnloadScene(_worldScene);
        Player.Instance.ResetAnimator();
        Debug.Log("[Gameplay] Unloaded World.");
    }

    // ------------ Other Functions ------------

    private async Task AwaitPadding()
    {
        await Task.Delay(Mathf.CeilToInt(500)); // time padding
        await AwaitSaftey();
    }

    private async Task AwaitSaftey()
    {
        await Task.Yield(); // Current Frame
        await Task.Yield(); // Awake Saftey
        await Task.Yield(); // Start Saftey
        await Task.Yield(); // Physics Saftey
        await Task.Yield(); // BONUS Saftey (you never know)
    }

    private void SetGameState(State gameState)
    {
        _state = gameState;

        switch (_state)
        {
            case State.Paused:
                Time.timeScale = 0f;
                AppController.Instance.SetInputsActive(true);
                // Cursor.lockState = CursorLockMode.None;
                // Cursor.visible = true;
                break;
            case State.Playing:
                AppController.Instance.SetInputsActive(true);
                Time.timeScale = 1f;
                // Cursor.lockState = CursorLockMode.Locked;
                // Cursor.visible = false;
                break;
            case State.Busy:
                AppController.Instance.SetInputsActive(false);
                Time.timeScale = 0f;
                break;
        }
    }
}