using System;
using UnityEngine;

public class Hole : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log(other.gameObject.name + " fell into a hole.");
    }
    
    // trigger when entire object is inside
    private void OnTriggerStay(Collider other) {
        // check if entire object is inside
        Bounds holeBounds = GetComponent<Collider>().bounds;
        Bounds otherBounds = other.bounds;
        if (holeBounds.Contains(otherBounds.min) && holeBounds.Contains(otherBounds.max)) {
            Debug.Log(other.gameObject.name + " is completely inside the hole.");
        }
    }
}
