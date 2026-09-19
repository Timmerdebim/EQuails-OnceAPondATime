using UnityEngine;

namespace TDK.PlayerSystem
{
    public class PlayerSounds : MonoBehaviour
    {
        [SerializeField] private SurfaceTypeDetector surfaceTypeDetector;
        [SerializeField] private FMODUnity.EventReference footstepEvent;

        [SerializeField] private FMODUnity.EventReference quackEvent;

        [SerializeField] private FMODUnity.EventReference waterEnterEvent;

        [SerializeField] private FMODUnity.EventReference waterLeaveEvent;

        [SerializeField] private FMODUnity.EventReference waterSplashEvent;

        [SerializeField] private FMODUnity.EventReference swimEvent;

        public static PlayerSounds Instance { get; private set; } //yes, this sucks, but I HAVE to do it because FMOD SUCKS

        private bool splash = false;

        private void Awake() 
        { 
            //May only be one instance ofc
            if (Instance != null && Instance != this) 
            { 
                Destroy(this); 
            } 
            else 
            { 
                Instance = this; 
            } 
        }

        public void PlayFootstep(SurfaceType surfaceType)
        {
            var instance = FMODUnity.RuntimeManager.CreateInstance(footstepEvent);
            instance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));
            instance.setParameterByName("SurfaceType", (float)surfaceType);
            instance.start();
            instance.release();
        }


        public void PlaySwimStroke() => FMODUnity.RuntimeManager.PlayOneShot(swimEvent, transform.position);

        public void PlayWaterSplash() => FMODUnity.RuntimeManager.PlayOneShot(waterSplashEvent, transform.position);

        public void PlayWaterEnter() => FMODUnity.RuntimeManager.PlayOneShot(waterEnterEvent, transform.position);
        public void PlayWaterLeave() => FMODUnity.RuntimeManager.PlayOneShot(waterLeaveEvent, transform.position);
    }
}
