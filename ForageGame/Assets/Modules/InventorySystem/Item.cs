using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Animations;
using System;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public string description { get; private set; }
    public Sprite icon;
    public bool isConsumable { get; private set; }
    float consumableEnergy;
    public GameObject worldPrefab; // The prefab to spawn when dropped
}
