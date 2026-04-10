using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace NPC
{
    public enum Character { Bracken, Mosswick, Grimble, Lyria }; 

    public class NpcController : MonoBehaviour
    {

        [Header("Dialogue Data")]
        [SerializeField] private Character character;
        [SerializeField] private List<TextAsset> _sourceFiles;
        
        private DialogueDatabase _database;
        private NPCDialogueState _state; 

        [Header("Dialogue References")]
        [SerializeField] private DialogueReferences dialogueReferences;
        private Dictionary<string, UnityEvent> dialogueActions; //...actual dict at runtime

        private Dictionary<string, NpcLocation> npcLocations; //...actual dict at runtime

        void Awake()
        {
            dialogueActions = dialogueReferences.GetDialogueActionMap();

            npcLocations = dialogueReferences.GetNpcLocationsMap();
            dialogueActions = new Dictionary<string, UnityEvent>();
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
