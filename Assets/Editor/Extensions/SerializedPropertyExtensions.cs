using UnityEditor;
using UnityEngine;

public static class SerializedPropertyExtensions {
    public static void PropertyField(this SerializedProperty serializedProperty, string name = "", bool includeChildren = false) {
        if (name.Equals(""))
            EditorGUILayout.PropertyField(serializedProperty, includeChildren);
        else
            EditorGUILayout.PropertyField(serializedProperty, new GUIContent(name), includeChildren);
    }
}