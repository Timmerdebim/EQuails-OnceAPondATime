using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Rendering; // this is for SerializedDictionary, dont ask why its in the rendering package...

[CreateAssetMenu(fileName = "Story Manager", menuName = "Data/Story Manager")]
public class StoryManager : ScriptableObject
{
    public static StoryManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;

        Load();
    }

    public bool Get(StoryFlag flag)
    {
        return storyState.TryGetValue(flag, out bool value) && value;
    }

    public void Set(StoryFlag flag, bool value)
    {
        storyState[flag] = value;
    }

    public void Save()
    {
        // TODO
    }

    public void Load()
    {
        // TODO
    }

    [SerializeField]
    private SerializedDictionary<StoryFlag, bool> storyState = new SerializedDictionary<StoryFlag, bool>
    {
        {StoryFlag.Frog_met, false },
        {StoryFlag.Frog_QFlute_Start, false },
        {StoryFlag.Frog_QFlute_End, false }
    };
}

public enum StoryFlag
{
    // ------------ RAT ------------
    Rat_met,
    Rat_QFree_Start,
    Rat_QFree_End,
    Rat_QFood_Start,
    Rat_QFood_End,
    Rat_QHome_Start,
    Rat_QHome_End,
    // ------------ BIRD ------------
    metBird,
    // ------------ FROG ------------
    Frog_met,
    Frog_QFlute_Start,
    Frog_QFlute_GaveItem1,
    Frog_QFlute_GaveItem2,
    Frog_QFlute_End,
    Frog_QBoat_Start,
    Frog_QBoat_End,
    // ------------ SQUIRREL ------------
    metSquirrel,
}