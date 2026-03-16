using System.Collections.Generic;
using TDK.Gadgets;
using UnityEngine;
using UnityEngine.Events;

public class MatchPuzzle : MonoBehaviour
{
    [SerializeField] private Dictionary<SwitchController, SwitchController> _switches = new();

    public UnityEvent OnSolved;

    public bool Locked = false;

    public void CheckAnswer()
    {
        if (Locked) return;

        foreach (SwitchController key in _switches.Keys)
            if (key.State != _switches[key].State) return;

        foreach (SwitchController key in _switches.Keys)
        {
            key.Locked = true;
            _switches[key].Locked = true;
        }

        OnSolved.Invoke();
        Locked = true;
    }
}
