using System.Collections.Generic;
using TDK.Gadgets;
using TDK.PortSystem;
using UnityEngine;
using UnityEngine.Events;

public class CodePuzzle : MonoBehaviour
{
    [SerializeField] private Dictionary<SwitchController, bool> _solutionDict = new();

    public UnityEvent OnSolved;

    public bool Locked = false;

    public void CheckAnswer()
    {
        if (Locked) return;

        foreach (SwitchController key in _solutionDict.Keys)
            if (key.State != _solutionDict[key]) return;

        foreach (SwitchController key in _solutionDict.Keys)
            key.Locked = true;

        OnSolved.Invoke();
        Locked = true;
    }
}
