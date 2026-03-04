using System.Collections.Generic;
using UnityEngine;

namespace Project.Signals
{
    public class SignalManager : MonoBehaviour
    {
        public static SignalManager Instance { get; private set; }
        private struct Wire { public ISignalSource source; public ISignalTarget target; }
        [SerializeField] private HashSet<Wire> currentWires = new();

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        #region Send Signals

        public void SendSignalFrom<T>(T signal, ISignalSource source)
        {
            foreach (Wire wire in currentWires)
                if (wire.source == source)
                    wire.target.ReceiveSignal(signal);
        }
        public void SendSignalTo<T>(T signal, ISignalTarget target) =>
            target.ReceiveSignal(signal);

        #endregion

        #region Add Wires

        public void AddWireFromTo(ISignalSource source, ISignalTarget target)
        {
            foreach (Wire wire in currentWires)
                if (wire.source == source && wire.target == target)
                    return;
            currentWires.Add(new Wire { source = source, target = target });
        }
        public void AddWiresFromTo(ISignalSource source, HashSet<ISignalTarget> targets)
        {
            foreach (ISignalTarget target in targets)
                AddWireFromTo(source, target);
        }
        public void AddWiresFromTo(HashSet<ISignalSource> sources, ISignalTarget target)
        {
            foreach (ISignalSource source in sources)
                AddWireFromTo(source, target);
        }
        public void AddWiresFromTo(HashSet<ISignalSource> sources, HashSet<ISignalTarget> targets)
        {
            foreach (ISignalSource source in sources)
                foreach (ISignalTarget target in targets)
                    AddWireFromTo(source, target);
        }

        #endregion

        #region Remove Wires

        public void RemoveWireFromTo(ISignalSource source, ISignalTarget target)
        {
            foreach (Wire wire in currentWires)
                if (wire.source == source && wire.target == target)
                    currentWires.Remove(wire);
        }
        public void RemoveWiresFromTo(HashSet<ISignalSource> sources, ISignalTarget target)
        {
            foreach (ISignalSource source in sources)
                RemoveWireFromTo(source, target);
        }
        public void RemoveWiresFromTo(ISignalSource source, HashSet<ISignalTarget> targets)
        {
            foreach (ISignalTarget target in targets)
                RemoveWireFromTo(source, target);
        }
        public void RemoveWiresFromTo(HashSet<ISignalSource> sources, HashSet<ISignalTarget> targets)
        {
            foreach (ISignalSource source in sources)
                foreach (ISignalTarget target in targets)
                    RemoveWireFromTo(source, target);
        }
        public void RemoveWiresFrom(ISignalSource source)
        {
            foreach (Wire wire in currentWires)
                if (wire.source == source)
                    currentWires.Remove(wire);
        }
        public void RemoveWiresFrom(HashSet<ISignalSource> sources)
        {
            foreach (ISignalSource source in sources)
                RemoveWiresFrom(source);
        }
        public void RemoveWiresTo(ISignalTarget target)
        {
            foreach (Wire wire in currentWires)
                if (wire.target == target)
                    currentWires.Remove(wire);
        }
        public void RemoveWiresTo(HashSet<ISignalTarget> targets)
        {
            foreach (ISignalTarget target in targets)
                RemoveWiresTo(target);
        }

        #endregion
    }
}
