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

    public enum State { Paused, Playing, Transitioning, Cutscene, InGameCutscene }
    public State _state { get; private set; } = State.Transitioning;
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
        SetGameState(State.Transitioning);
        SaveManager.Instance.SaveWorld();
        AppController.Instance.Quit();
    }

    public async Task QuitToMainMenu()
    {
        SetGameState(State.Transitioning);
        await UnloadWorld();
        await AppController.Instance.ToMainMenu();
    }

    public async Task FinishGame()
    {
        SetGameState(State.Transitioning);
        await UnloadWorld();
        await AppController.Instance.ToCreditsSequence();
    }

    private bool isSleeping_saftey = false;
    public async Task Sleep()
    {
        if (isSleeping_saftey) return;
        isSleeping_saftey = true;

        SetGameState(State.Transitioning);
        Player.Instance.playerController.IsSleeping(true);

        await UnloadWorld();

        if (!StoryFlagManager.Instance.FlagActive(firstNight)) // first night cutscene
        {
            await SceneServices.LoadScene(_cutscene);
            await AwaitSaftey();
            SetGameState(State.Cutscene);
            await AwaitPadding();
            await ImageCutsceneController.Instance.PlayFirstNightSequence();
            await AwaitPadding();
            SetGameState(State.Transitioning);
            await SceneServices.UnloadScene(_cutscene);
        }

        Player.Instance.playerController.IsSleeping(false);
        await LoadWorld();
        StoryFlagManager.Instance.AddFlag(this.firstNight);// it does not matter if we keep adding the flag after the first night, nothing will change: this is here
        Player.Instance.energy.TakeDamage(-9999);
        Player.Instance.energy.AddEnergy(9999);
        isSleeping_saftey = false;
    }

    private bool isDead_saftey = false;
    public async Task Death()
    {
        if (isDead_saftey) return; // you can't be double dead
        isDead_saftey = true;

        SetGameState(State.Transitioning);

        Player.Instance.animator.SetBool("isDead", true);
        await Task.Delay(Mathf.CeilToInt(0.5f * 1000)); //animation (1 sec.) (cut at 0.5 sec.)

        await UnloadWorld();

        await LoadWorld();
        Player.Instance.energy.TakeDamage(-9999);
        Player.Instance.energy.AddEnergy(9999);
        isDead_saftey = false;
    }

    public void Escape()
    {
        if (_state == State.Paused) PauseMenuController.Instance.Escape();
        else if (_state == State.Playing) _ = PauseGame();
    }

    public async Task PauseGame()
    {
        SetGameState(State.Transitioning);
        await SceneServices.LoadScene(_pauseScene);
        await AwaitSaftey();
        SetGameState(State.Paused);
    }

    public async Task ResumeGame()
    {
        SetGameState(State.Transitioning);
        await SceneServices.UnloadScene(_pauseScene);
        SetGameState(State.Playing);
    }

    public async Task LoadWorld(string worldId)
    {
        SetGameState(State.Transitioning);
        _saveManager.SelectWorld(worldId);
        await LoadWorld();
    }

    public async Task LoadWorld()
    {
        SetGameState(State.Transitioning);
        await SceneServices.LoadScene(_worldScene);
        await AwaitSaftey();
        SaveManager.Instance.LoadWorld();
        await AwaitSaftey();

        await AwaitPadding();
        _tsc.FadeIn();
        SetGameState(State.Playing);
    }

    public async Task UnloadWorld()
    {
        SetGameState(State.Transitioning);
        await _tsc.FadeOutAsync();
        await AwaitPadding();
        StoryFlagManager.Instance.OnTimePassing();
        SaveManager.Instance.SaveWorld();
        await SceneServices.UnloadScene(_worldScene);
        Player.Instance.ResetAnimator();
    }

    public async Task LoadDebug()
    {
        SetGameState(State.Transitioning);
        _saveManager.SelectWorld("-1");

        await AwaitPadding();
        _tsc.FadeIn();
        SetGameState(State.Playing);
    }

    public async Task InGameCutsceneStart(Animator cutsceneAnimator, string cutsceneName, bool lockInputs, bool pauseTime, bool useTransitionScreen)
    {
        SetGameState(State.InGameCutscene);
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
        SetGameState(State.Playing);
        if (useTransitionScreen)
        {
            await AwaitPadding();
            _tsc.FadeIn();
        }
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
            case State.Transitioning:
                AppController.Instance.SetInputsActive(false);
                Time.timeScale = 0f;
                break;
            case State.Cutscene:
                AppController.Instance.SetInputsActive(false);
                Time.timeScale = 0f;
                break;
            case State.InGameCutscene:
                break;
        }
    }
}