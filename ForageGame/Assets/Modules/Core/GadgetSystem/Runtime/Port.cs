using System;
using System.Collections.Generic;
using UnityEngine;

namespace TDK.GadgetSystem.Runtime
{
    /// <summary>
    /// Base class for all Gadgets. A Gadget is any object that has an on/off state
    /// and can be connected to other Gadgets via wires.
    /// </summary>
    public abstract class Port : MonoBehaviour
    {
        [Header("Port Settings")]
        [SerializeField] private string gadgetLabel = "";
        /// <summary>Display label (falls back to GameObject name).</summary>
        public string Label => string.IsNullOrEmpty(gadgetLabel) ? gameObject.name : gadgetLabel;

        [Header("Gizmo Settings")]
        [SerializeField] private Color wireColor = Color.yellow;
        [SerializeField] private bool showWireLabels = true;
    }
}