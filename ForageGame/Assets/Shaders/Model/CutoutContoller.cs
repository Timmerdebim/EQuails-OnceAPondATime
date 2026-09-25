using System;
using System.Collections;
using TDK.CameraSystem;
using UnityEngine;

public class CutoutController : MonoBehaviour
{
    [SerializeField] private CameraController _camera;
    [SerializeField] private LayerMask _cutoutLayers;

    [SerializeField] private Material baseMat;
    [SerializeField] private float _speed;
    [Header("Cutout Presets")]
    // CameraOuterRadius => Vector.x;
    // CameraInnerRadius => Vector.y;
    // PlayerOuterRadius => Vector.z;
    // PlayerInnerRadius => Vector.w;
    [SerializeField] private Vector4 standardOff;
    [SerializeField] private Vector4 standardOn;
    [SerializeField] private Vector4 caveOff;
    [SerializeField] private Vector4 caveOn;

    private bool _isActive = false;
    public enum CutoutMode { Standard, Cave }
    private CutoutMode _mode = CutoutMode.Standard;

    public void SetCutoutMode(CutoutMode cutoutMode)
    {
        if (_mode != cutoutMode)
        {
            _mode = cutoutMode;
            UpdateCutout();
        }
    }

    private void UpdateCutout()
    {
        if (_isActive)
        {
            if (_mode == CutoutMode.Cave)
                SetCutout(caveOn);
            else
                SetCutout(standardOn);
        }
        else
        {
            if (_mode == CutoutMode.Cave)
                SetCutout(caveOff);
            else
                SetCutout(standardOff);
        }
    }


    #region Obstruction detection

    // Matches the variable name in the Shader
    private static readonly int PosID = Shader.PropertyToID("_CameraTarget");
    private static readonly int cam_R = Shader.PropertyToID("_Camera_Outer_Radius");
    private static readonly int cam_r = Shader.PropertyToID("_Camera_Inner_Radius");
    private static readonly int play_R = Shader.PropertyToID("_Player_Outer_Radius");
    private static readonly int play_r = Shader.PropertyToID("_Player_Inner_Radius");

    void LateUpdate()
    {
        Shader.SetGlobalVector(PosID, _camera._targetTransform.position); // Send the player's position to ALL shaders containing this variable

        Vector3 vector = _camera.transform.position - _camera._targetTransform.position;
        if (_isActive != Physics.Raycast(_camera._targetTransform.position, vector, vector.magnitude, _cutoutLayers))
        {
            _isActive = !_isActive;
            UpdateCutout();
        }
    }

    #endregion

    #region Material adjustments

    private void SetCutout(Vector4 cutoutProfile)
    {
        StopAllCoroutines();
        StartCoroutine(SetCutoutCoroutine(cutoutProfile));
    }

    private IEnumerator SetCutoutCoroutine(Vector4 targetCutoutProfile)
    {
        Vector4 initialCutoutProfile = GetMaterialProperties();

        float t = 0;

        while (t < 1)
        {
            SetMaterialProperties(Vector4.Lerp(initialCutoutProfile, targetCutoutProfile, Mathf.SmoothStep(0, 1, t)));
            t += Time.deltaTime * _speed;
            yield return 0;
        }

        SetMaterialProperties(targetCutoutProfile);
    }

    private Vector4 GetMaterialProperties()
    {
        return new(Shader.GetGlobalFloat(cam_R),
        Shader.GetGlobalFloat(cam_r),
        Shader.GetGlobalFloat(play_R),
        Shader.GetGlobalFloat(play_r));
    }

    private void SetMaterialProperties(Vector4 vector)
    {
        Shader.SetGlobalFloat(cam_R, vector.x);
        Shader.SetGlobalFloat(cam_r, vector.y);
        Shader.SetGlobalFloat(play_R, vector.z);
        Shader.SetGlobalFloat(play_r, vector.w);
    }

    void OnDisable()
    {
        Shader.SetGlobalVector(PosID, new(0, 0, 0));
        Shader.SetGlobalFloat(cam_R, 0);
        Shader.SetGlobalFloat(cam_r, 0);
        Shader.SetGlobalFloat(play_R, 0);
        Shader.SetGlobalFloat(play_r, 0);
    }

    void OnDestroy() => StopAllCoroutines();

    #endregion
}
