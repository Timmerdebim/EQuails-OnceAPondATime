using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace TDK.Gadgets
{
    public class CodePuzzle : MonoBehaviour
    {
        [SerializeField] private SolutionEntry[] _solution;

        [Serializable]
        private struct SolutionEntry
        {
            public SwitchController SwitchController;
            public bool Target;
        }

        public UnityEvent OnSolved;

        public bool Locked = false;

        public void CheckAnswer()
        {
            if (Locked) return;

            foreach (SolutionEntry entry in _solution)
                if (entry.SwitchController.State != entry.Target) return;

            PuzzleSolved();
        }

        private void PuzzleSolved()
        {
            LockSwitches();
            OnSolved.Invoke();
            Locked = true;
        }

        private void LockSwitches()
        {
            foreach (SolutionEntry entry in _solution)
                entry.SwitchController.Locked = true;
        }
    }
}