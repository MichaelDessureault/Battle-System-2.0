using System;
using UnityEditor;
using UnityEngine;

public class EditorExtension : Editor {

    /// <summary>
    /// Used to remove the need of typing "EditorGUI." infront of indentLevel
    /// </summary>
    public int indentLevel {
        get { return EditorGUI.indentLevel;  }
        set { EditorGUI.indentLevel = value; }
    }
    
    /// <summary>
    /// Shows the targeted script in the inspector, the field is disabled.
    /// </summary>
    /// <param name="type">use typeof(*scriptname*) when passing the type of the script</param> 
    public void ShowScript(Type type) {
        GUI.enabled = false;
        EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), type, false);
        GUI.enabled = true;
    }

    /// <summary>
    /// Can insert multiple spaces based on the amount passed
    /// </summary>
    public void Space(int amount = 1) {
        if (amount <= 0)
            return;

        for (int i = 0; i < amount; i++) {
            EditorGUILayout.Space();
        }
    }

    /// <summary>
    /// Inserts a vertical layout group that is indented by 1 and contains all the serializedProerties passed.
    /// </summary>
    /// <param name="serializedProperties">Array of the serializedProperties with the vertical</param>
    /// <param name="propertyNames">Array of strings corresponding the serializedProperties array</param>
    /// <param name="withskin">If GUI.skin.box will be used or not</param>
    public void InsertVertical (SerializedProperty[] serializedProperties, string[] propertyNames, bool withskin = false) {
        if (serializedProperties.Length == 0)
            return;

        propertyNames = ValidatePropertyNameArray(serializedProperties, propertyNames);

        if (withskin)
            EditorGUILayout.BeginVertical(GUI.skin.box);
        else
            EditorGUILayout.BeginVertical();

        indentLevel++; 

        for (int i = 0; i < serializedProperties.Length; i++) {
            serializedProperties[i].PropertyField(propertyNames[i]);
        }

        indentLevel--;
        EditorGUILayout.EndVertical();
    }
    
    /// <summary>
    /// Inserts a foldout with a vertical layout group within it.  The vertical layout group is indented by 1 and contains all the serializedProerties passed.
    /// </summary>
    /// <param name="boolArray">ref to the boolArray being used for the foldout</param>
    /// <param name="boolArrayIndex">current index of the foldout</param>
    /// <param name="foldoutTitle">Name of the foldout</param>
    /// <param name="serializedProperties">Array of the serializedProperties with the vertical</param>
    /// <param name="propertyNames">Array of strings corresponding the serializedProperties array</param>
    /// <param name="withskin">If GUI.skin.box will be used or not</param>
    public void InsertVertialFoldout (ref bool[] boolArray, int boolArrayIndex, string foldoutTitle, SerializedProperty[] serializedProperties, string[] propertyNames = null, bool withskin = false) {
        if (serializedProperties.Length == 0)
            return;

        propertyNames = ValidatePropertyNameArray(serializedProperties, propertyNames);

        if (withskin)
            EditorGUILayout.BeginVertical(GUI.skin.box);
        else
            EditorGUILayout.BeginVertical();
       

        boolArray[boolArrayIndex] = EditorGUILayout.Foldout(boolArray[boolArrayIndex], foldoutTitle);

        if (boolArray[boolArrayIndex]) {
            indentLevel++;
            for (int i = 0; i < serializedProperties.Length; i++) {
                serializedProperties[i].PropertyField(propertyNames[i]);
            }
            indentLevel--;
        }
        
        EditorGUILayout.EndVertical();
    }
    
    /// <summary>
    /// Compares the size of the serializedProperties array and propertyNames array, changes the size of the propertyNames array to the correct amount of serializedProperties
    /// If propertyNames array is null a fully new array is created with empty strings foreach index value
    /// </summary>
    /// <returns>The new string array</returns>
    private string[] ValidatePropertyNameArray (SerializedProperty[] serializedProperties, string[] propertyNames = null) {
        if (propertyNames == null) {
            return ArrayExtensions.EmptyStringArray(serializedProperties.Length);
        } else  if (propertyNames.Length == serializedProperties.Length) {
            return propertyNames;
        } else {
            string[] newArray = ArrayExtensions.EmptyStringArray(serializedProperties.Length);
            for (int i = 0; i < propertyNames.Length; i++) {
                newArray[i] = propertyNames[i];
            }
            return newArray;
        }
    }
    
}
