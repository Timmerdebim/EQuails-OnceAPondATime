using System;
using System.Collections.Generic;
using TDK.Gadgets;
using UnityEngine;
using UnityEngine.Events;

public class SequencePuzzle : MonoBehaviour
{
    [SerializeField] private List<SwitchController> _solution = new();

    public UnityEvent OnSolved;
    public UnityEvent OnFailed;

    public bool Locked = false;

    private int _sequenceIndex = 0;

    public void LogInput(SwitchController switchController)
    {
        if (Locked) return;

        if (_solution[_sequenceIndex] != switchController)
        {
            _sequenceIndex = 0;
            OnFailed.Invoke();
            return;
        }

        _sequenceIndex++;

        if (_sequenceIndex == _solution.Count)
        {
            foreach (SwitchController key in _solution)
                key.Locked = true;

            OnSolved.Invoke();
            Locked = true;
        }
    }
}
