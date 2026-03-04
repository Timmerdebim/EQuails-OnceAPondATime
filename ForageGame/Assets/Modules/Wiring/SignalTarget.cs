using System;
using System.Collections.Generic;
using Assets.Modules.Interaction;
using UnityEngine;

namespace Project.Signals
{
    public abstract class SignalTarget : MonoBehaviour, ISignalTarget
    {
        [Header("Signal Target")]
        [SerializeField] private HashSet<ISignalSource> initialSignals = new();

        void Start() => SignalManager.Instance?.AddWiresFromTo(initialSignals, this);
        public void ReceiveSignal<T>(T signal) => throw new System.NotImplementedException();
        void OnDestroy() => SignalManager.Instance.RemoveWiresTo(this);
    }
}