using Assets.Modules.Interaction;
using TDK.GadgetSystem.Runtime.Gadgets;
using UnityEngine;

namespace TDK.Gadgets
{
    [RequireComponent(typeof(SwitchGadget))]
    [RequireComponent(typeof(Animator))]
    public class SwitchController : MonoBehaviour, IInteractable
    {
        private SwitchGadget switchGadget;
        private Animator animator;
        void Awake()
        {
            switchGadget = GetComponent<SwitchGadget>();
            animator = GetComponent<Animator>();
        }

        public void Interact()
        {
            switchGadget.Toggle();
            animator.SetBool("isOpen", switchGadget.IsActive)
        }

        public void Focus()
        {
            throw new System.NotImplementedException();
        }

        public void Unfocus()
        {
            throw new System.NotImplementedException();
        }
    }
}