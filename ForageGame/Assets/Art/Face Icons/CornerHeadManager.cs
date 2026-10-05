using System.Collections.Generic;
using DG.Tweening;
using NPC;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// THis script just routes calls to the correct head controller
/// 
/// ~Lars
/// </summary>
public class CornerHeadManager : MonoBehaviour
{
    public static CornerHeadManager Instance;

    [System.Serializable]
    public struct cornerHeadEntry
    {
        public DialogueSpeakerType npc;
        public CornerHeadController cornerHeadController;
    }
    public List<cornerHeadEntry> cornerHeadEntries;

    private Dictionary<DialogueSpeakerType, CornerHeadController> cornerHeads = new();


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;

        //set up dictionary
        foreach (var entry in cornerHeadEntries)
            cornerHeads[entry.npc] = entry.cornerHeadController;
    }


    public void SetState(DialogueSpeakerType npc, bool state, bool instant = false)
    {
        if (cornerHeads.TryGetValue(npc, out var cornerHeadController))
        {
            cornerHeadController.SetState(state, instant);
        }
        else
        {
            Debug.LogError($"[CornerHeadManager]: npc type {npc} has no cornerHeadController");
        }
    }

    public void MakeNotBlack(DialogueSpeakerType npc)
    {
        if (cornerHeads.TryGetValue(npc, out var cornerHeadController))
        {
            cornerHeadController.MakeNotBlack();
        }
        else
        {
            Debug.LogError($"[CornerHeadManager]: npc type {npc} has no cornerHeadController");
        }
    }
}
 