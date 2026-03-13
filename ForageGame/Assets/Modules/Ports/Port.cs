// using System;
// using UnityEngine;

// namespace Project.Ports
// {
//     // [AttributeUsage(AttributeTargets.Class)]
//     // public class HasPortsAttribute : Attribute { }

//     // [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
//     // public class PortAttribute :
//     // Attribute
//     // {
//     //     public string Name { get; }
//     //     public PortAttribute(string name = null) => Name = name;
//     // }

//     public abstract class Port
//     {
//         public string Name { get; internal set; }
//         public object Owner { get; internal set; }
//         public Type DataType { get; internal set; }
//     }

//     // The Unit class represents "no data" - like void but usaeble as a type
//     public readonly struct Unit : IEquatable<Unit>
//     {
//         public static readonly Unit Value = new Unit();
//         public bool Equals(Unit other) => true;
//         public override bool Equals(object obj) => obj is Unit;
//         public override int GetHashCode() => 0;
//         public override string ToString() => "()";
//     }
// }