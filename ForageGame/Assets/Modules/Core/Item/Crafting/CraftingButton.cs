using Assets.Modules.Interaction;
using UnityEngine;
using UnityEngine.Events;

namespace TDK.ItemSystem.Inventory
{
    public class CraftingButton : MonoBehaviour, IInteractable
    {
        [SerializeField] Crafter crafter;

        public void Interact()
        {
            crafter.TryCraft();
        }

        public void Focus()
        {
        }

        public void Unfocus()
        {
        }
    }
}