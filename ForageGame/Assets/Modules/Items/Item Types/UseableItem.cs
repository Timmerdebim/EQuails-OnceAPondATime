using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Animations;
using System;

[CreateAssetMenu(fileName = "New Useable", menuName = "Items/Useable")]
public class UseableItem : Item
{
    [SerializeField] protected PlayerUpgradeType upgradeType = PlayerUpgradeType.Attack;

    public override bool Use() => false;

    public override bool TryPickup()
    {
        if (!Player.Instance) return false;
        Player.Instance?.Upgrade(upgradeType);
        return true;
    }
}
