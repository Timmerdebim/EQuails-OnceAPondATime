using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TDK.PortSystem
{
    /// <summary>
    /// Base class for all Gadgets. A Gadget is any object that has an on/off state
    /// and can be connected to other Gadgets via wires.
    /// </summary>
    public class InputPort : Port
    {
        [Header("Input Port Settings")]
        [SerializeField] private List<OutputPort> initialSources = new();

        public UnityEvent<bool, OutputPort> OnSignalReceived;

        void Start() => PortSystem.Connections.AddEntries(initialSources, this);
        void OnDestroy() => PortSystem.Connections.RemoveEntriesByTarget(this);
        public void ReceiveSignalFrom(bool signal, OutputPort source) { OnSignalReceived.Invoke(signal, source); }
    }
}