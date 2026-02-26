using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Animations;
using System;

[CreateAssetMenu(fileName = "New Bottle", menuName = "Items/Bottle")]
public class Bottle : Item
{
    [SerializeField] protected Item waterBottle;
    [SerializeField] protected Item pollenBottle;
    [SerializeField] protected Item sporeBottle;
    [SerializeField] protected Item fireflyBottle;

    public override bool Use()
    {
        // // Get colliders and check if any of them have the correct tag
        // // if standing in water
        // {
        //     return TryUseBottleToGetItem(waterBottle);
        // }
        // // if standing in pollen cloud
        // {
        //     return TryUseBottleToGetItem(pollenBottle);
        // }
        // // if standing in spore cloud
        // {
        //     return TryUseBottleToGetItem(sporeBottle);
        // }
        // // if standing in firefly cloud
        // {
        //     return TryUseBottleToGetItem(fireflyBottle);
        // }

        return true;
    }

    private bool TryUseBottleToGetItem(Item item)
    {
        if (!Inventory.Instance.hotbar.TryRemoveItemAtCurrent(this))
            return false;

        if (!Inventory.Instance.hotbar.TryAddItemAtAny(item))
            Inventory.Instance.SpawnItemAt(item, Player.Instance.transform.position);
        return true;
    }
}
