using UnityEngine;
using UnityEngine.Events;

namespace TDK.GadgetSystem.Runtime.Gadgets
{
    /// <summary>
    /// A Switch is a Gadget that can be toggled manually (e.g. by player interaction).
    /// It broadcasts its state to all connected output Gadgets.
    /// </summary>
    public class SwitchGadget : Gadget
    {
        [Header("Switch Settings")]
        [SerializeField] private bool isToggleSwitch = true;  // true = toggle, false = momentary (hold)
        [SerializeField] private KeyCode debugKey = KeyCode.None; // optional keyboard shortcut for testing

        [Header("Events")]
        public UnityEvent OnSwitchedOn;
        public UnityEvent OnSwitchedOff;

        /// <summary>
        /// Call this from player interaction, UI button, trigger zone, etc.
        /// Toggles the switch or (if momentary) turns it ON.
        /// </summary>
        public void Interact()
        {
            if (isToggleSwitch)
                Toggle();
            else
                SetActive(true);
        }

        /// <summary>For momentary switches: release turns it OFF.</summary>
        public void Release()
        {
            if (!isToggleSwitch)
                SetActive(false);
        }

        protected override void OnStateChanged(bool isActive)
        {
            if (isActive) OnSwitchedOn?.Invoke();
            else OnSwitchedOff?.Invoke();

            Debug.Log($"[Switch] {Label} → {(isActive ? "ON" : "OFF")}");
        }

#if UNITY_EDITOR
        protected override void DrawWireGizmos()
        {
            base.DrawWireGizmos();
        }

        private void OnDrawGizmosSelected()
        {
            // Draw a lever icon above the switch
            Gizmos.color = IsActive ? Color.green : Color.red;
            Vector3 pivot = transform.position + Vector3.up * 0.2f;
            Vector3 tipOff = IsActive ? Vector3.up * 0.5f : (Vector3.up * 0.3f + Vector3.right * 0.35f);
            Gizmos.DrawLine(pivot, pivot + tipOff);
            Gizmos.DrawSphere(pivot + tipOff, 0.07f);
        }
#endif
    }
}