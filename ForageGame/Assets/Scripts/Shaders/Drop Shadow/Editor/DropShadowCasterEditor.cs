using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DropShadowCaster))]
public class DropShadowCasterEditor : Editor
{
    private SerializedProperty materialProperty;
    private DropShadowCaster myTarget;

    private void OnEnable()
    {
        materialProperty = serializedObject.FindProperty("_material");
        myTarget = (DropShadowCaster)target;
    }

    private float NonNegativeFloatField(string label, float value)
    {
        float newValue = EditorGUILayout.FloatField(label, value);
        if (newValue < 0)
            newValue = 0;
        return newValue;
    }

    public override void OnInspectorGUI()
    {
        // Update serialized object
        serializedObject.Update();

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Drop Shadow Parameters", EditorStyles.boldLabel);

        myTarget.Length = NonNegativeFloatField("Shadow Depth", myTarget.Length);
        myTarget.InitialRadius = NonNegativeFloatField("Initial Radius", myTarget.InitialRadius);
        myTarget.FinalRadius = NonNegativeFloatField("Final Radius", myTarget.FinalRadius);
        myTarget.InitialColor = EditorGUILayout.ColorField("Initial Color", myTarget.InitialColor);
        myTarget.FinalColor = EditorGUILayout.ColorField("Final Color", myTarget.FinalColor);

        EditorGUILayout.EndVertical();

        EditorGUILayout.PropertyField(materialProperty, new GUIContent("Drop Shadow Material"));

        // Apply changes to serialized object
        serializedObject.ApplyModifiedProperties();
        myTarget.UpdateDecalProjector();
    }

    private void OnSceneGUI()
    {
        // Get the target component
        myTarget = (DropShadowCaster)target;

        if (myTarget == null)
            return;

        Transform transform = myTarget.transform;

        Vector3 normal = transform.forward;
        Vector3 right = transform.right;
        Vector3 up = transform.up;
        Vector3 position1 = transform.position;
        Vector3 position2 = position1 + myTarget.Length * normal;

        float radius1 = myTarget.InitialRadius;
        float radius2 = myTarget.FinalRadius;

        Handles.DrawLine(position1 + radius1 * up, position2 + radius2 * up);
        Handles.DrawLine(position1 - radius1 * up, position2 - radius2 * up);
        Handles.DrawLine(position1 + radius1 * right, position2 + radius2 * right);
        Handles.DrawLine(position1 - radius1 * right, position2 - radius2 * right);

        Handles.DrawWireDisc(position1, normal, radius1);
        Handles.DrawWireDisc(position2, normal, radius2);

        EditorGUI.BeginChangeCheck();

        float slider1 = RadiusField(position1, normal, right, myTarget.InitialRadius);
        float slider2 = RadiusField(position2, normal, right, myTarget.FinalRadius);

        if (EditorGUI.EndChangeCheck())
        {
            // Ensure radius is not negative
            slider1 = Mathf.Max(0, slider1);
            slider2 = Mathf.Max(0, slider2);

            // Apply the new radius value
            Undo.RecordObject(myTarget, "Change Test Radius");
            myTarget.InitialRadius = slider1;
            myTarget.FinalRadius = slider2;

            // Force the editor to update
            EditorUtility.SetDirty(myTarget);
        }
    }

    private float RadiusField(Vector3 position, Vector3 normal, Vector3 radial, float radius)
    {
        // Draw the radius handle
        Vector3 handlePosition = position + radial * radius;
        Vector3 newhandlePosition = Handles.Slider(handlePosition, radial, HandleUtility.GetHandleSize(handlePosition) / 8, Handles.SphereHandleCap, 0.1f);
        float newRadius = (newhandlePosition - position).magnitude;
        return newRadius;
    }
}