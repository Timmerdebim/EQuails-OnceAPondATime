using System.Collections.Generic;
using System.Linq;
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
        
        [SerializeField] private DialogueDatabase _database;

        [Header("Dialogue References")]
        [SerializeField] private DialogueReferences dialogueReferences;
        private Dictionary<string, UnityEvent> dialogueActions; //...actual dict at runtime

        private Dictionary<string, NpcLocation> npcLocations; //...actual dict at runtime

        private Dictionary<string, ItemData> items; //...actual dict at runtime

        [Header("Current State")]
        [SerializeField] private StoryStage _activeStage;
        [SerializeField] private Dictionary<NpcLocation, int> _lineIndices = new();
        [SerializeField] private HashSet<int> _completedStageIndices = new();

        void Awake()
        {
            dialogueActions = dialogueReferences.GetDialogueActionMap();
            npcLocations = dialogueReferences.GetNpcLocationsMap();
            items = dialogueReferences.GetItemDataMap();
        }

        //Changed to Start() from Awake() since it gave inconsistent behavior in terms of timing ~Lars
        private void Start()
        {
            StoryFlagManager.onFlagAdded += OnNewStoryFlag;
            _database = parser.Parse(_sourceFile.text, StoryFlagManager.Instance.flagDatabase, items, npcLocations, dialogueActions);
            EvaluateActiveStage();
        }
        #region State Management

        private int GetActiveStageIndex() => _activeStage == null ? -1 : _database.storyStages.IndexOf(_activeStage);

        private void OnNewStoryFlag(StoryFlag flag) => EvaluateActiveStage(); //TODO: only if current stage done!, add to completed stages if so!

        private void EvaluateActiveStage()
        {
            Debug.Log($"[NpcController: {character}] Re-evaluating active stage, current stage index is {GetActiveStageIndex()}");

            int startIndex = GetActiveStageIndex();

            var next = _database.storyStages
                .Skip(startIndex)
                .FirstOrDefault(s => 
                    !_completedStageIndices.Contains(_database.storyStages.IndexOf(s)) &&
                    StoryFlagManager.Instance.FlagListActive(s.RequiredFlags));// &&
                    //s.requiredItems.All(item => _inventory.HasItem(item))); //TODO: do

            if (next == _activeStage) return; //if makes no difference nothing changes!
            
            _activeStage = next;
            _lineIndices.Clear();
            _completedStageIndices.Add(GetActiveStageIndex());

            Debug.Log($"[NpcController: {character}] New active StoryStage set with index {GetActiveStageIndex()}");
            //UpdateLocationActiveStates();
        }
        
        #endregion

        #region API

        public DialogueLine GetNextDialogue(NpcLocation location)
        {
            DialogueLine line = new DialogueLine();
            line.StageID="repeat";
            line.Text = $"This is location: {location.gameObject.name}";
            return line;
        }
        #endregion
    }
}
