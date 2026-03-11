using Project.Ports;
using Project.Signals.Targets;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HitButton : MonoBehaviour, IHitHandler
{
    [SerializeField] private SlidingDoor door; // will try and get the toggle ports
    public OutputPort<Unit> clickPort { get; } = new();

    void Start()
    {
        clickPort.Connect(door._togglePort);
    }

    public void Hit(float value) => clickPort.Send(new Unit());
}
