using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using TDK.ItemSystem;

namespace NPC
{
    public class DialogueParser: MonoBehaviour
    {
        //Stateful reading of lines
        private class LineReader
        {
            private readonly string[] _lines;
            private int _index;

            public LineReader(string[] lines) => _lines = lines;
            public bool HasLines => _index < _lines.Length;
            public string Peek() => _lines[_index].Trim();

            public string Consume()
            {
                string line = _lines[_index].Trim();
                _index++;
                return line;
            }
            public void SkipEmpty()
            {
                while (HasLines && string.IsNullOrEmpty(Peek())) Consume();
            }
        }

        private LineReader reader;
        private Dictionary<string, NpcLocation> _locations;
        private Dictionary<string, UnityEvent> _actions;
        private Dictionary<string, ItemData> _items;
        private Dictionary<string, StoryFlag> _flags;
        public DialogueDatabase Parse(string fileContents, 
                                            Dictionary<string, StoryFlag> flags, 
                                            Dictionary<string, ItemData> items,
                                            Dictionary<string, NpcLocation> locations, 
                                            Dictionary<string, UnityEvent> dialogueActions)
        {
            _locations = locations;
            _actions = dialogueActions;
            _items = items;
            _flags = flags;

            DialogueDatabase db = new DialogueDatabase();

            reader = new LineReader(fileContents.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None));
            while (reader.HasLines)
            {
                reader.SkipEmpty();
                if (!reader.HasLines) break; //end of file safegaurd

                var currentLine = reader.Consume();
                Debug.Log(currentLine);

                if (currentLine.StartsWith("---")) //StoryStage marker
                    db.storyStages.Add(ParseStoryStage());
                else //anything else is incorrect input
                {
                    Debug.LogError($"Unknown line skipped: {currentLine}");
                }
            }
            return db;
        }

        private StoryStage ParseStoryStage()
        {
            StoryStage stage = new StoryStage();

            Debug.Log("Parsing a StoryStage!");

            while (reader.HasLines)
            {
                reader.SkipEmpty();
                string line = reader.Peek();
                if (line.StartsWith("---")) break; // next stage
                else if (line.StartsWith("Flags:", StringComparison.OrdinalIgnoreCase))
                {
                    stage.RequiredFlags = ParseReferenceList(reader.Consume(), _flags, "Flags");
                }
                else if (line.StartsWith("Required-Items:")) //Items
                {
                    if(stage.requiredItems.Count > 0) Debug.LogError("Duplicate \"Required-Items:\" attribute in StoryStage!");
                    stage.requiredItems = ParseReferenceList(reader.Consume(), _items, "Items");
                }
                else if (line.StartsWith("Terminal:")) //LocationDialogue
                {
                    var locations = ParseReferenceList(reader.Consume(), _locations, "Terminal");
                    if (locations.Count > 1) //safegaurd for multiple entered locations
                        Debug.LogError("[DialogueParser] NpcLocation: expects exactly one location ID, expect duplicate dialogue (or problems)");
                    foreach (NpcLocation loc in locations)
                    {
                        stage.locationDialogues[loc] = ParseLocationDialogue();
                    }
                }
                else //everything else is WRONG
                {
                    Debug.LogWarning($"[DialogueParser] Unexpected line in StoryStage: '{reader.Consume()}'");
                }
            }
            return stage;
        }

        /// <summary>
        /// This one is a mess
        /// Don't feel like fixing it
        /// </summary>
        private LocationDialogue ParseLocationDialogue()
        {
            var ld = new LocationDialogue();

            while (reader.HasLines)
            {
                reader.SkipEmpty();
                if (!reader.HasLines) break; //end of file

                string line = reader.Peek();

                // exit conditions - do not consume, parent owns these
                if (line.StartsWith("---") || line.StartsWith("Terminal:")) break;

                if (line.StartsWith("<")) //main or flavor text marker
                    ld.isMainDialogue = ParseValue(reader.Consume()).Equals("<Main>", StringComparison.OrdinalIgnoreCase);

                else if (line.StartsWith("Stage:"))
                    ld.Lines.Add(ParseDialogueLine());

                else
                    Debug.LogWarning($"[DialogueParser] Unexpected line in LocationDialogue: '{reader.Consume()}'");
            }
            return ld;
        }

        private DialogueLine ParseDialogueLine()
        {
            var dl = new DialogueLine();
            reader.Consume();
            return dl;
        }

        #region Utility
        //trim off the pre-value stuffs
        private string ParseValue(string line)
        {
            int idx = line.IndexOf(':');
            return idx == -1 ? "" : line[(idx + 1)..].Trim();
        }

        private List<string> ParseValueList(string line)
        {
            string val = ParseValue(line);
            if (string.IsNullOrEmpty(val)) return new List<string>();
            return val.Split(',').Select(s => s.Trim())
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToList();
        }

        private List<T> ParseReferenceList<T>(string line, Dictionary<string, T> lookup, string context) //context is what we were trying to parse, i.e., StoryFlag
        {
            return ParseValueList(line).Select(key =>
            {
                if (lookup.TryGetValue(key, out var value)) return (true, value);
                Debug.LogError($"[DialogueParser] Unknown key '{key}' in {context}"); //DOES NOT WORK!
                return (false, default);
            })
            .Where(r => r.Item1)
            .Select(r => r.Item2)
            .ToList();
        }
        #endregion
    }






    public static class OLDDialogueParser
    {
        public static DialogueDatabase Parse(List<string> fileContents, 
                                            Dictionary<string, NpcLocation> locations, 
                                            Dictionary<string, UnityEvent> dialogueActions, 
                                            Dictionary<string, ItemData> items)
        {
            DialogueDatabase db = new DialogueDatabase();
            StoryStage currentStage = null;
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

                    // 1. CHARACTER HEADER IS GONE NOW ~Lars
                    // 2. REQUIRED FLAGS (Start of Block)
                    if (line.StartsWith("Flags:", StringComparison.OrdinalIgnoreCase))
                    {
                        currentStage = new StoryStage();
                        
                        string val = ParseValue(line);
                        if (!val.Equals("<none>", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(val))
                        {
                            foreach (var raw in val.Split(',').Select(s => s.Trim()))
                            {
                                if (StoryFlagManager.Instance.TryGetStoryFlag(raw, out var flag))
                                {
                                    currentStage.RequiredFlags.Add(flag);
                                }
                                else
                                {
                                    Debug.LogError($"Unknown story flag '{raw}' in dialogue.txt");
                                }
                            }
                        }
                        db.storyStages.Add(currentStage);
                    }
                    // 3. SET FLAGS
                    else if (line.StartsWith("Set-Flags:", StringComparison.OrdinalIgnoreCase))
                    {
                        if (currentStage == null) continue;

                        string val = ParseValue(line);
                        if (!val.Equals("<none>", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(val))
                        {
                            // Parse simply as strings to pass to the Manager later
                            var flagsToSet = val.Split(',').Select(s => s.Trim());
                            //currentStage.SetFlags.AddRange(flagsToSet); TODO: THIS DOES NOTHING BEWARE!!!
                        }
                    }
                    // 4. STAGE/LINE CONTENT
                    else if (line.StartsWith("Stage:", StringComparison.OrdinalIgnoreCase))
                    {
                        if (currentStage == null) continue;
                        
                        string stageInfo = ParseValue(line);
                        currentLine = new DialogueLine { StageID = stageInfo, Text = "" };
                        //currentStage.Lines.Add(currentLine); TODO: THIS DOES NOTHING BEWARE!!!
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