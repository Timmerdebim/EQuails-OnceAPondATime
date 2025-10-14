using System.Collections.Generic;
using UnityEngine;


public class DuckHitbox : MonoBehaviour {

    public int attackDamage = 24;
    
    LineRenderer lineEffect;
    BoxCollider hitboxCollider;

    private bool prevEnabled = false;
    // to avoid hitting the same target multiple times in one activation

    HashSet<GameObject> hitsThisActivation = new HashSet<GameObject>();

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
    }

    public void OnTriggerEnter(Collider other) {
        // layer must be attackable
        // if (other.gameObject.layer == LayerMask.NameToLayer("Attack")) {
        //     Debug.Log("DuckHitbox triggered by: " + other.name);
        //     
        // }

        if (this.hitsThisActivation.Contains(other.gameObject)) {
            return;
        }
        
        // exclude own parents
        if (other.transform.IsChildOf(this.transform.parent)) {
            return;
        }

        hitsThisActivation.Add(other.gameObject);
        Debug.Log("DuckHitbox triggered by: " + other.name);
        // get healthcomponent of other
        HealthComponent hc = other.gameObject.GetComponent<HealthComponent>();
        if (!hc) {
            Debug.Log("DuckHitbox: No HealthComponent found on " + other.gameObject.name);
            return;
        }
        else {
            hc.Hit(attackDamage);
        }

        // other.gameObject.SendMessage("Hit", 1, SendMessageOptions.DontRequireReceiver);
        // damage
    }
}