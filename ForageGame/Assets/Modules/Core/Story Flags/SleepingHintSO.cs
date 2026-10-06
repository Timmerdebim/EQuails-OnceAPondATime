using UnityEngine;
using System.Collections.Generic;
using TDK.ItemSystem;

[System.Serializable]
[CreateAssetMenu(menuName = "Story/SleepingHint")]
public class SleepingHint : ScriptableObject
{
    public List<StoryFlag> requiredFlags;
    public List<ItemData> requiredItems;
    public List<StoryFlag> absentFlags;
    public List<ItemData> absentItems;
    public string hint;
}
