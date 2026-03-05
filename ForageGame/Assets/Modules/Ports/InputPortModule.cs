// using System;
// using System.Collections.Generic;
// using Project.Ports;
// using UnityEngine;


// namespace Project.Ports.Inspector
// {
//     public enum PortEntryDataType { Unit, Bool, Int, Float, String }

//     [Serializable]
//     public abstract class PortEntry
//     {
//         public string portName;
//         public PortEntryDataType dataType;
//     }

//     [Serializable]
//     public class InputPortEntry : PortEntry
//     {
//         public Action reaction;
//     }

//     [Serializable]
//     public struct InputPortReference
//     {
//         public GameObject targetGameObject; // port module 
//         public string inputPortName;
//     }

//     [Serializable]
//     public class OutputPortEntry : PortEntry
//     {
//         public List<InputPortReference> connections = new();
//     }

//     public class PortModule : MonoBehaviour
//     {
//         [SerializeField] private List<InputPortEntry> initialInputPorts = new();
//         [SerializeField] private readonly List<OutputPortEntry> initialOutputPorts = new();

//         // Runtime port instances
//         private List<InputPortBase> inputPorts = new();
//         private List<OutputPortBase> outputPorts = new();

//         private void Awake() => CreatePorts();

//         private void CreatePorts()
//         {
//             inputPorts = PortModuleSystem.CreateInputPorts(initialInputPorts);
//             outputPorts = PortModuleSystem.CreateOutputPorts(initialOutputPorts);
//         }

//         private void ConnectPorts()
//         {
//             inputPorts = PortModuleSystem.CreateInputPorts(initialInputPorts);
//             outputPorts = PortModuleSystem.CreateOutputPorts(initialOutputPorts);
//         }


//         // public void SendSignal(string portName, object value)
//         // {
//         //     if (outputPorts.TryGetValue)
//         // }

//         // Public methods to send values from code
//         public void SendBool(string portName, bool value)
//         {
//             if (_runtimeOutputs.TryGetValue(portName, out var port) && port is OutputPort<bool> boolPort)
//                 boolPort.Send(value);
//         }

//         public void SendInt(string portName, int value)
//         {
//             if (_runtimeOutputs.TryGetValue(portName, out var port) && port is OutputPort<int> intPort)
//                 intPort.Send(value);
//         }

//         // ... similar for float, string

//         public InputPortBase GetInputPort(string portName)
//         {
//             _runtimeInputs.TryGetValue(portName, out var port);
//             return port;
//         }


//         private void EnablePortModule()
//         {

//             PortSystem.RegisterPorts(this);

//         }

//         private void DisablePortModule() => PortSystem.UnregisterPorts(this);
//     }

//     public static class PortModuleSystem
//     {
//         public static List<InputPortBase> CreateInputPorts(List<InputPortEntry> inputPortEntries)
//         {
//             List<InputPortBase> inputPorts = new();
//             foreach (InputPortEntry entry in inputPortEntries)
//             {
//                 if (string.IsNullOrEmpty(entry.portName)) continue;
//                 var inputPort = CreateInputPort(entry);
//                 if (inputPort != null) inputPorts.Add(inputPort);
//             }
//             return inputPorts;
//         }

//         public static InputPortBase CreateInputPort(InputPortEntry entry)
//         {
//             switch (entry.dataType)
//             {
//                 case PortEntryDataType.Bool:
//                     var boolPort = new InputPort<bool>(value => InvokeUnityEvent<bool>(entry.onReceiveEvent, value));
//                     return boolPort;
//                 case PortEntryDataType.Int:
//                     var intPort = new InputPort<int>(value => InvokeUnityEvent<int>(entry.onReceiveEvent, value));
//                     return intPort;
//                 case PortEntryDataType.Float:
//                     var floatPort = new InputPort<float>(value => InvokeUnityEvent<float>(entry.onReceiveEvent, value));
//                     return floatPort;
//                 case PortEntryDataType.String:
//                     var stringPort = new InputPort<string>(value => InvokeUnityEvent<string>(entry.onReceiveEvent, value));
//                     return stringPort;
//                 default:
//                     return null;
//             }
//         }

//         public static List<OutputPortBase> CreateOutputPorts(List<OutputPortEntry> outputPortEntries)
//         {
//             List<OutputPortBase> outputPorts = new();

//             foreach (OutputPortEntry entry in outputPortEntries)
//             {
//                 if (string.IsNullOrEmpty(entry.portName)) continue;
//                 var outputPort = CreateOutputPort(entry);
//                 if (outputPort != null)
//                 {
//                     outputPorts.Add(outputPort);

//                     // Connect each target
//                     foreach (var conn in entry.connections)
//                     {
//                         if (conn.targetGameObject == null || string.IsNullOrEmpty(conn.inputPortName))
//                             continue;

//                         var targetModule = conn.targetGameObject.GetComponent<PortModule>();
//                         if (targetModule == null) continue;

//                         var targetInput = targetModule.GetInputPort(conn.inputPortName);
//                         if (targetInput != null)
//                         {
//                             // Use the PortManager to connect (or connect directly)
//                             // For simplicity, we assume outputPort has a Connect method
//                             outputPort.ConnectTo(targetInput);
//                         }
//                     }
//                 }
//             }
//             return outputPorts;
//         }

//         public static OutputPortBase CreateOutputPort(OutputPortEntry entry)
//         {
//             switch (entry.dataType)
//             {
//                 case PortEntryDataType.Bool: return new OutputPort<bool>();
//                 case PortEntryDataType.Int: return new OutputPort<int>();
//                 case PortEntryDataType.Float: return new OutputPort<float>();
//                 case PortEntryDataType.String: return new OutputPort<string>();
//                 default: return null;
//             }
//         }

//     }
// }