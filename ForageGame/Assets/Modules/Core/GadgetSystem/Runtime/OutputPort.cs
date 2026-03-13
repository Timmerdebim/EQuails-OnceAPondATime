using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TDK.GadgetSystem.Runtime
{
    /// <summary>
    /// Base class for all Gadgets. A Gadget is any object that has an on/off state
    /// and can be connected to other Gadgets via wires.
    /// </summary>
    public abstract class OutputPort : Port
    {
        [Header("Output Port Settings")]
        [SerializeField] private bool initialSignal = false;
        [SerializeField] private InputPort[] initialTargets;

        void Awake() => _signal = initialSignal;
        void Start()
        {
            PortSystem.Connections.AddEntries(this, initialTargets.ToList());
            PortSystem.SendSignalFromSource(_signal, this);
        }

        void OnDestroy() => PortSystem.Connections.RemoveEntriesBySource(this);

        // ────────────────────────────────────────────────────────────────────────────────────────────
        #region Signal API

        private bool _signal;
        public bool Signal
        {
            get => _signal;
            private set
            {
                if (_signal == value) return;
                _signal = value;
                PortSystem.SendSignalFromSource(_signal, this);
            }
        }
        public void SetSignal(bool signal) => Signal = signal;

        #endregion

        // ────────────────────────────────────────────────────────────────────────────────────────────
        #region Gizmos

#if UNITY_EDITOR
        private void OnDrawGizmos() => DrawWireGizmos();

        private void DrawWireGizmos()
        {
            if (initialTargets == null) return;

            InputPort[] targets = initialTargets;

            // targets = PortSystem.Connections.GetTargetsBySource(this).ToArray();

            foreach (var target in targets)
            {
                if (target == null) continue;

                Vector3 from = this.transform.position;
                Vector3 to = target.transform.position;
                Vector3 dir = to - from;
                float dist = dir.magnitude;

                // Main wire
                Gizmos.DrawLine(from, to);

                // Direction arrow at 50% along the wire
                if (dist > 0.3f)
                {
                    float arrowSize = dist * 0.05f;

                    Vector3 arrowTip = from + dir * (0.50f + arrowSize / 2);
                    Vector3 fwd = dir.normalized;
                    Vector3 right = Vector3.Cross(fwd, Vector3.up).normalized;
                    if (right.sqrMagnitude < 0.01f)
                        right = Vector3.Cross(fwd, Vector3.forward).normalized;

                    Vector3 arrowL = arrowTip - fwd * arrowSize + right * arrowSize / 2;
                    Vector3 arrowR = arrowTip - fwd * arrowSize - right * arrowSize / 2;

                    Gizmos.DrawLine(arrowTip, arrowL);
                    Gizmos.DrawLine(arrowTip, arrowR);
                }
                // Label at wire midpoint
                Vector3 labelPos = from + dir * 0.5f + Vector3.up * 0.28f;
                UnityEditor.Handles.Label(labelPos, $"{this.Label}→ {target.Label} [{Signal.ToString()}]");
            }
        }
#endif
        #endregion
    }
}