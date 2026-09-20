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
        float relativeSpeed = _speed / (Vector4.Distance(initialCutoutProfile, targetCutoutProfile) + 0.01f); // +0.01f for div 0 protection

        while (t < 1)
        {
            SetMaterialProperties(Vector4.Lerp(initialCutoutProfile, targetCutoutProfile, Mathf.SmoothStep(0, 1, t)));
            t += Time.deltaTime * relativeSpeed;
            yield return 0;
        }

        SetMaterialProperties(targetCutoutProfile);
    }

    private Vector4 GetMaterialProperties()
    {
        return new(baseMat.GetFloat("_Camera_Outer_Radius"),
        baseMat.GetFloat("_Camera_Inner_Radius"),
        baseMat.GetFloat("_Player_Outer_Radius"),
        baseMat.GetFloat("_Player_Inner_Radius"));
    }

    private void SetMaterialProperties(Vector4 vector)
    {
        baseMat.SetFloat("_Camera_Outer_Radius", vector.x);
        baseMat.SetFloat("_Camera_Inner_Radius", vector.y);
        baseMat.SetFloat("_Player_Outer_Radius", vector.z);
        baseMat.SetFloat("_Player_Inner_Radius", vector.w);
    }

    void OnDestroy() => StopAllCoroutines();

    #endregion
}
