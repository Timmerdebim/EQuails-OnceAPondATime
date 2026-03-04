using System.Collections.Generic;
using UnityEngine;

namespace Project.Signals
{
    public abstract class SignalSource : MonoBehaviour, ISignalSource
    {
        [Header("Signal Source")]
        [SerializeField] private HashSet<ISignalTarget> initialTargets = new();

        void Start() => SignalManager.Instance?.AddWiresFromTo(this, initialTargets);
        protected void SendSignal<T>(T signal) => SignalManager.Instance.SendSignalFrom(signal, this);
        void OnDestroy() => SignalManager.Instance.RemoveWiresFrom(this);
    }
}