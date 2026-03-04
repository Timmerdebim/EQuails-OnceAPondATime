using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Modules.Interaction;
using UnityEngine;

namespace Project.Signals
{
    public abstract class SignalTarget : MonoBehaviour, ISignalTarget
    {
        [Header("Signal Target")]
        [SerializeField] private List<SignalSource> initialSources = new();

        void Start()
        {
            HashSet<ISignalSource> sources = new();
            foreach (var source in initialSources)
                sources.Add(source);
            SignalManager.Instance?.AddWiresFromTo(sources, this);
        }
        public void ReceiveSignal<T>(T signal) => throw new System.NotImplementedException();
        void OnDestroy() => SignalManager.Instance.RemoveWiresTo(this);
    }
}