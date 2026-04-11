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

        private LineReader _reader;
        private Dictionary<string, NpcLocation> _locations;
        private Dictionary<string, UnityEvent> _actions;
        private Dictionary<string, ItemData> _items;
        public DialogueDatabase Parse(string fileContents, 
                                            Dictionary<string, NpcLocation> locations, 
                                            Dictionary<string, UnityEvent> dialogueActions, 
                                            Dictionary<string, ItemData> items)
        {
            _locations = locations;
            _actions = dialogueActions;
            _items = items;

            DialogueDatabase db = new DialogueDatabase();

            _reader = new LineReader(fileContents.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None));
            while (_reader.HasLines)
            {
                Debug.Log(_reader.Consume());
            }

            return db;
        }

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