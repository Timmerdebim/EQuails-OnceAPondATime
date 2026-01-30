using UnityEngine;
using System.Collections.Generic;

public class StoryFlagManager : MonoBehaviour
{
    public static StoryFlagManager Instance { get; private set; }

    // Database: ID → SO
    private Dictionary<string, StoryFlag> flagDatabase;

    // Active flags: SO → active
    private HashSet<StoryFlag> activeFlags;

    private void Awake()
    {
        //Singleton management
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadAllFlags(); //construct existing flag database
        activeFlags = new HashSet<StoryFlag>(); //clear existing flags TODO: load from save
    }

    private void LoadAllFlags()
    {
        flagDatabase = new Dictionary<string, StoryFlag>();

        // Load all StoryFlag SOs placed in Resources/StoryFlags
        StoryFlag[] all = Resources.LoadAll<StoryFlag>("StoryFlags");

        foreach (var f in all)
        {
            if (string.IsNullOrEmpty(f.id))
            {
                Debug.LogWarning($"StoryFlag '{f.name}' has no ID!");
                continue;
            }

            if (!flagDatabase.ContainsKey(f.id))
            {
                flagDatabase.Add(f.id, f);
                print($"StoryFlag found: {f.id}");
            }
            else
            {
                Debug.LogWarning($"Duplicate StoryFlag ID '{f.id}'!");
            }
        }
    }
}
