using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(PinchableScrollRect))]
public class PinchableScrollRectEditor : ScrollRectEditor
{
	#region Fields
    private SerializedProperty _minZoom = null;
    private SerializedProperty _maxZoom = null;
	#endregion Fields

	#region Properties
	#endregion Properties

	#region Methods
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
 
        EditorGUILayout.PropertyField(_minZoom);
        EditorGUILayout.PropertyField(_maxZoom);

        serializedObject.ApplyModifiedProperties();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        _minZoom = serializedObject.FindProperty("_minZoom");
        _maxZoom = serializedObject.FindProperty("_maxZoom");
    }
    
	#endregion Methods
}