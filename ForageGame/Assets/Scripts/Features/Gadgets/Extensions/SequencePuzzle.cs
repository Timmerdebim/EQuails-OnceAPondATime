using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TDK.Gadgets
{
    public class SequencePuzzle : MonoBehaviour
    {
        [SerializeField] private List<ButtonController> _solution = new();

        public UnityEvent OnSolved;
        public UnityEvent OnFailed;

        public bool Locked = false;
        private int _sequenceIndex = 0;

        public void LogInput(ButtonController buttonController)
        {
            if (Locked) return;

            if (_solution[_sequenceIndex] != buttonController)
            {
                PuzzleFailed();
                if (_solution[0] != buttonController)
                    return;
            }

            _sequenceIndex++;

            if (_sequenceIndex == _solution.Count) PuzzleSolved();
        }

        private void PuzzleFailed()
        {
            _sequenceIndex = 0;
            OnFailed.Invoke();
        }

        private void PuzzleSolved()
        {
            foreach (ButtonController key in _solution)
                key.Locked = true;

            OnSolved.Invoke();
            Locked = true;
        }
    }
}