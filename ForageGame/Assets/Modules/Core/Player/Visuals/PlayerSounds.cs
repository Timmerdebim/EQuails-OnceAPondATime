using UnityEngine;

namespace TDK.PlayerSystem
{
    public class PlayerSounds : MonoBehaviour
    {
        [SerializeField] private SurfaceTypeDetector surfaceTypeDetector;
        [SerializeField] private FMODUnity.EventReference footstepEvent;

        [SerializeField] private FMODUnity.EventReference quackEvent;

        [Header("Swimming")]
        [SerializeField] private FMODUnity.EventReference waterEnterEvent;

        [SerializeField] private FMODUnity.EventReference waterLeaveEvent;

        [SerializeField] private FMODUnity.EventReference waterSplashEvent;

        [SerializeField] private FMODUnity.EventReference swimEvent;

        [Header("Flying")]
        [SerializeField] private FMODUnity.EventReference takeOffEvent;
        [SerializeField] private FMODUnity.EventReference wingFlapEvent;

        [Header("Combat")]
        [SerializeField] private FMODUnity.EventReference wingSlapEvent;

        public static PlayerSounds Instance { get; private set; } //yes, this sucks, but I HAVE to do it because FMOD SUCKS

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

        private void PlayOneShotWithParameter(FMODUnity.EventReference eventReference, string paramName, float paramValue)
        {
            var instance = FMODUnity.RuntimeManager.CreateInstance(eventReference);
            instance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));
            instance.setParameterByName(paramName, paramValue);
            instance.start();
            instance.release();
        }

        public void PlayFootstep(SurfaceType surfaceType) =>  PlayOneShotWithParameter(footstepEvent, "SurfaceType", (float)surfaceType);

        public void PlaySwimStroke() => FMODUnity.RuntimeManager.PlayOneShot(swimEvent, transform.position);

        public void PlayWaterSplash() => FMODUnity.RuntimeManager.PlayOneShot(waterSplashEvent, transform.position);

        public void PlayWaterEnter() => FMODUnity.RuntimeManager.PlayOneShot(waterEnterEvent, transform.position);
        public void PlayWaterLeave() => FMODUnity.RuntimeManager.PlayOneShot(waterLeaveEvent, transform.position);

        //this specific one is called by state enter of jump and flutter rather than via animation event and PlayerEffects
        public void PlayTakeOff() => FMODUnity.RuntimeManager.PlayOneShot(takeOffEvent, transform.position);
        public void PlayWingFlap() => FMODUnity.RuntimeManager.PlayOneShot(wingFlapEvent, transform.position);

        public void PlayWingSlap() => FMODUnity.RuntimeManager.PlayOneShot(wingSlapEvent, transform.position);
    }
}
