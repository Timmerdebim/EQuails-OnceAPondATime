// using Assets.Modules.Interaction;
// using TDK.GadgetSystem.Runtime;
// using UnityEngine;

// namespace TDK.Gadgets
// {
//     [RequireComponent(typeof(InputPort))]
//     [RequireComponent(typeof(Animator))]
//     public class DoorController : MonoBehaviour, IInteractable
//     {
//         private InputPort _inputPower;
//         private Animator _animator;

//         [SerializeField] private bool _isActive = false;

//         void Awake()
//         {
//             _inputPower = GetComponent<InputPort>();
//             _animator = GetComponent<Animator>();
//         }

//         void Start()
//         {
//             _inputPower.SetSignal(_isActive);
//             _animator.SetBool("isActive", _isActive);
//         }

//         public void Interact()
//         {
//             _isActive = !_isActive;
//             _outputPower.SetSignal(_isActive);
//             _animator.SetBool("isActive", _isActive);
//         }

//         public void Focus()
//         {
//             throw new System.NotImplementedException();
//         }

//         public void Unfocus()
//         {
//             throw new System.NotImplementedException();
//         }
//     }
// }