using UnityEngine;
using Assets.Modules.Interaction;
using UnityEngine.Events;
using System;
using Project.Items;


//TODO: just a testing script, should be integrated with general World Items I think ~Lars
public class StoryFlagItem : WorldItem
{
    [SerializeField] private StoryFlag flag;

    override public void Interact()
    {
        StoryFlagManager.Instance.AddFlag(flag);
        // Unlock Recipies
        // TODO
        // TODO: show discovery
        Destroy(gameObject);
    }
}
