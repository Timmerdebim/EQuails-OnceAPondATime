using System.Threading.Tasks;
using TDK.CameraSystem;
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
    [SerializeField] private Transform _cameraTarget;
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private TransitionScreenController _tsc;

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

    public void StopCutscene() // for looping events
    {
        if (!_isPlaying)
        {
            Debug.LogWarning("Cannot end cutscene while no cutscene is playing.");
            return;
        }
        _animator.SetTrigger("Stop");
    }

    public void OnStateExit()
    {
        _ = GameplayController.Instance?.InGameCutsceneStop(_useFadeOnStop);
        _animator.ResetTrigger("Stop");
        ResetCamera();
        _isPlaying = false;
    }

    #endregion

    #region Animation Controlls
    // Should only be used by the animator!
    public void ResetCamera() => _cameraController.SetPlayerTarget();
    public void SetCameraTarget() => _cameraController.SetTarget(_cameraTarget);
    public void FadeToBlack() => _tsc.FadeOut();
    public void FadeFromBlack() => _tsc.FadeIn();

    #endregion
}
