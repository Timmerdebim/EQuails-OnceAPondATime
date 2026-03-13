// #if UNITY_EDITOR
// using System.Collections.Generic;
// using TDK.GadgetSystem.Runtime;
// using UnityEditor;
// using UnityEngine;

// namespace TDK.GadgetSystem.Editor
// {
//     /// <summary>
//     /// Custom editor window for viewing and managing Gadget connections in the scene.
//     /// Open via  Window > Gadget System > Gadget Network Overview
//     /// </summary>
//     public class GadgetNetworkWindow : EditorWindow
//     {
//         private Vector2 _scroll;
//         private bool _autoRefresh = true;
//         private bool _showInactive = true;
//         private string _filter = "";

//         [MenuItem("Window/Gadget System/Gadget Network Overview")]
//         public static void ShowWindow()
//         {
//             var w = GetWindow<GadgetNetworkWindow>("Gadget Network");
//             w.minSize = new Vector2(320, 200);
//         }

//         private void OnGUI()
//         {
//             DrawToolbar();
//             DrawGadgetList();
//         }

//         private void OnInspectorUpdate()
//         {
//             if (_autoRefresh) Repaint();
//         }

//         // ── Toolbar ───────────────────────────────────────────────────────────

//         private void DrawToolbar()
//         {
//             EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

//             _autoRefresh = GUILayout.Toggle(_autoRefresh, "Auto-Refresh", EditorStyles.toolbarButton, GUILayout.Width(90));

//             GUILayout.Label("Filter:", GUILayout.Width(38));
//             _filter = EditorGUILayout.TextField(_filter, EditorStyles.toolbarSearchField, GUILayout.Width(120));

//             _showInactive = GUILayout.Toggle(_showInactive, "Show Inactive", EditorStyles.toolbarButton, GUILayout.Width(95));

//             GUILayout.FlexibleSpace();

//             if (GUILayout.Button("Select All", EditorStyles.toolbarButton, GUILayout.Width(70)))
//                 SelectAllGadgets();

//             EditorGUILayout.EndHorizontal();
//         }

//         // ── Gadget List ───────────────────────────────────────────────────────

//         private void DrawGadgetList()
//         {
//             var gadgets = FindAllGadgets();

//             EditorGUILayout.HelpBox(
//                 $"{gadgets.Count} gadget(s) found in scene.",
//                 MessageType.None);

//             _scroll = EditorGUILayout.BeginScrollView(_scroll);

//             foreach (var g in gadgets)
//             {
//                 if (!_showInactive && !g.IsActive) continue;
//                 if (!string.IsNullOrEmpty(_filter) &&
//                     !g.Label.ToLower().Contains(_filter.ToLower())) continue;

//                 DrawGadgetRow(g);
//             }

//             EditorGUILayout.EndScrollView();
//         }

//         private void DrawGadgetRow(Gadget g)
//         {
//             Color bgOld = GUI.backgroundColor;
//             GUI.backgroundColor = g.IsActive
//                 ? new Color(0.6f, 1f, 0.6f)
//                 : new Color(1f, 0.6f, 0.6f);

//             EditorGUILayout.BeginVertical("box");
//             GUI.backgroundColor = bgOld;

//             // Header row
//             EditorGUILayout.BeginHorizontal();

//             GUIStyle statusStyle = new GUIStyle(EditorStyles.boldLabel)
//             {
//                 normal = { textColor = g.IsActive ? Color.green : Color.red }
//             };
//             GUILayout.Label(g.IsActive ? "● ON" : "○ OFF", statusStyle, GUILayout.Width(52));

//             if (GUILayout.Button(g.Label, EditorStyles.label))
//                 Selection.activeGameObject = g.gameObject;

//             GUILayout.Label($"({g.GetType().Name})", EditorStyles.miniLabel);

//             GUILayout.FlexibleSpace();

//             // Quick-toggle in editor (play mode only)
//             GUI.enabled = Application.isPlaying;
//             if (GUILayout.Button("Toggle", GUILayout.Width(55)))
//                 g.Toggle();
//             GUI.enabled = true;

//             EditorGUILayout.EndHorizontal();

//             // Output connections
//             if (g.Outputs != null && g.Outputs.Count > 0)
//             {
//                 EditorGUI.indentLevel++;
//                 foreach (var output in g.Outputs)
//                 {
//                     if (output == null) continue;
//                     EditorGUILayout.BeginHorizontal();
//                     GUILayout.Space(20);
//                     GUILayout.Label("→", GUILayout.Width(14));
//                     if (GUILayout.Button(output.Label, EditorStyles.miniButton))
//                         Selection.activeGameObject = output.gameObject;
//                     EditorGUILayout.EndHorizontal();
//                 }
//                 EditorGUI.indentLevel--;
//             }

//             EditorGUILayout.EndVertical();
//         }

//         // ── Helpers ───────────────────────────────────────────────────────────

//         private static List<Gadget> FindAllGadgets()
//         {
//             // FindObjectsOfType works in all Unity versions
//             var all = FindObjectsByType<Gadget>(FindObjectsInactive.Include, FindObjectsSortMode.None);
//             return new List<Gadget>(all);
//         }

//         private static void SelectAllGadgets()
//         {
//             var gadgets = FindAllGadgets();
//             var gos = new List<Object>();
//             foreach (var g in gadgets) gos.Add(g.gameObject);
//             Selection.objects = gos.ToArray();
//         }
//     }
// }
// #endif