// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Reflection;
// using UnityEngine;

// namespace Project.Ports
// {
//     public abstract class OutputPortBase : Port
//     {
//         internal abstract void Connect(InputPortBase input);
//         internal abstract void Disconnect(InputPortBase input);
//     }

//     public class OutputPort<T> : OutputPortBase
//     {
//         private List<WeakReference<InputPortBase>> _connections = new();

//         public OutputPort() => DataType = typeof(T);

//         public void Send(T value)
//         {
//             // Iterate over a copy to avoid modification during enumeration
//             foreach (var weakRef in _connections.ToList())
//             {
//                 if (weakRef.TryGetTarget(out var input))
//                     input.ReceiveValue(value);
//                 else
//                     _connections.Remove(weakRef); // Clean up dead references
//             }
//         }

//         internal override void Connect(InputPortBase input)
//         {
//             if (input.DataType != DataType)
//                 throw new ArgumentException($"Cannot connect {DataType} to {input.DataType}");

//             _connections.Add(new WeakReference<InputPortBase>(input));
//         }

//         internal override void Disconnect(InputPortBase input)
//         {
//             var toRemove = _connections.FirstOrDefault(wr => wr.TryGetTarget(out var target) && target == input);

//             if (toRemove != null)
//                 _connections.Remove(toRemove);
//         }
//     }
// }
