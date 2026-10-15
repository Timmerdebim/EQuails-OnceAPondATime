using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// JUST exists so the NPC head bopping is guaranteed to be in sync with the music
/// </summary>
public class NpcMusicCutscene : MonoBehaviour
{
    private EventInstance instance;
    private EVENT_CALLBACK callback;

    // Thread-safe queue: FMOD callback thread writes, Update() reads
    private static ConcurrentQueue<string> markerQueue = new ConcurrentQueue<string>();
    private static ConcurrentQueue<float> beatQueue = new ConcurrentQueue<float>(); // tempo per beat

    [SerializeField] private Animator animator;


    public void StartMusicCutscene(MusicTheme theme)
    {
        instance = RuntimeManager.CreateInstance(theme.eventRef);
        RuntimeManager.AttachInstanceToGameObject(instance, transform);

        callback = new EVENT_CALLBACK(EventCallback);
        instance.setCallback(callback,
            EVENT_CALLBACK_TYPE.TIMELINE_BEAT | EVENT_CALLBACK_TYPE.TIMELINE_MARKER);

        instance.start();
    }

    [AOT.MonoPInvokeCallback(typeof(EVENT_CALLBACK))]
    static FMOD.RESULT EventCallback(EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
    {
        switch (type)
        {
            case EVENT_CALLBACK_TYPE.TIMELINE_MARKER:
                var markerProps = (TIMELINE_MARKER_PROPERTIES)
                    Marshal.PtrToStructure(parameterPtr, typeof(TIMELINE_MARKER_PROPERTIES));
                markerQueue.Enqueue(markerProps.name);
                break;

            case EVENT_CALLBACK_TYPE.TIMELINE_BEAT:
                var beatProps = (TIMELINE_BEAT_PROPERTIES)
                    Marshal.PtrToStructure(parameterPtr, typeof(TIMELINE_BEAT_PROPERTIES));
                beatQueue.Enqueue(beatProps.tempo);
                break;
        }
        return FMOD.RESULT.OK;
    }

    void Update()
    {
        while (markerQueue.TryDequeue(out string marker))
            HandleMarker(marker);

        while (beatQueue.TryDequeue(out float tempo))
            HandleBeat(tempo);
    }

    void HandleMarker(string marker)
    {
        switch (marker)
        {
            case "emotion:flustered":
                SetEmotion("flustered"); // your existing sprite/anim swap logic
                break;
        }
    }

    public void SetTempo(float tempo) 
    { 
        /* swap sprite/animator state */ 
        animator.speed = tempo / 60f; //60bpm is the base of the animation
    }

    void HandleBeat(float tempo)
    {
        //Debug.Log($"Beat @ tempo: {tempo}");
        // your head-bop resync logic from before
    }

    void SetEmotion(string em) { /* swap sprite/animator state */ }

    void OnDestroy()
    {
        instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        instance.release();
    }
}
