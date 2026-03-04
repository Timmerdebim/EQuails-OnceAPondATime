using System;
using System.Collections.Generic;
using Assets.Modules.Interaction;
using UnityEngine;

namespace Project.Signals
{
    public interface ISignalTarget
    {
        public void ReceiveSignal<T>(T signal);
    }
}