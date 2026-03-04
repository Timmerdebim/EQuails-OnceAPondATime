using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Project.Signals
{
    public abstract class SignalSource : MonoBehaviour, ISignalSource
    {
        [Header("Signal Source")]
        [SerializeField] private List<SignalTarget> initialTargets = new();

        void Start()
        {
            Debug.Log("A");
            HashSet<ISignalTarget> targets = new();
            foreach (var target in initialTargets)
                targets.Add(target);
            Debug.Log(targets);
            SignalManager.Instance.AddWiresFromTo(this, targets);
        }
        protected void SendSignal<T>(T signal) => SignalManager.Instance.SendSignalFrom(signal, this);
        void OnDestroy() => SignalManager.Instance.RemoveWiresFrom(this);
    }
}