using UnityEngine;
using Weather;
using TDK.PlayerSystem;

public class PlayerLanternController : MonoBehaviour
{
    [System.Serializable]
    private struct LanternPose
    {
        public Vector3 localPosition;   // offset from player, world-axis-aligned
        public Quaternion localRotation;
    }

    [Header("References")]
    [SerializeField] private ConfigurableJoint _handJoint;
    [SerializeField] private Light _light;
    [SerializeField] private Material playerMat; //TODO: materialpropertyblock instead
    [SerializeField] private Transform[] _visuals;

    [Header("Positioning")]
    [SerializeField] private LanternPose[] facingPoses = new LanternPose[4]; //front left, front right, back left, back right
    [SerializeField] private LanternPose _retractedPose = new() { localPosition = Vector3.zero, localRotation = Quaternion.identity };
    [SerializeField] private float deployDuration = 0.4f; // seconds to fully deploy/retract
    [SerializeField] private float _lerpPosSpeed = 1;
    [SerializeField] private float _lerpRotSpeed = 1;

    private int _currentFacingIndex = 0; //front left, front right, back left, back right
    private enum State { Depolyed, Retracting, Retracted, Deploying }
    [SerializeField] private State _state = State.Retracted;
    private float _deployProgress = 0f; // 0 = retracted, 1 = deployed

    void Start()
    {
        Player.Instance.visuals.onFacingDirectionChanged.AddListener(OnFacingDirectionChanged);
        SetState(State.Retracted);
    }

    void OnValidate()
    {
        SetState(_state);
    }

    public void SetDeployment(bool isDeployed)
    {
        if (!Player.Instance.playerData.lanternUnlocked) return; //easy way to only have it work when unlocked

        if (isDeployed)//WeatherManager will continuously set this during transitions, so only do something when we need to
        {
            if (_state != State.Depolyed && _state != State.Deploying)
                SetState(State.Deploying);
        }
        else
        {
            if (_state != State.Retracted && _state != State.Retracting)
                SetState(State.Retracting);
        }
    }

    private void OnFacingDirectionChanged(bool isFacingLeft, bool isFacingFront)
    {
        _currentFacingIndex = (isFacingLeft ? 0 : 1) + (isFacingFront ? 0 : 2);
    }

    void FixedUpdate()
    {
        switch (_state)
        {
            case State.Depolyed:
                DeltaHandJointRefresh(facingPoses[_currentFacingIndex], Time.fixedDeltaTime);
                return;
            case State.Retracting:
                DeltaHandJointRefresh(_retractedPose, Time.fixedDeltaTime);
                _deployProgress = Mathf.MoveTowards(_deployProgress, 0, Time.fixedDeltaTime / deployDuration);
                DeltaDeploymentRefresh();
                if (_deployProgress < 0.01f) SetState(State.Retracted);
                return;
            case State.Retracted:
                return;
            case State.Deploying:
                DeltaHandJointRefresh(facingPoses[_currentFacingIndex], Time.fixedDeltaTime);
                _deployProgress = Mathf.MoveTowards(_deployProgress, 1, Time.fixedDeltaTime / deployDuration);
                DeltaDeploymentRefresh();
                if (_deployProgress > 0.99f) SetState(State.Depolyed);
                return;
        }
    }

    private void DeltaHandJointRefresh(LanternPose lanternPose, float delta)
    {
        if (_handJoint != null)
        {
            _handJoint.connectedAnchor = Vector3.MoveTowards(_handJoint.connectedAnchor, lanternPose.localPosition, _lerpPosSpeed * delta);
            _handJoint.targetRotation = Quaternion.RotateTowards(_handJoint.targetRotation, lanternPose.localRotation, _lerpRotSpeed * delta);
        }
    }

    private void DeltaDeploymentRefresh()
    {
        _handJoint.anchor = Mathf.SmoothStep(0, 1, _deployProgress) * Vector3.up;
        foreach (Transform visual in _visuals)
            visual.localScale = Mathf.SmoothStep(0, 1, _deployProgress) * Vector3.one;
    }

    private void SetState(State state)
    {
        _state = state;
        switch (_state)
        {
            case State.Depolyed:
                _deployProgress = 1;
                DeltaDeploymentRefresh();
                break;
            case State.Retracting:
                break;
            case State.Retracted:
                DeltaHandJointRefresh(_retractedPose, 999);
                _deployProgress = 0;
                DeltaDeploymentRefresh();
                break;
            case State.Deploying:
                return;
        }
    }

    #region Visuals

    [Header("Lantern Settings")]
    [SerializeField] private Color mutedColor = Color.red;
    [SerializeField] private Color BrightColor = Color.yellow;
    [SerializeField] private float minIntensity = 0f;
    [SerializeField] private float maxIntensity = 5f;
    [SerializeField][Range(0f, 10f)] private float flickerSpeed = 1f;
    [SerializeField] private Vector3 playerMatNegativeEmission;
    [SerializeField] FBM1D fbm = new FBM1D(FBM1D.NoiseFunctionType.Sin, 4, 1.97f, 0.43f);
    private float _lanternStrength = 0.5f;

    private void Update()
    {
        if (_deployProgress > 0.01f)
        {
            _lanternStrength = fbm.Eval01(Time.time * flickerSpeed);
            _light.color = Color.Lerp(mutedColor, BrightColor, _lanternStrength);
            _light.intensity = Mathf.Lerp(minIntensity * _deployProgress, maxIntensity * _deployProgress, _lanternStrength);
        }
        else _light.intensity = 0f;

        //needs to happen in update, since the player might just walk out of a dark region
        // playerMat.SetVector("_Emission", _currentFacingIndex <= 1 ? playerMatNegativeEmission * _deployProgress : Vector3.zero);
    }

    #endregion
}
