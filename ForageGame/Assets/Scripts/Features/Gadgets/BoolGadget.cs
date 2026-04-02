using System;
using Assets.Modules.Interaction;
using TDK.PortSystem;
using TDK.SaveSystem;
using UnityEngine;
using UnityEngine.Events;

namespace TDK.Gadgets
{
    public class Gadget : MonoBehaviour, ISaveable, ILoadable
    {
        [SerializeField] private Guid _guid;
        [ContextMenu("Generate GUID")]
        private void GenerateGuid()
        {
            _guid = System.Guid.NewGuid();
        }

        public UnityEvent OnActivate;
        public UnityEvent OnDeactivate;

        private bool _locked = false;
        public bool Locked
        {
            get => _locked;
            private set => _locked = value;
        }
        [SerializeField] private bool _state;
        public bool State
        {
            get => _state;
            private set
            {
                if (Locked) return;
                _state = value;
                if (_state) OnActivate.Invoke();
                else OnDeactivate.Invoke();
            }
        }

        private void Initialize(bool state, bool locked)
        {
            _state = state;
            if (_state) OnActivate.Invoke();
            else OnDeactivate.Invoke();
            Locked = locked;
        }

        public void ToggleState() => State = !State;
        public void SetState(bool state) => State = state;

        public void SaveData(ref WorldSaveData data)
        {
            data.Gadgets[_guid] = new()
            {
                State = _state,
                Locked = _locked
            };
        }

        public void LoadData(WorldSaveData data)
        {
            if (data.Gadgets.ContainsKey(_guid))
                Initialize(data.Gadgets[_guid].State, data.Gadgets[_guid].Locked);
            else
                Initialize(_state, _locked);
        }
    }

    [System.Serializable]
    public class GadgetSaveData
    {
        public bool State = new();
        public bool Locked = new();
    }
}
