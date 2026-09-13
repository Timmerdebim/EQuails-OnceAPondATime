using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using System;
using TDK.InteractionSystem;

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
    
    [RequireComponent(typeof(Interactable))]
    [RequireComponent(typeof(OutlineObject))]
    [RequireComponent(typeof(Collider))]
    public class MultiNpcInteractionController : MonoBehaviour
    {
        [SerializeField] private NpcLocation _initialSpeaker;
        private NpcLocation _nextSpeaker;
        private NpcLocation _currentSpeaker;

        private void Awake()
        {
            GetComponent<Interactable>().SetInteractibility(false); //disable by default
        }

        public void StartCutscene()
        {
            _currentSpeaker = _initialSpeaker;
            _nextSpeaker = _initialSpeaker;
            GetComponent<Interactable>().SetInteractibility(true);
        }

        public void EndCutscene()
        {
            _currentSpeaker.WalkAway();
            GetComponent<Interactable>().SetInteractibility(false);
        }

        // Called by player interacting with the floor interactable
        public void Next()
        {
            //if the next speaker is a different one, close the old one's dialogue box
            if (_currentSpeaker != _nextSpeaker)
            {
                _currentSpeaker.WalkAway();
            }
            _nextSpeaker.Next();
            _currentSpeaker = _nextSpeaker;
        }

        // Called by player unfocusing
        public void WalkAway()
        {
            _currentSpeaker.WalkAway();
        }

        // Called via DialogueAction when speaker switches
        public void SwitchSpeaker(NpcLocation next)
        {
            if (next == _currentSpeaker) return;
            Debug.Log($"[MultiNpcInteractionController]: Switching speaker from {_currentSpeaker.name} to {next.name}");
            _nextSpeaker = next;
        }
    }
}


