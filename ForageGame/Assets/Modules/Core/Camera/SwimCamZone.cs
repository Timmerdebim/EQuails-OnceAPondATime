using UnityEngine;
using TDK.PlayerSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace TDK.CameraSystem
{
    public class SwimCamZone : MonoBehaviour
    {
        static private int _counter = 0;

        void OnTriggerEnter(Collider other)
        {
            if (other.transform == Player.Instance.transform)
            {
                if (_counter == 0) Player.Instance._cameraTarget.localScale = new(20, 1, 1);
                _counter++;
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.transform == Player.Instance.transform)
            {
                if (_counter == 1) Player.Instance._cameraTarget.localScale = new(12, 1, 1);
                _counter--;
                if (_counter < 0) _counter = 0;
            }
        }
    }
}