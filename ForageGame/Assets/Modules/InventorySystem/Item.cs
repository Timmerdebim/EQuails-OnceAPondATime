using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Animations;
using System;

[CreateAssetMenu(fileName = "New Item", menuName = "Crafting/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public string description;
    public Sprite icon;
    public bool isConsumable;
    public float consumableEnergy;
    public GameObject worldPrefab; // The prefab to spawn when dropped
}
