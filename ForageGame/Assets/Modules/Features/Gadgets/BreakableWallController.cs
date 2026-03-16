using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class BreakableWallController : MonoBehaviour
{
    public UnityEvent OnBreak;
    private bool _isBroken = false;

    public void Break()
    {
        if (_isBroken) return;
        _isBroken = true;
        OnBreak.Invoke();
    }
}
