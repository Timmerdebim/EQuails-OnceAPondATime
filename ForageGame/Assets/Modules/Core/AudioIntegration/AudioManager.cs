using UnityEngine;
using System.Collections.Generic;
namespace AudioIntegration
{
    /// <summary>
    /// Any audio source that needs to be stopped by the audio manager should implement this interface and register itself with the audio manager.
    /// </summary>
    public interface IManagedAudioSource
    {
        void StopAndRelease();
    }

    /// <summary>
    /// Just makes sure we have a reference to all audio sources that either don't stop themselves or might need stopping.
    /// i.e., this does not care what scene we are in, it can be main menu music, or world ambience.
    /// 
    /// ~Lars
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        private List<IManagedAudioSource> _registeredSources = new(); 

        [SerializeField] private string pausablesBusPath;
        private FMOD.Studio.Bus _pausablesBus;


        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            // DontDestroyOnLoad(gameObject);
            _pausablesBus = FMODUnity.RuntimeManager.GetBus(pausablesBusPath);
        }

        public void Register(IManagedAudioSource source)   => _registeredSources.Add(source);
        public void Unregister(IManagedAudioSource source) => _registeredSources.Remove(source);

        public void StopAndReleaseAll()
        {
            foreach (var source in _registeredSources)
            {
                // Debug.Log($"[AudioManager] Stopping and releasing audio instance: {source.GetType().Name}.");
                source.StopAndRelease();
                _pausablesBus.stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT); //not all pausables are IManagedAudioSources, so make sure their sfx don't resume on re-entering game
            }  
        }

        public void OnGamePaused()
        {
            _pausablesBus.setPaused(true);
        }

        public void OnGameResumed()
        {
            _pausablesBus.setPaused(false);
        }
    }

}