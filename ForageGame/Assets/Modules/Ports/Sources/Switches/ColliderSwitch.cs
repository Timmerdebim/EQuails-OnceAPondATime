// using UnityEngine;
// using Project.Ports;
// using Project.Signals.Targets;
// using DG.Tweening;

// [RequireComponent(typeof(Collider))]
// [RequireComponent(typeof(Rigidbody))]
// public class ColliderSwitch : MonoBehaviour
// {
//     [SerializeField] private Transform hingeTransform;
//     [SerializeField] private float rotationAngle = 60f;
//     [SerializeField] private SlidingDoor door; // will try and get the toggle ports
//     public OutputPort<bool> boolPort { get; } = new();
//     private enum State { Open, Closing, Closed, Opening, None }
//     private State currentState = State.None;
//     [SerializeField] bool startPowered = false;
//     private Sequence sequence;

//     void Start()
//     {
//         boolPort.Connect(door._setPort);
//         SetState(startPowered, true);
//     }

//     public void OnTriggerEnter(Collider other) => ToggleSwitch();

//     private void ToggleSwitch()
//     {
//         if (currentState == State.Open) Close();
//         else if (currentState == State.Closed) Open();
//     }

//     private void SetState(bool powered, bool direct = false)
//     {
//         if (powered) Close(direct);
//         else Open(direct);
//     }

//     private void Open(bool direct = false)
//     {
//         if (direct)
//         {
//             sequence?.Kill();
//             hingeTransform.eulerAngles = rotationAngle * Vector3.forward;
//             currentState = State.Open;
//             return;
//         }
//         if (currentState == State.Open || currentState == State.Opening) return;
//         currentState = State.Opening;
//         sequence?.Kill();
//         sequence = DOTween.Sequence()
//         .Append(hingeTransform.DOLocalRotate(rotationAngle * Vector3.forward, 1))
//         .OnComplete(() =>
//         {
//             boolPort.Send(false);
//             currentState = State.Open;
//         });
//     }

//     private void Close(bool direct = false)
//     {
//         if (direct)
//         {
//             sequence?.Kill();
//             hingeTransform.eulerAngles = -rotationAngle * Vector3.forward;
//             currentState = State.Closed;
//             return;
//         }
//         if (currentState == State.Closed || currentState == State.Closing) return;
//         currentState = State.Closing;
//         sequence?.Kill();
//         sequence = DOTween.Sequence()
//         .Append(hingeTransform.DOLocalRotate(-rotationAngle * Vector3.forward, 1))
//         .OnComplete(() =>
//         {
//             boolPort.Send(true);
//             currentState = State.Closed;
//         });
//     }

//     void OnDestroy()
//     {
//         sequence?.Kill();
//     }
// }
