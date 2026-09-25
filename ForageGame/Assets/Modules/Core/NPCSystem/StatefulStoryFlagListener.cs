using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Very simple component that reacts to a flag being added
/// Invokes a UnityEvent to be as generic as possible
/// </summary>
/// 
[System.Serializable]
public struct StoryFlagListenerEntry
{
    public StoryFlag flag;
    public UnityEvent action;
}

public class StatefulStoryFlagListener : MonoBehaviour
{
    [Tooltip("Order matters, only the latest active flag will trigger it's action on reload. Earlier flags are always ignored.")]
    [SerializeField] private List<StoryFlagListenerEntry> storyFlagActionMap;

    [SerializeField] private StoryFlagListenerEntry activeEntry;

    void Awake()
    {
        //EvaluateLatestFlagAction(); //should not matter, but is a fallback
    }

    void OnEnable()
    {
        StoryFlagManager.onFlagAdded += OnStoryFlagAdded;
        StoryFlagManager.onStoryFlagsLoaded += EvaluateLatestFlagAction;
    }
    void OnDisable()
    {
        StoryFlagManager.onFlagAdded -= OnStoryFlagAdded;
        StoryFlagManager.onStoryFlagsLoaded -= EvaluateLatestFlagAction;
    }

    private void OnStoryFlagAdded(StoryFlag newFlag)
    {
        if (storyFlagActionMap.Any(e => e.flag == newFlag))
        {
            EvaluateLatestFlagAction();
        }
    }

    private void EvaluateLatestFlagAction()
    {
        // Walk from the END of the list, since later index = later story progress.
        // First active flag we hit from the back IS the "latest active" one.
        //only search for newer flags, of course.
        for (int i = storyFlagActionMap.Count - 1; i > storyFlagActionMap.IndexOf(activeEntry); i--)
        {
            var entry = storyFlagActionMap[i];
            if (entry.flag == null || StoryFlagManager.Instance.FlagActive(entry.flag))
            {
                activeEntry = entry;
                if(entry.flag == null) Debug.Log($"[StatefulStoryFlagListener]: {gameObject.name} new active StoryFlagListenerEntry: None flag");
                else Debug.Log($"[StatefulStoryFlagListener]: {gameObject.name} new active StoryFlagListenerEntry: {entry.flag.id}");
                activeEntry.action?.Invoke();
                return;
            }
        }
    }

    
}
