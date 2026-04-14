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

        [Header("References")]
        [SerializeField] private DialogueReferences dialogueReferences;
        [SerializeField] private List<NpcLocation> locations;

        [Header("Current State")]
        [SerializeField] private StoryStage _activeStage;
        [SerializeField] private Dictionary<NpcLocation, int> _lineIndices = new();
        [SerializeField] private HashSet<int> _completedStageIndices = new();

        void Awake()
        {
            locations = GetComponentsInChildren<NpcLocation>().ToList();
        }

        //Changed to Start() from Awake() since it gave inconsistent behavior in terms of timing ~Lars
        private void Start()
        {
            StoryFlagManager.onFlagAdded += OnNewStoryFlag;
            _database = parser.Parse(_sourceFile.text, 
                                    StoryFlagManager.Instance.flagDatabase, 
                                    dialogueReferences.GetItemDataMap(), 
                                    dialogueReferences.GetNpcLocationsMap(), 
                                    dialogueReferences.GetDialogueActionMap());
            EvaluateActiveStage();
        }
        #region Stage Management

        private int GetActiveStageIndex() => _activeStage == null ? -1 : _database.storyStages.IndexOf(_activeStage);

        private void OnNewStoryFlag(StoryFlag flag)
        {
            if(_completedStageIndices.Contains(GetActiveStageIndex())) EvaluateActiveStage(); //do this only if current stage is done
        }

        private void EvaluateActiveStage()
        {
            Debug.Log($"[NpcController: {character}] Re-evaluating active stage, current stage index is {GetActiveStageIndex()}");

            int startIndex = GetActiveStageIndex();

            //this gets a default value if no target is found which will be null
            var next = _database.storyStages
                .Skip(startIndex)
                .FirstOrDefault(s => 
                    !_completedStageIndices.Contains(_database.storyStages.IndexOf(s)) &&
                    StoryFlagManager.Instance.FlagListActive(s.RequiredFlags));// &&
                    //s.requiredItems.All(item => _inventory.HasItem(item))); //TODO: do

            if (next == _activeStage || next == null) 
            {
                Debug.Log($"[NpcController: {character}] No new Active StoryStage detected");
                return; //if makes no difference nothing changes!
            }
            
            StartNewStoryStage(next);
        }

        private void StartNewStoryStage(StoryStage stage)
        {
            _activeStage = stage;
            Debug.Log($"[NpcController: {character}] New active StoryStage set with index {GetActiveStageIndex()}");

            //update location indices
            _lineIndices.Clear();
            foreach (var loc in _activeStage.locationDialogues.Keys)
            {
                _lineIndices.Add(loc, 0);
            }

            UpdateActiveLocations();
        }

        //turn off NpcLocations that have no dialogue set in the active StoryStage
        private void UpdateActiveLocations()
        {
            foreach (var loc in locations)
                loc.gameObject.SetActive(_activeStage != null && _activeStage.locationDialogues.ContainsKey(loc));
        }
        
        #endregion

        #region API

        /// <summary>
        /// This function will only ever continue the active StoryStage, making it simpler
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public DialogueLine GetNextDialogue(NpcLocation location)
        {
            if(_activeStage == null) 
            {
                Debug.LogError($"[NpcController: {character}] No active StoryStage");
                return GetErrorLine();
            }
            if(_activeStage.locationDialogues.TryGetValue(location, out var dialogue))
            {
                if (_lineIndices[location] >= dialogue.StandardLines.Count)
                {
                    Debug.Log($"[NpcController: {character}] Displaying repeat stage");
                    var line = dialogue.GetSpecialLine("repeat");
                    if (dialogue.isMainDialogue) //TODO: do this after the 'main' stages are displayed, keeping the repeat as repeat. THis must coincide with DialogueBox closing
                    {
                        Debug.Log($"[NpcController: {character}] Finished main locationDialogue");
                        _completedStageIndices.Add(GetActiveStageIndex());
                        EvaluateActiveStage();
                    }
                    return line;
                }

                //regular line
                var l = dialogue.StandardLines[_lineIndices[location]];
                _lineIndices[location] = _lineIndices[location] + 1;
                return l;
            }
            else
            {
                //this REALLY should never happen
                Debug.LogError($"[NpcController: {character}] Active StoryStage has no dialogue for location: {location}");
                return GetErrorLine();
            }
        }

        private DialogueLine GetErrorLine()
        {
            DialogueLine line = new DialogueLine();
            line.StageID="repeat";
            line.Text = $"This text should not appear! Error!";
            return line;
        }

        #endregion
    }
}
