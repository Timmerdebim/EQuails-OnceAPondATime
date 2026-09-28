using System;
using System.Collections;
using TDK.CameraSystem;
using UnityEngine;

public class CutoutSetter : MonoBehaviour
{
    public void UseCaveMode(bool useCaveMode) => CutoutController.Instance.UseCaveMode(useCaveMode, false);
    void OnDestroy() => CutoutController.Instance.UseCaveMode(false, false);
}
