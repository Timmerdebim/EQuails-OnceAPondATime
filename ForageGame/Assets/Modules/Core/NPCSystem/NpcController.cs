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

        [Header("Dialogue Data")]
        [SerializeField] private Character character;
        [SerializeField] private List<TextAsset> _sourceFiles;
        
        private DialogueDatabase _database;
        private NPCDialogueState _state; 

        [Header("Dialogue References")]
        [SerializeField] private List<DialogueActionEntry> dialogueActionMap; //serializable dict in inspector...
        private Dictionary<string, UnityEvent> dialogueActions; //...actual dict at runtime

        [SerializeField] private List<NpcLocationEntry> NpcLocationMap; //serializable dict in inspector...
        private Dictionary<string, NpcLocation> NpcLocations; //...actual dict at runtime

        void Awake()
        {
            dialogueActions = new Dictionary<string, UnityEvent>();
            foreach (var entry in dialogueActionMap)
                dialogueActions[entry. dialogueAction] = entry.gadgetAction;

            NpcLocations = new Dictionary<string, NpcLocation>();
            foreach (var entry in NpcLocationMap)
                NpcLocations[entry.location] = entry.gameObject;
        }

        public DialogueLine GetNextDialogue(NpcLocation location)
        {
            DialogueLine line = new DialogueLine();
            line.StageID="repeat";
            line.Text = $"This is location: {location.gameObject.name}";
            return line;
        }
    }
}
