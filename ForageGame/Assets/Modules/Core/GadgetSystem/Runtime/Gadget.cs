using System.Collections.Generic;
using UnityEngine;

namespace TDK.GadgetSystem.Runtime
{
    /// <summary>
    /// Base class for all Gadgets. A Gadget is any object that has an on/off state
    /// and can be connected to other Gadgets via wires.
    /// </summary>
    public abstract class Gadget : MonoBehaviour
    {
        [Header("Gadget Settings")]
        [SerializeField] private bool startActive = false;
        [SerializeField] private string gadgetLabel = "";

        [Header("Wire Visuals")]
        [SerializeField] private Color wireColor = Color.yellow;
        [SerializeField] private bool showWireLabels = true;

        // The list of Gadgets this one sends its signal TO (outputs)
        [SerializeField] private List<Gadget> outputs = new();

        private bool _isActive;

        /// <summary>Whether this Gadget is currently active (on).</summary>
        public bool IsActive
        {
            get => _isActive;
            private set
            {
                if (_isActive == value) return;
                _isActive = value;
                OnStateChanged(_isActive);
            }
        }

        /// <summary>Read-only view of output connections.</summary>
        public IReadOnlyList<Gadget> Outputs => outputs;

        /// <summary>Display label (falls back to GameObject name).</summary>
        public string Label => string.IsNullOrEmpty(gadgetLabel) ? gameObject.name : gadgetLabel;

        public Color WireColor => wireColor;
        public bool ShowWireLabels => showWireLabels;

        protected virtual void Start()
        {
            _isActive = startActive;
            OnStateChanged(_isActive);
        }

        // ── Connection API ──────────────────────────────────────────────

        /// <summary>Connect this Gadget's output to another Gadget's input.</summary>
        public void ConnectTo(Gadget target)
        {
            if (target == null || target == this) return;
            if (!outputs.Contains(target))
                outputs.Add(target);
        }

        /// <summary>Remove an output connection.</summary>
        public void Disconnect(Gadget target)
        {
            outputs.Remove(target);
        }

        /// <summary>Remove all output connections.</summary>
        public void DisconnectAll()
        {
            outputs.Clear();
        }

        // ── State API ───────────────────────────────────────────────────

        /// <summary>Set this Gadget active or inactive and propagate to outputs.</summary>
        public void SetActive(bool active)
        {
            IsActive = active;
            PropagateSignal();
        }

        /// <summary>Toggle the current state.</summary>
        public void Toggle() => SetActive(!_isActive);

        /// <summary>Send this Gadget's current signal to all connected outputs.</summary>
        public void PropagateSignal()
        {
            foreach (var output in outputs)
                output?.ReceiveSignal(this, _isActive);
        }

        /// <summary>
        /// Called when another Gadget sends a signal to this one.
        /// Default: sets this Gadget's state to the received signal.
        /// Override for custom logic (AND gates, OR gates, inverters, etc).
        /// </summary>
        public virtual void ReceiveSignal(Gadget sender, bool signal)
        {
            SetActive(signal);
        }

        // ── Abstract / Virtual hooks ─────────────────────────────────────

        /// <summary>Fired whenever IsActive changes. Implement visual/audio/logic responses here.</summary>
        protected abstract void OnStateChanged(bool isActive);

        // ── Gizmos ───────────────────────────────────────────────────────

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            DrawGadgetGizmo();
            DrawWireGizmos();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = _isActive ? Color.green : Color.red;
            Gizmos.DrawWireSphere(transform.position, 0.6f);
        }

        private void DrawGadgetGizmo()
        {
            Gizmos.color = _isActive
                ? new Color(0f, 1f, 0.3f, 0.35f)
                : new Color(1f, 0.2f, 0.2f, 0.35f);
            Gizmos.DrawCube(transform.position, Vector3.one * 0.4f);

            Gizmos.color = _isActive ? Color.green : Color.red;
            Gizmos.DrawWireCube(transform.position, Vector3.one * 0.4f);
        }

        protected virtual void DrawWireGizmos()
        {
            if (outputs == null) return;

            foreach (var target in outputs)
            {
                if (target == null) continue;

                Vector3 from = transform.position;
                Vector3 to = target.transform.position;
                Vector3 dir = to - from;
                float dist = dir.magnitude;

                Color liveColor = wireColor;
                Color deadColor = new Color(wireColor.r * 0.35f, wireColor.g * 0.35f, wireColor.b * 0.35f, 0.55f);
                Gizmos.color = _isActive ? liveColor : deadColor;

                // Main wire
                Gizmos.DrawLine(from, to);

                // Direction arrow at 65% along the wire
                if (dist > 0.3f)
                {
                    Vector3 arrowTip = from + dir * 0.65f;
                    Vector3 fwd = dir.normalized;
                    Vector3 right = Vector3.Cross(fwd, Vector3.up).normalized;
                    if (right.sqrMagnitude < 0.01f)
                        right = Vector3.Cross(fwd, Vector3.forward).normalized;

                    float arrowSize = Mathf.Clamp(dist * 0.07f, 0.1f, 0.3f);
                    Vector3 arrowL = arrowTip - fwd * arrowSize + right * arrowSize * 0.5f;
                    Vector3 arrowR = arrowTip - fwd * arrowSize - right * arrowSize * 0.5f;

                    Gizmos.DrawLine(arrowTip, arrowL);
                    Gizmos.DrawLine(arrowTip, arrowR);
                }

                // Small terminal sphere at target
                Gizmos.DrawSphere(to, 0.09f);

                // Optional label at wire midpoint
                if (showWireLabels)
                {
                    Vector3 labelPos = from + dir * 0.5f + Vector3.up * 0.28f;
                    string status = _isActive ? "ON" : "OFF";
                    UnityEditor.Handles.color = _isActive ? Color.green : new Color(1f, 0.4f, 0.4f);
                    UnityEditor.Handles.Label(labelPos, $"→ {target.Label} [{status}]",
                        new GUIStyle(UnityEditor.EditorStyles.miniLabel)
                        {
                            normal = { textColor = _isActive ? Color.green : new Color(1f, 0.45f, 0.45f) }
                        });
                }
            }
        }
#endif
    }
}