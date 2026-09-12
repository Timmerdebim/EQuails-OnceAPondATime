using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Story/Flag")]
public class StoryFlag : ScriptableObject
{
    // cons
    public StoryFlag(string id, bool passesTime = true)
    {
        this.id = id;
        this.passesTime = passesTime;
    }
    //must string match with the event flags in Dialogue.txt
    public string id;

    [Tooltip("Certain Flags are used for cutscenes and other multi-npc syncing stuff. So not all story flags should pass time.")]
    public bool passesTime = true;
}
