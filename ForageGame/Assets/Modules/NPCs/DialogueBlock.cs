using UnityEngine;
using System.Collections.Generic;
using System;

[System.Serializable]
public struct DialogueBlock
{
    public string blockName;
    public StoryFlag[] requiredFlags;
    public string[] dialogueText;
    public StoryFlag[] flagsToSet;
}
