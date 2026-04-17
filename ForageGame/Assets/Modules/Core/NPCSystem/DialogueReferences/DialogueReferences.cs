using System.Collections.Generic;
using TDK.ItemSystem;
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

    [System.Serializable]
    public struct ItemEntry
    {
        public string name;
        public ItemData item;
    }
    public class DialogueReferences : MonoBehaviour
    {
        [SerializeField] private List<DialogueActionEntry> dialogueActionMap; //serializable dict in inspector...

        [SerializeField] private List<NpcLocationEntry> NpcLocationMap; //serializable dict in inspector...

        [SerializeField] private List<ItemEntry> ItemMap; //serializable dict in inspector...

        public Dictionary<string, UnityEvent> GetDialogueActionMap()
        {
            var dict = new Dictionary<string, UnityEvent>();
            foreach (var entry in dialogueActionMap)
                dict[entry. dialogueAction] = entry.gadgetAction;
            return dict;
        }

        public Dictionary<string, NpcLocation> GetNpcLocationsMap()
        {
            var dict = new Dictionary<string, NpcLocation>();
            foreach (var entry in NpcLocationMap)
                dict[entry.location] = entry.gameObject;
            return dict;
        }

        public Dictionary<string, ItemData> GetItemDataMap()
        {
            var dict = new Dictionary<string, ItemData>();
            foreach (var entry in ItemMap)
                dict[entry.name] = entry.item;
            return dict;
        }
    }
}
