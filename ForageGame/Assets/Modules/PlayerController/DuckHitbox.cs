using System;
using System.Collections.Generic;
using UnityEngine;


public class DuckHitbox : MonoBehaviour {
    public int attackDamage = 24;

    LineRenderer lineEffect;
    BoxCollider hitboxCollider;

    private bool prevEnabled = false;
    // to avoid hitting the same target multiple times in one activation

    private HashSet<GameObject> hitsThisActivation = new HashSet<GameObject>();
    private Collider[] overlapBoxResults = new Collider[10];

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake() {
        hitboxCollider = GetComponent<BoxCollider>();
        lineEffect = GetComponent<LineRenderer>();
        // hide it, not disable it
        lineEffect.enabled = false;
    }

    // Update is called once per frame
    void Update() {
        // if this component is enabled, enable the line effect
        if (hitboxCollider.enabled) {
            lineEffect.enabled = true;
        }
        else {
            lineEffect.enabled = false;
        }


        // if it was enabled last frame but not this frame, clear hitsThisActivation
        if (prevEnabled && !hitboxCollider.enabled) {
            hitsThisActivation.Clear();
        }

        prevEnabled = hitboxCollider.enabled;

        if (!hitboxCollider.enabled) {
            return;
        }

        // check for collisions with other colliders
        var size = Physics.OverlapBoxNonAlloc(hitboxCollider.bounds.center, hitboxCollider.bounds.extents, overlapBoxResults, hitboxCollider.transform.rotation);
        for (int i = 0; i < size; i++) {
            // Debug.Log( "Hitbox overlap with: " + overlapBoxResults[i].gameObject.name);
            Collider hitCollider = overlapBoxResults[i];
            // if (hitCollider.gameObject == this.gameObject) {
            //     continue; // ignore self
            // }
            // if already hit this activation, ignore
            if (hitsThisActivation.Contains(hitCollider.gameObject)) {
                continue;
            }
            hitsThisActivation.Add(hitCollider.gameObject);
            // ignore parents
            if (hitCollider.transform.IsChildOf(this.transform.parent)) {
                continue;
            }
            
            
            HealthComponent hc = hitCollider.gameObject.GetComponent<HealthComponent>();
            if (!hc) {
                // Debug.Log("DuckHitbox: No HealthComponent found on " + hitCollider.gameObject.name);
                continue;
            }
            else {
                hc.Hit(attackDamage);
            }

            // Debug.Log(hitCollider.gameObject.name);
        }
    }

    // // public void OnTrigge
    //
    // // public void OnCollisionStay(Collision other) {
    // //     throw new NotImplementedException();
    // // }
    //
    // public void OnCollisionStay(Collision other) {
    //     // layer must be attackable
    //     // if (other.gameObject.layer == LayerMask.NameToLayer("Attack")) {
    //     //     Debug.Log("DuckHitbox triggered by: " + other.name);
    //     //     
    //     // }
    //
    //     // if (this.hitsThisActivation.Contains(other.gameObject)) {
    //     //     return;
    //     // }
    //     //
    //     // // exclude own parents
    //     // if (other.transform.IsChildOf(this.transform.parent)) {
    //     //     return;
    //     // }
    //
    //     hitsThisActivation.Add(other.gameObject);
    //     Debug.Log("DuckHitbox triggered by: " + other.gameObject.name);
    //     // get healthcomponent of other
    //     HealthComponent hc = other.gameObject.GetComponent<HealthComponent>();
    //     if (!hc) {
    //         Debug.Log("DuckHitbox: No HealthComponent found on " + other.gameObject.name);
    //         return;
    //     }
    //     else {
    //         hc.Hit(attackDamage);
    //     }
    //
    //     // other.gameObject.SendMessage("Hit", 1, SendMessageOptions.DontRequireReceiver);
    //     // damage
    // }
}