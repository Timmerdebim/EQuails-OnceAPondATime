using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TDK.Gadgets
{
    public class MatchPuzzle : MonoBehaviour
    {
        [SerializeField] private SolutionEntry[] _solution;

        [Serializable]
        private struct SolutionEntry
        {
            public SwitchController Switch1;
            public SwitchController Switch2;
        }
        public UnityEvent OnSolved;
        public bool Locked = false;

        public void CheckAnswer()
        {
            if (Locked) return;

            foreach (SolutionEntry entry in _solution)
                if (entry.Switch1.State != entry.Switch2.State) return;

            foreach (SolutionEntry entry in _solution)
            {
                entry.Switch1.Locked = true;
                entry.Switch2.Locked = true;
            }

            OnSolved.Invoke();
            Locked = true;
        }
    }
}