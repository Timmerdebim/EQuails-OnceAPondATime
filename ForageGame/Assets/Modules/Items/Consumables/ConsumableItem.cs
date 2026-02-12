using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Animations;
using System;

[CreateAssetMenu(fileName = "New Consumable Item", menuName = "Crafting/ConsumableItem")]
public class ConsumableItem : Item
{
    public float consumableEnergy;
    public Item returnItem;
}
