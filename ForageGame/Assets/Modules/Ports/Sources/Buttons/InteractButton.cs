// using Assets.Modules.Interaction;
// using Project.Ports;
// using Project.Signals.Targets;
// using UnityEngine;

// [RequireComponent(typeof(Collider))]
// public class InteractButton : MonoBehaviour, IInteractable
// {
//     [SerializeField] private SlidingDoor door; // will try and get the toggle ports
//     public OutputPort<Unit> clickPort { get; } = new();

//     void Start()
//     {
//         clickPort.Connect(door._togglePort);
//     }

//     public void Interact() => clickPort.Send(new Unit());
//     public void Focus() { }
//     public void Unfocus() { }
// }
