using UnityEngine;

[CreateAssetMenu(menuName = "Story/Flag")]
public class StoryFlag : ScriptableObject
{
    //must string match with the event flags in Dialogue.txt
    public string id;
}
