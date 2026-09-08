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

    public void StartScriptedEvent(string cutsceneName, bool lockInputs)
    {
        if (_isPlaying)
        {
            Debug.LogWarning("Cannot start in game cutscene while an in game cutscene is playing.");
            return;
        }
        AppController.Instance.SetInputsActive(lockInputs);
        _animator.Play(cutsceneName);
    }

    public void StopScriptedEvent(string cutsceneName, bool lockInputs) // for looping events
    {
        _animator.SetTrigger("Stop");
    }

    public void PlayCutscene(string cutsceneName, bool useTransitionScreen)
    {
        if (_isPlaying)
        {
            Debug.LogWarning("Cannot start in game cutscene while an in game cutscene is playing.");
            return;
        }
        _ = GameplayController.Instance.InGameCutsceneStart(_animator, cutsceneName, useTransitionScreen);
    }

    public void OnStateExit()
    {
        if (GameplayController.Instance._state == GameplayController.State.Cutscene)
            _ = GameplayController.Instance.InGameCutsceneStop(false);
        AppController.Instance.SetInputsActive(true); // safety
        _animator.ResetTrigger("Stop");
        _isPlaying = false;
    }

    #endregion

    #region Animation Controlls
    // Should only be used by the animator!

    public void ResetCamera()
    {
        _cameraController.SetPlayerTarget();
    }

    public void SetCameraTarget()
    {
        _cameraController.SetTarget(_cameraTarget, Vector3.zero);
    }

    #endregion
}
