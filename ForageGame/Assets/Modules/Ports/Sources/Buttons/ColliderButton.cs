using Project.Ports;
using Project.Signals.Targets;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class ColliderButton : MonoBehaviour
{
    [SerializeField] private SlidingDoor door; // will try and get the toggle ports
    public OutputPort<Unit> clickPort { get; } = new();

    void Start()
    {
        clickPort.Connect(door._togglePort);
    }

    public void OnTriggerEnter(Collider other) => clickPort.Send(new Unit());
}
