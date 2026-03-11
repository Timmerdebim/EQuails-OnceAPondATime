// using System;
// using System.Collections.Generic;
// using Assets.Modules.Interaction;
// using Project.Signals.Actuators;
// using UnityEngine;


// namespace Project.Signals.Sources
// {
//     // A button calls out each time it is triggered
//     public class Button : ISignalSource, IActuator
//     {
//         public void OnTrigger() => SignalManager.Instance.SendSignalFrom<EmptySignal>(null, this);
//         public virtual void Focus() { }
//         public virtual void Unfocus() { }
//     }
// }

