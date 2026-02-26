using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Animations;
using System;

[CreateAssetMenu(fileName = "New Item", menuName = "Crafting/Item")]
public class Item : ScriptableObject
{
    [SerializeField] protected string itemName;
    [SerializeField] protected string description;
    [SerializeField] protected Sprite sprite;

    public int GetId() => Inventory.Instance.GetIdByItem(this);
    public string GetName() => itemName;
    public string GetDescription() => description;
    public Sprite GetSprite() => sprite;

    public virtual bool Use()
    {
        // Input use function; return true if executed successfully.
        return true;
    }
}
