using System.Collections.Generic;
using System.Collections;
using UnityEngine;

/// <summary>
/// Handles the random trigger-ing of general music themes, also with events in mind
/// </summary>
public class MusicManager : MonoBehaviour
{
    //yes this is a singleton, fight me ~Lars
    public static MusicManager Instance { get; private set; }

    //list of themes that can be played rn, depending on what trigger regions we are in
    public List<FMODUnity.EventReference> eligibleThemes;
    [SerializeField] private FMOD.Studio.EventInstance currentTheme; 

    private enum ManagerState
    {
        Idle,
        ProcessStarted, //Random process started
        Playing,
        Interrupted //Events can suspend the general themes
    }
    [SerializeField] private ManagerState state = ManagerState.Idle;

    [SerializeField] private float meanWaitTime;
    [SerializeField] private float randomVariance;

    private IEnumerator RandomProcess;

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

        RandomProcess = RandomEventLoop();
    }

    public void AddTheme(FMODUnity.EventReference theme)
    {
        eligibleThemes.Add(theme);
        if(state == ManagerState.Idle)
        {
            state = ManagerState.ProcessStarted;
            StartCoroutine(RandomProcess);
             print("RandomProcess Started!");
        }
    }

    public void RemoveTheme(FMODUnity.EventReference theme)
    {
        eligibleThemes.Remove(theme);
        if(state == ManagerState.ProcessStarted && eligibleThemes.Count <= 0)
        {
            StopCoroutine(RandomProcess);
            state = ManagerState.Idle;
            print("RandomProcess Stopped!");
        }
    }

    IEnumerator RandomEventLoop()
    {
        while (true)
        {
            float waitTime = Mathf.Max(0.1f, Random.Range(meanWaitTime - randomVariance, meanWaitTime + randomVariance));
            yield return new WaitForSeconds(waitTime);

            TriggerRandomEvent();
        }
    }

    private void TriggerRandomEvent()
    {
        print("event FIRED");
        state = ManagerState.Playing;

        print("restarting random process...");
        StartCoroutine(RandomProcess);
        state = ManagerState.ProcessStarted;
    }
}
