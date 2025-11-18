using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class DuckUIEditor : EditorWindow {
    private ProgressBar energyBar;
    
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    [MenuItem("Window/UI Toolkit/DuckUIEditor")]
    public static void ShowExample()
    {
        DuckUIEditor wnd = GetWindow<DuckUIEditor>();
        wnd.titleContent = new GUIContent("DuckUIEditor");
    }

    public void CreateGUI()
    {
        energyBar = rootVisualElement.Q<ProgressBar>("energybar");
        energyBar.value = 0.9f;
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // VisualElements objects can contain other VisualElement following a tree hierarchy.
        VisualElement label = new Label("Hello World! From C#");
        root.Add(label);

        // Instantiate UXML
        VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
        root.Add(labelFromUXML);
    }
}
