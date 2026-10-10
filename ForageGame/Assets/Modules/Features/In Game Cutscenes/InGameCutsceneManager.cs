using System.Threading.Tasks;
using NPC;
using TDK.CameraSystem;
using TDK.PlayerSystem;
using UnityEngine;

public class InGameCutsceneManager : MonoBehaviour
{
    public static InGameCutsceneManager Instance { get; private set; }
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    [SerializeField] private Animator _animator;
    [SerializeField] private CutoutController _cutoutController;
    [SerializeField] private Transform _cameraTarget;
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private TransitionScreenController _tsc;

    [Header("Animation action values")]
    [SerializeField] private Vector3 caveChuckPosition;
    [SerializeField] private Vector3 finalePosition;

    private bool _isPlaying;
    private bool _useFadeOnStop = false; // for the call back

    #region Cutscene controlls

    public void PlayCutscene(string cutsceneName, bool lockInputs, bool pauseTime, bool useFadeOnStart = false, bool useFadeOnStop = false)
    {
        if (_isPlaying)
        {
            Debug.LogWarning("Cannot start cutscene while a cutscene is playing.");
            return;
        }
        _isPlaying = true;
        _useFadeOnStop = useFadeOnStop;
        _ = GameplayController.Instance?.InGameCutsceneStart(_animator, cutsceneName, lockInputs, pauseTime, useFadeOnStart);
    }

    public void OnStateExit(bool useEndOnStop = false)
    {
        _ = GameplayController.Instance?.InGameCutsceneStop(_useFadeOnStop, useEndOnStop);
        _cutoutController.UseCutout(true, true);
        ResetCamera();
        _isPlaying = false;
    }

    #endregion

    #region Animation Controlls
    // Should only be used by the animator!
    public void ResetCamera()
    {
        _cameraController.SetPlayerTarget();
        _cameraController.TeleportToTarget();
    }
    public void SetCameraTarget()
    {
        _cameraController.SetTarget(_cameraTarget);
        _cameraController.TeleportToTarget();
    }
    public void FadeToBlack() => _tsc.FadeOut();
    public void FadeFromBlack() => _tsc.FadeIn();

    public void TeleportPlayerCaveChuck() => Player.Instance.playerController.TeleportTo(caveChuckPosition);
    public void TeleportPlayerFinale() => Player.Instance.playerController.TeleportTo(finalePosition);
    public void AddFlag(StoryFlag flag) => StoryFlagManager.Instance.AddFlag(flag);
    public void HideAllNPCs()
    {
        Debug.Log($"[InGameCutsceneManager]: hiding all npcs via camera culling mask");
        Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("NPC"));
        Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("Player"));
    }
    public void ShowAllNPCs()
    {
        Debug.Log($"[InGameCutsceneManager]: showing all npcs again via camera culling mask");
        Camera.main.cullingMask |= 1 << LayerMask.NameToLayer("NPC");
        Camera.main.cullingMask |= 1 << LayerMask.NameToLayer("Player");
    }
    #endregion
}
