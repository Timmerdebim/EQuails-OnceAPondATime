using System;
using UnityEngine;

namespace Project.Ports
{
    public abstract class InputPortBase : Port
    {
        internal abstract void ReceiveValue(object value);
    }

    public class InputPort<T> : InputPortBase
    {
        private Action<T> _handler;

        public InputPort(Action<T> handler = null)
        {
            DataType = typeof(T);
            _handler = handler;
        }

        public void SetHandler(Action<T> handler) => _handler = handler;

        internal override void ReceiveValue(object value)
        {
            if (value is T typedValue)
                _handler?.Invoke(typedValue);
            else
                throw new ArgumentException($"Expected {typeof(T)}, got {value?.GetType()}");
        }
    }
}