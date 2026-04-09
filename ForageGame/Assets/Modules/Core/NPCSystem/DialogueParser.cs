using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NPC
{
    public static class DialogueParser
    {
        public static DialogueDatabase Parse(List<string> fileContents)
        {
            DialogueDatabase db = new DialogueDatabase();
            CharacterProfile currentCharacter = null;
            DialogueBlock currentBlock = null;
            DialogueLine currentLine = null;

            foreach (var content in fileContents)
            {
                string[] lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

                foreach (var rawLine in lines)
                {
                    string line = rawLine;
                    if (line.Contains("#")) line = line.Split('#')[0];
                    line = line.Trim();

                    if (string.IsNullOrEmpty(line)) continue;
                    if (line.StartsWith("---")) continue;

                    // 1. CHARACTER HEADER
                    if (line.StartsWith("Character:", StringComparison.OrdinalIgnoreCase))
                    {
                        string name = ParseValue(line);
                        currentCharacter = db.GetCharacter(name);
                        if (currentCharacter == null)
                        {
                            currentCharacter = new CharacterProfile { Name = name };
                            db.Characters.Add(currentCharacter);
                        }
                        currentBlock = null;
                    }
                    // 2. REQUIRED FLAGS (Start of Block)
                    else if (line.StartsWith("Flags:", StringComparison.OrdinalIgnoreCase))
                    {
                        if (currentCharacter == null) continue;
                        currentBlock = new DialogueBlock();
                        
                        string val = ParseValue(line);
                        if (!val.Equals("<none>", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(val))
                        {
                            foreach (var raw in val.Split(',').Select(s => s.Trim()))
                            {
                                if (StoryFlagManager.Instance.TryGetStoryFlag(raw, out var flag))
                                {
                                    currentBlock.RequiredFlags.Add(flag);
                                }
                                else
                                {
                                    Debug.LogError($"Unknown story flag '{raw}' in dialogue.txt");
                                }
                            }
                        }
                        
                        currentCharacter.Blocks.Add(currentBlock);
                    }
                    // 3. SET FLAGS
                    else if (line.StartsWith("Set-Flags:", StringComparison.OrdinalIgnoreCase))
                    {
                        if (currentBlock == null) continue;

                        string val = ParseValue(line);
                        if (!val.Equals("<none>", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(val))
                        {
                            // Parse simply as strings to pass to the Manager later
                            var flagsToSet = val.Split(',').Select(s => s.Trim());
                            currentBlock.SetFlags.AddRange(flagsToSet);
                        }
                    }
                    // 4. STAGE/LINE CONTENT
                    else if (line.StartsWith("Stage:", StringComparison.OrdinalIgnoreCase))
                    {
                        if (currentBlock == null) continue;
                        
                        string stageInfo = ParseValue(line);
                        currentLine = new DialogueLine { StageID = stageInfo, Text = "" };
                        currentBlock.Lines.Add(currentLine);
                    }
                    // 5. TEXT BODY
                    else
                    {
                        if (currentLine != null)
                        {
                            if (!string.IsNullOrEmpty(currentLine.Text)) currentLine.Text += "\n";
                            currentLine.Text += line;
                        }
                    }
                }
            }
            return db;
        }

        private static string ParseValue(string line)
        {
            int idx = line.IndexOf(':');
            return (idx == -1) ? "" : line.Substring(idx + 1).Trim();
        }
    }
}