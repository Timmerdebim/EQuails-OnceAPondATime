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

    public class NpcController : MonoBehaviour
    {
        [SerializeField] private List<DialogueActionEntry> dialogueActionMap; //serializable dict in inspector...
        [SerializeField] private Dictionary<string, UnityEvent> dialogueActions; //...actual dict at runtime

        void Awake()
        {
            dialogueActions = new Dictionary<string, UnityEvent>();
            foreach (var entry in dialogueActionMap)
                dialogueActions[entry. dialogueAction] = entry.gadgetAction;
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}
