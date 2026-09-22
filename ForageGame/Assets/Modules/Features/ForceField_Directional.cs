using System.Collections.Generic;
using UnityEngine;

public class ForceField_Directional : MonoBehaviour
{
    [SerializeField] private Vector3 _force = Vector3.forward;
    [SerializeField] private ForceMode _forceMode = ForceMode.Force;
    [SerializeField] private bool _useTransformRotation = true;
    private List<Rigidbody> _rigidbodiesInField = new();

    private Vector3 _finalForce;

    private void FixedUpdate()
    {
        _finalForce = _useTransformRotation
            ? transform.rotation * _force
            : _force;

        for (int i = _rigidbodiesInField.Count - 1; i >= 0; i--)
        {
            if (_rigidbodiesInField[i] != null)
                _rigidbodiesInField[i].AddForce(_finalForce, _forceMode);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;
        if (rb != null && !_rigidbodiesInField.Contains(rb))
            _rigidbodiesInField.Add(rb);
    }

    private void OnTriggerExit(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;
        if (rb != null && _rigidbodiesInField.Contains(rb))
            _rigidbodiesInField.Remove(rb);
    }
}
