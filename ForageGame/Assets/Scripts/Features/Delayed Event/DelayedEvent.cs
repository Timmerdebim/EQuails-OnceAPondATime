using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DelayedActionModule : MonoBehaviour
{
    [SerializeField] private float _delayTime = 1f;
    [SerializeField] private UnityEvent _defaultAction;

    public void DelayedInvokeDefault() => DelayedInvoke(_defaultAction);
    public void DelayedInvoke(Action action) { DelayedAction.Invoke(this, action, _delayTime); }
    public void DelayedInvoke(UnityEvent unityEvent) { DelayedAction.Invoke(this, () => unityEvent?.Invoke(), _delayTime); }
}


public static class DelayedAction
{
    public static void Invoke(this MonoBehaviour mb, Action action, float delay)
    {
        mb.StartCoroutine(InvokeRoutine(action, delay));
    }

    private static IEnumerator InvokeRoutine(Action action, float delay)
    {
        yield return new WaitForSeconds(delay);
        action();
    }
}