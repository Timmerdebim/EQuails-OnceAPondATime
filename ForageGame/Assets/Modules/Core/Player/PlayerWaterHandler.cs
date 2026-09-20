using System;
using TDK.PlayerSystem;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PlayerWaterHandler : MonoBehaviour
{
    [SerializeField] private LayerMask waterLayer;

    private int _colliderCounter = 0;

    void OnTriggerEnter(Collider other)
    {
        if ((waterLayer.value & (1 << other.gameObject.layer)) != 0)
            AddCounter(1);
    }

    void OnTriggerExit(Collider other)
    {
        if ((waterLayer.value & (1 << other.gameObject.layer)) != 0)
            AddCounter(-1);
    }

    private void AddCounter(int value)
    {
        if (_colliderCounter > 0 && _colliderCounter + value < 1)
            Player.Instance.playerController.OnWaterExit();
        if (_colliderCounter < 1 && _colliderCounter + value > 0)
            Player.Instance.playerController.OnWaterEnter();
        _colliderCounter = Math.Max(0, _colliderCounter + value);
    }

    public void Reset() => AddCounter(-_colliderCounter);
}
