using System.Collections.Generic;
using UnityEngine;

public class ForceField : MonoBehaviour
{
    [SerializeField] private float _forceStrength = 30;
    private List<Rigidbody> _rigidbodiesInField = new();
    [SerializeField] private ForceMode _forceMode = ForceMode.Force;
    private enum ForceShape { Directional, Cylindrical, Sphereical }
    [SerializeField] private ForceShape _forceShape = ForceShape.Directional;
    [SerializeField] private Vector3 _localForceAxis = Vector3.up;

    private Vector3 _globalForceAxis;
    private Vector3 _finalForce;

    private void FixedUpdate()
    {
        _globalForceAxis = transform.rotation * _localForceAxis.normalized;
        switch (_forceShape)
        {
            case ForceShape.Directional:
                ApplyDiectionalForces();
                break;
            case ForceShape.Cylindrical:
                ApplyCylindricalForces();
                break;
            case ForceShape.Sphereical:
                ApplySphericalForces();
                break;
        }
    }

    private Vector3 _force = Vector3.zero;

    private void ApplyDiectionalForces()
    {
        _force = _globalForceAxis * _forceStrength;

        for (int i = _rigidbodiesInField.Count - 1; i >= 0; i--)
        {
            if (_rigidbodiesInField[i] != null)
                _rigidbodiesInField[i].AddForce(_force, _forceMode);
        }
    }

    private void ApplyCylindricalForces()
    {
        for (int i = _rigidbodiesInField.Count - 1; i >= 0; i--)
        {
            if (_rigidbodiesInField[i] != null)
            {
                _force = Vector3.ProjectOnPlane(_rigidbodiesInField[i].position - transform.position, _globalForceAxis).normalized * _forceStrength;
                _rigidbodiesInField[i].AddForce(_force, _forceMode);
            }
        }
    }

    private void ApplySphericalForces()
    {
        for (int i = _rigidbodiesInField.Count - 1; i >= 0; i--)
        {
            if (_rigidbodiesInField[i] != null)
            {
                _force = (_rigidbodiesInField[i].position - transform.position).normalized * _forceStrength;
                _rigidbodiesInField[i].AddForce(_force, _forceMode);
            }
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
