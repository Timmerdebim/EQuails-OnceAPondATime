using System.Collections.Generic;
using TDK.ItemSystem;
using UnityEngine;
using UnityEngine.Events;

namespace NPC
{
    public enum Character { Bracken, Mosswick, Grimble, Lyria }; 

    public class NpcController : MonoBehaviour
    {

        [Header("Dialogue Data")]
        [SerializeField] private Character character;
        [SerializeField] private TextAsset _sourceFile;
        [SerializeField] private DialogueParser parser;
        
        private DialogueDatabase _database;
        private NPCDialogueState _state; 

        [Header("Dialogue References")]
        [SerializeField] private DialogueReferences dialogueReferences;
        private Dictionary<string, UnityEvent> dialogueActions; //...actual dict at runtime

        private Dictionary<string, NpcLocation> npcLocations; //...actual dict at runtime

        private Dictionary<string, ItemData> items; //...actual dict at runtime

        void Awake()
        {
            dialogueActions = dialogueReferences.GetDialogueActionMap();
            npcLocations = dialogueReferences.GetNpcLocationsMap();
            items = dialogueReferences.GetItemDataMap();
        }

        //Changed to Start() from Awake() since it gave inconsistent behavior in terms of timing ~Lars
        private void Start()
        {
            _database = parser.Parse(_sourceFile.text, npcLocations, dialogueActions, items);
            _state = new NPCDialogueState();
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
