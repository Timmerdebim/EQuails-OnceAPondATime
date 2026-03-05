// using System.Collections.Generic;
// using System.Reflection;
// using UnityEngine;

// namespace Project.Ports
// {
//     public static class PortSystem
//     {
//         private static readonly Dictionary<object, Dictionary<string, Port>> _ports = new();

//         // Call this after constructing an object that uses [HasPorts]
//         public static void RegisterPorts(object owner)
//         {
//             if (!owner.GetType().IsDefined(typeof(HasPortsAttribute), false)) return;
//             var ports = new Dictionary<string, Port>();
//             var type = owner.GetType();

//             // Fields
//             foreach (var feild in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
//             {
//                 var attr = feild.GetCustomAttribute<PortAttribute>();
//                 if (attr == null) continue;

//                 var port = feild.GetValue(owner) as Port;
//                 if (port == null) continue;

//                 string name = attr.Name ?? feild.Name;
//                 port.Name = name;
//                 port.Owner = owner;
//                 ports[name] = port;
//             }

//             // Properties
//             foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
//             {
//                 var attr = prop.GetCustomAttribute<PortAttribute>();
//                 if (attr == null) continue;

//                 var port = prop.GetValue(owner) as Port;
//                 if (port == null) continue;

//                 string name = attr.Name ?? prop.Name;
//                 port.Name = name;
//                 port.Owner = owner;
//                 ports[name] = port;
//             }

//             lock (_ports)
//                 _ports[owner] = ports;
//         }

//         public static void UnregisterPorts(object owner)
//         {
//             lock (_ports)
//                 _ports.Remove(owner);
//         }

//         public static Port GetPort(object owner, string portName)
//         {
//             lock (_ports)
//                 if (_ports.TryGetValue(owner, out var ports) && ports.TryGetValue(portName, out var port))
//                     return port;
//             return null;
//         }

//         public static void Connect(OutputPortBase source, InputPortBase target) => source.Connect(target);
//         public static void Disconnect(OutputPortBase source, InputPortBase target) => source.Disconnect(target);
//     }
// }
