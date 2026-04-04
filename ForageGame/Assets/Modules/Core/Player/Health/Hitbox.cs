using System;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public bool monoHit; // Only deals damage 1 time per object
    public int attackDamage = 24;
    public float attacksPerSecond = 1f;  // Only valid if monoHit is false
    // private float timeSinceLastAttack = 0;
    public LayerMask targetLayers;
    private HashSet<GameObject> objectsHit = new HashSet<GameObject>(); // only valid for mono hit objects

    public void PivotTarget(Vector3 direction)
    {
        transform.localPosition = direction;
        transform.localRotation = Quaternion.LookRotation(direction);
    }

    public void Reset()
    {
        objectsHit = new HashSet<GameObject>();
    }


    void OnTriggerEnter(Collider other)
    {
        // Only valid for mono hit objects
        if (!monoHit) return;

        // Check if on correct layer
        if ((targetLayers.value & (1 << other.gameObject.layer)) == 0)
            return;

        // Check if it can be hit
        IHitHandler hh = other.gameObject.GetComponent<IHitHandler>();
        if (hh == null)
            return;

        // Check if it has already been hit
        if (objectsHit.Contains(other.gameObject))
            return;

        // Deal damage
        hh.Hit(attackDamage);
        objectsHit.Add(other.gameObject);
    }

    void OnTriggerStay(Collider other)
    {
        // Only valid for repeating hit objects
        if (monoHit) return;

        // Check if on correct layer
        if ((targetLayers.value & (1 << other.gameObject.layer)) == 0)
            return;

        // Check if it can be hit
        IHitHandler hh = other.gameObject.GetComponent<IHitHandler>();
        if (hh == null)
            return;

        // Check if the time has passed to allow for a ??? (do not do this?)

        // Deal damage
        hh.Hit(attackDamage);
    }
}