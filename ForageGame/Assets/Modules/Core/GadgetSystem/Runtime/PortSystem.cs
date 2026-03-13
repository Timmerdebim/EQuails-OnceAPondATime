using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TDK.GadgetSystem.Runtime
{
    public static class PortSystem
    {

        public static TwoWayDict<OutputPort, InputPort> Connections = new();


        public static void SendSignalFromSource(bool signal, OutputPort source)
        {
            foreach (InputPort target in Connections.GetTargetsBySource(source))
                target.ReceiveSignalFrom(signal, source);
        }

        public static List<bool> GetSignalsToTarget(InputPort target)
        {
            List<bool> signals = new();
            foreach (OutputPort source in Connections.GetSourcesByTarget(target))
                signals.Add(source.Signal);
            return signals;
        }
    }
}