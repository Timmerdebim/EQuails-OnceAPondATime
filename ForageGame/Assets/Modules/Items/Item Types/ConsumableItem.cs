using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Animations;
using System;

[CreateAssetMenu(fileName = "New Consumable", menuName = "Crafting/Consumable")]
public class ConsumableItem : Item
{
    [SerializeField] protected float consumableEnergy;
    [SerializeField] protected Item returnItem;

    public override bool Use()
    {
        if (!Inventory.Instance.hotbar.TryRemoveItemAtCurrent(this))
            return false;

        Player.Instance.energy.TakeDamage(-consumableEnergy); // a little botched but ok...

        if (returnItem != null)
        {
            if (!Inventory.Instance.hotbar.TryAddItemAtAny(returnItem))
                Inventory.Instance.SpawnItemAt(returnItem, Player.Instance.transform.position);
        }
        return true;
    }
}
