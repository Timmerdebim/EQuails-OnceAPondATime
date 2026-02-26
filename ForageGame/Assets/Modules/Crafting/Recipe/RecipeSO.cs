using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Crafting/Recipe")]
public class RecipeSO : ScriptableObject
{
    public List<Item> requiredItems;
    public Item resultItem;
    public bool canRepeatRecipe;
}
