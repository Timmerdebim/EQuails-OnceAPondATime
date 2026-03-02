using UnityEngine;
using DG.Tweening;

public class Door_Gate : MonoBehaviour
{
    private bool _isOpen = false;
    private Tween _currentTween;

    public void OpenDoor()
    {
        if (_isOpen) return;

        _currentTween?.Kill();
        _currentTween = transform.DOMove(transform.position - new Vector3(0, 5, 0), 1)
            .SetEase(Ease.InOutCubic)
            .OnComplete(() => DoorOpened());
    }

    private void DoorOpened()
    {
        _isOpen = true;
        Collider collider = GetComponent<Collider>();
        collider.enabled = false;
    }
}
