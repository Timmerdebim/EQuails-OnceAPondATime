using System.Collections.Generic;
using UnityEngine;

namespace TDK.GadgetSystem.Runtime
{
    /// <summary>
    /// Base class for all Gadgets. A Gadget is any object that has an on/off state
    /// and can be connected to other Gadgets via wires.
    /// </summary>
    public abstract class InputPort : Port
    {
        [Header("Input Port Settings")]
        [SerializeField] private List<OutputPort> initialSources = new();

        void Start()
        {
            PortSystem.Connections.AddEntries(initialSources, this);
        }
        void OnDestroy() => PortSystem.Connections.RemoveEntriesByTarget(this);

        public void ReceiveSignalFrom(bool signal, OutputPort source) { }
    }
}