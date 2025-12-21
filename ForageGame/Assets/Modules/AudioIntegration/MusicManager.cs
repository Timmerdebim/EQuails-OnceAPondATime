using System.Collections.Generic;
using System.Collections;
using UnityEngine;

[System.Serializable]
public class ThemeEntry
{
    public FMODUnity.EventReference eventPath;  // FMOD event path
    public float meanDelay = 10f;
    public float variance = 10f;
    public float cooldown = 60f;

    [HideInInspector] public float lastPlayedTime = -Mathf.Infinity; //for theme cooldown
    [HideInInspector] public Coroutine waitRoutine = null;
}

/// <summary>
/// Handles the random trigger-ing of general music themes, also with events in mind
/// </summary>
public class MusicManager : MonoBehaviour
{
    //yes this is a singleton, fight me ~Lars
    public static MusicManager Instance { get; private set; }

    //list of themes that can be played rn, depending on what trigger regions we are in
    public List<ThemeEntry> eligibleThemes;
    [SerializeField] private FMOD.Studio.EventInstance currentTheme; 

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

    public void AddTheme(ThemeEntry theme)
    {
        eligibleThemes.Add(theme);
        theme.waitRoutine = StartCoroutine(ThemeWaitRoutine(theme));
        print("ThemeWaitRoutine Started for theme" + theme.eventPath);
    }

    public void RemoveTheme(ThemeEntry theme)
    {
        print("hehehe not implemented");
        eligibleThemes.Remove(theme);
        // if(state == ManagerState.ProcessStarted && eligibleThemes.Count <= 0)
        // {
        //     StopCoroutine(RandomProcess);
        //     state = ManagerState.Idle;
        //     print("RandomProcess Stopped!");
        // }
    }

    private void StopAllScheduling()
    {
        foreach (var theme in eligibleThemes)
            StopScheduling(theme);
    }

    private void StopScheduling(ThemeEntry theme)
    {
        if (theme.waitRoutine != null)
        {
            print("Stopping scheduling for theme: " + theme.eventPath);
            StopCoroutine(theme.waitRoutine);
            theme.waitRoutine = null;
        }
    }

    IEnumerator ThemeWaitRoutine(ThemeEntry theme)
    {
        while (true)
        {
            //do not play a theme on cooldown
            float elapsedCooldown = Time.time - theme.lastPlayedTime;
            if (elapsedCooldown < theme.cooldown)
            {
                yield return new WaitForSeconds(theme.cooldown - elapsedCooldown);
                continue;
            }

            float waitTime = Mathf.Max(0.1f, UnityEngine.Random.Range(theme.meanDelay - theme.variance, theme.meanDelay + theme.variance));
            yield return new WaitForSeconds(waitTime);

            // if (currentTheme.isValid() == false) //are we not currently doing anything
            // {
            PlayTheme(theme);
            yield break; //end coroutine
            //}
        }
    }

    private void PlayTheme(ThemeEntry theme)
    {
        //we acquired the lock, stop all other scheduling
        StopAllScheduling();
        print("Play " + theme.eventPath);
        currentTheme = FMODUnity.RuntimeManager.CreateInstance(theme.eventPath);
        currentTheme.start();
        theme.lastPlayedTime = Time.time;
        print("Yeah nothing is gonna happen after this lol");

    }
}
