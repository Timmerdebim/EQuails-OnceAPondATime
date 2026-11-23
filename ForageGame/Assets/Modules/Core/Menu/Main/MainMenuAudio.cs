using UnityEngine;
using FMODUnity;
using FMOD.Studio;

/// <summary>
/// Just plays the ambience and the correct theme for the main menu (i.e. none, SafeSpace, or credits)
/// </summary>
public class MainMenuAudio : MonoBehaviour
{
    [SerializeField] private EventReference ambienceEvent;
    [SerializeField] private EventReference mainMenuMusic;
    [Tooltip("Only played when finishing the game, not when going through main menu")]
    [SerializeField] private EventReference creditsMusic;
    [SerializeField] private EventReference duckMusicAndAmbienceSnapshot;
    private EventInstance music;
    private EventInstance ambience;
    private EventInstance snapshot;
    public static MainMenuAudio Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Start()
    {
        snapshot = RuntimeManager.CreateInstance(duckMusicAndAmbienceSnapshot);
        ambience = RuntimeManager.CreateInstance(ambienceEvent);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnEnterMenu(bool postCredits=false)
    {
        ambience.start();

        if(postCredits)
        {
            music = RuntimeManager.CreateInstance(creditsMusic);
            music.start();
        }
        else if (true) //TODO: check save file
        {
            music = RuntimeManager.CreateInstance(mainMenuMusic);
            music.start();
        }
    }

    public void EnterMainScreen()
    {
        snapshot.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
    public void ExitMainScreen()
    {
        snapshot.start();
    }

    //on starting game or quit
    public void OnExitMenu()
    {
        music.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        music.release();
        ambience.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        ambience.release();
        snapshot.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        snapshot.release();
    }
}
