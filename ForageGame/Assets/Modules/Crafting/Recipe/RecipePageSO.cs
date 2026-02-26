using UnityEngine;

[CreateAssetMenu(fileName = "RecipePage", menuName = "Crafting/RecipePage")]
public class RecipePageSO : ScriptableObject
{
    public string Name;
    public Sprite pageSprite;
    [Tooltip("Position in the scrapbook this page will be shown, not used for now lolol")]
    public int pageIndex;
}
