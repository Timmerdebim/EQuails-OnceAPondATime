using Assets.Modules.Interaction;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Items
{
    public class Item : ScriptableObject
    {
        [SerializeField] protected string itemName;
        [SerializeField] protected string description;
        [SerializeField] protected Sprite sprite;

        public int GetId() => ItemManager.Instance.GetIdByItem(this);
        public string GetName() => itemName;
        public string GetDescription() => description;
        public Sprite GetSprite() => sprite;

        // World item function; return true if executed successfully.
        public virtual bool TryWorldItemInteract() => throw new System.NotImplementedException();

        // Item function; return true if executed successfully.
        public virtual bool TryUse() => throw new System.NotImplementedException();
    }
}