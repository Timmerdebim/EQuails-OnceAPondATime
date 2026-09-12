using System.Collections.Generic;
using System.Linq;
using TDK.ItemSystem;
using TDK.ItemSystem.Inventory;
using UnityEngine;
using UnityEngine.Events;
using TDK.ItemSystem.Types;
using TDK.SaveSystem;
using System;

namespace NPC
{

    /// <summary>
    /// This is a carefully 'engineered' solution for the two times Npcs talk to each other.
    /// There are quite some requirements and specifics:
    /// 
    /// - There are still 'normal' NpcLocations that are part of an NpcController, 
    /// but their interactables must be disabled since the player must interact with this thing instead.
    /// - The 'OutlinedObject' should be set to the default speaker, but maybe two outlined objects can be used somehow.
    /// - Npc dialogues are still handled by the NpcControllers, but the DialogueLines simply have Actions that switch the next speaker to the other NpcLocation.
    /// 
    /// - The MultiNpcInteractionController then simply routes interactions to the next speaker, while making sure the previous one closes their dialogue accordingly.
    /// 
    /// </summary>
    public class MultiNpcInteractionController : MonoBehaviour
    {
        [SerializeField] private NpcLocation _defaultSpeaker;
        private NpcLocation _nextSpeaker;
        private NpcLocation _currentSpeaker;

        private void OnEnable()
        {
            _nextSpeaker = _defaultSpeaker;
        }

        // Called by player interacting with the floor interactable
        public void Next() => _nextSpeaker.Next();

        // Called by player unfocusing - silently ignored during cutscene
        public void OnUnfocus() { }

        // Called via DialogueAction when speaker switches
        public void SwitchSpeaker(NpcLocation next)
        {
            if (next == _nextSpeaker) return;
            _currentSpeaker = _nextSpeaker;
            _nextSpeaker = next;
            _currentSpeaker.WalkAway();
        }
    }
}


