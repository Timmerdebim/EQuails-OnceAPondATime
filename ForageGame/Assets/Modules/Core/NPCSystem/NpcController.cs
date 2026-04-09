using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace NPC
{
    /// <summary>
    /// Allows for a serializable dictionary in the inspector
    /// </summary>
    [System.Serializable]
    public struct DialogueActionEntry
    {
        public string dialogueAction;
        public UnityEvent gadgetAction;
    }

    [System.Serializable]
    public struct NpcLocationEntry
    {
        public string location;
        public NpcLocation gameObject;
    }

    public enum Character { Bracken, Mosswick, Grimble, Lyria }; 

    public class NpcController : MonoBehaviour
    {
        [SerializeField] private List<DialogueActionEntry> dialogueActionMap; //serializable dict in inspector...
        [SerializeField] private Dictionary<string, UnityEvent> dialogueActions; //...actual dict at runtime

        [SerializeField] private List<NpcLocationEntry> NpcLocationMap; //serializable dict in inspector...
        [SerializeField] private Dictionary<string, NpcLocation> NpcLocations; //...actual dict at runtime

        [SerializeField] private Character character;

        void Awake()
        {
            dialogueActions = new Dictionary<string, UnityEvent>();
            foreach (var entry in dialogueActionMap)
                dialogueActions[entry. dialogueAction] = entry.gadgetAction;

            NpcLocations = new Dictionary<string, NpcLocation>();
            foreach (var entry in NpcLocationMap)
                NpcLocations[entry.location] = entry.gameObject;
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}
