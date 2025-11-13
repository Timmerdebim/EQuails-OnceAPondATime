using System;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public enum PickupType {
        ThroughDash,
    }
    public PickupType pickupType;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other) {
        // only the duck
        if (other.gameObject.GetComponent<DuckController>() != null) {
            Debug.Log("Picked up " + gameObject.name);
            DuckController duck = other.gameObject.GetComponent<DuckController>();
            switch (PickupType.ThroughDash) {
                case PickupType.ThroughDash:
                    duck.dashType = DuckController.DashType.throughDash;
                    break;
            }
            Destroy(gameObject);
        }
    }
}
