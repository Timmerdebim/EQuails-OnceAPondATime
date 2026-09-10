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

    private bool _isPlaying;

    #region Cutscene controlls

    public void PlayCutscene(string cutsceneName, bool lockInputs, bool useTransitionScreen)
    {
        if (_isPlaying)
        {
            Debug.LogWarning("Cannot start cutscene while a cutscene is playing.");
            return;
        }
        AppController.Instance.SetInputsActive(lockInputs);
        _ = GameplayController.Instance.InGameCutsceneStart(_animator, cutsceneName, useTransitionScreen);
    }

    public void StopCutscene(string cutsceneName) // for looping events
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
        if (GameplayController.Instance._state == GameplayController.State.Cutscene)
            _ = GameplayController.Instance.InGameCutsceneStop(false);
        AppController.Instance.SetInputsActive(true);
        _animator.ResetTrigger("Stop");
        ResetCamera();
        _isPlaying = false;
    }

    #endregion

    #region Animation Controlls
    // Should only be used by the animator!
    public void ResetCamera() => _cameraController.SetPlayerTarget();
    public void SetCameraTarget() => _cameraController.SetTarget(_cameraTarget);

    #endregion
}
