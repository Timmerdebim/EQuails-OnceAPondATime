#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace TDK.PortSystem
{
    [CustomEditor(typeof(Port), true)]   // 'true' applies to all subclasses
    public class PortInspector : UnityEditor.Editor
    {
        void OnSceneGUI()
        {
        }

        // public override void OnInspectorGUI()
        // {
        //     var gadget = (Gadget)target;

        //     Color old = GUI.backgroundColor;
        //     GUI.backgroundColor = bannerColor;
        //     GUILayout.Box(
        //         gadget.IsActive ? $"● {gadget.Label}  —  ON" : $"○ {gadget.Label}  —  OFF",
        //         bannerStyle,
        //         GUILayout.ExpandWidth(true), GUILayout.Height(28)
        //     );
        //     GUI.backgroundColor = old;

        //     EditorGUILayout.Space(4);

        //     // Runtime controls (play mode only)
        //     if (Application.isPlaying)
        //     {
        //         EditorGUILayout.BeginHorizontal();
        //         if (GUILayout.Button("Toggle", GUILayout.Height(24)))
        //             gadget.Toggle();
        //         if (GUILayout.Button("Force ON", GUILayout.Height(24)))
        //             gadget.SetActive(true);
        //         if (GUILayout.Button("Force OFF", GUILayout.Height(24)))
        //             gadget.SetActive(false);
        //         if (GUILayout.Button("Propagate", GUILayout.Height(24)))
        //             gadget.PropagateSignal();
        //         EditorGUILayout.EndHorizontal();
        //         EditorGUILayout.Space(4);
        //     }

        //     // Connection summary
        //     if (gadget.Outputs != null && gadget.Outputs.Count > 0)
        //     {
        //         EditorGUILayout.LabelField("Output Connections", EditorStyles.boldLabel);
        //         EditorGUI.indentLevel++;
        //         foreach (var output in gadget.Outputs)
        //         {
        //             if (output == null) { EditorGUILayout.LabelField("⚠ null reference"); continue; }

        //             EditorGUILayout.BeginHorizontal();
        //             EditorGUILayout.ObjectField(output, typeof(Gadget), true);
        //             GUIStyle s = new GUIStyle(EditorStyles.miniLabel)
        //             {
        //                 normal = { textColor = output.IsActive ? Color.green : Color.red }
        //             };
        //             GUILayout.Label(output.IsActive ? "ON" : "OFF", s, GUILayout.Width(28));
        //             EditorGUILayout.EndHorizontal();
        //         }
        //         EditorGUI.indentLevel--;
        //         EditorGUILayout.Space(4);
        //     }

        // Draw the normal serialized fields below
        //     DrawDefaultInspector();
        // }
    }
}
#endif