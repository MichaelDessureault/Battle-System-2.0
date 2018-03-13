using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(CharacterGuiSetup))]
public class CharacterGuiSetupEditor : EditorExtension
{

    #region SerializezdProperties
    #region assassin variables
    SerializedProperty assassinHealthBarProp;
    SerializedProperty assassinNameProp;
    SerializedProperty assassinLevelProp;
    SerializedProperty assassinAbilitiesProp;
    SerializedProperty assassinAbilitiesPPProp;
    #endregion
     
    #region guardian variables
    SerializedProperty guardianHealthBarProp;
    SerializedProperty guardianNameProp;
    SerializedProperty guardianLevelProp;
    SerializedProperty guardianAbilitiesProp;
    SerializedProperty guardianAbilitiesPPProp;
    #endregion


    #region wizard variables
    SerializedProperty wizardHealthBarProp;
    SerializedProperty wizardNameProp;
    SerializedProperty wizardLevelProp;
    SerializedProperty wizardAbilitiesProp;
    SerializedProperty wizardAbilitiesPPProp;
    #endregion
    #endregion

    private bool[] charactersFoldOutArray = new bool[GameManager.numberOfCharacters];
    private bool[] abilityFoldOutArray1 = new bool[Ability.KMaxAbilities];
    private bool[] abilityFoldOutArray2 = new bool[Ability.KMaxAbilities];
    private bool[] abilityFoldOutArray3 = new bool[Ability.KMaxAbilities];

    private SerializedProperty[] assassinProps;
    private SerializedProperty[] guardianProps;
    private SerializedProperty[] wizardProps;
    private SerializedProperty[][] charactersProp;

    private void OnEnable() {
        assassinHealthBarProp = serializedObject.FindProperty("assassinHealthBar");
        assassinNameProp = serializedObject.FindProperty("assassinName");
        assassinLevelProp = serializedObject.FindProperty("assassinLevel");
        assassinAbilitiesProp = serializedObject.FindProperty("assassinAbilities");
        assassinAbilitiesPPProp = serializedObject.FindProperty("assassinAbilitiesPP");

        guardianHealthBarProp = serializedObject.FindProperty("guardianHealthBar");
        guardianNameProp = serializedObject.FindProperty("guardianName");
        guardianLevelProp = serializedObject.FindProperty("guardianLevel");
        guardianAbilitiesProp = serializedObject.FindProperty("guardianAbilities");
        guardianAbilitiesPPProp = serializedObject.FindProperty("guardianAbilitiesPP");

        wizardHealthBarProp = serializedObject.FindProperty("wizardHealthBar");
        wizardNameProp = serializedObject.FindProperty("wizardName");
        wizardLevelProp = serializedObject.FindProperty("wizardLevel");
        wizardAbilitiesProp = serializedObject.FindProperty("wizardAbilities");
        wizardAbilitiesPPProp = serializedObject.FindProperty("wizardAbilitiesPP");

        assassinProps  = new SerializedProperty[]   { assassinHealthBarProp, assassinNameProp, assassinLevelProp, assassinAbilitiesProp, assassinAbilitiesPPProp };
        guardianProps  = new SerializedProperty[]   { guardianHealthBarProp, guardianNameProp, guardianLevelProp, guardianAbilitiesProp, guardianAbilitiesPPProp };
        wizardProps    = new SerializedProperty[]   { wizardHealthBarProp,   wizardNameProp,   wizardLevelProp,   wizardAbilitiesProp,   wizardAbilitiesPPProp   };
        charactersProp = new SerializedProperty[][] { assassinProps, guardianProps, wizardProps };
    }

    private bool charactersFoldout;
    public override void OnInspectorGUI() {
        serializedObject.Update();

        ShowScript(typeof(CharacterGuiSetup));

        charactersFoldout = EditorGUILayout.Foldout(charactersFoldout, new GUIContent("Characters"));
        if (charactersFoldout) {
            indentLevel++;
            for (int i = 0; i < 3; i++) {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                charactersFoldOutArray[i] = EditorGUILayout.Foldout(charactersFoldOutArray[i], new GUIContent(GameManager.characterTypeStrings[i]));

                if (charactersFoldOutArray[i]) {
                    InsertVertical( new SerializedProperty[] { charactersProp[i][0], charactersProp[i][1], charactersProp[i][2] },
                                    new string[] { "Health Bar", "Name", "Level" });

                    indentLevel++;
                    for (int j = 0; j < Ability.KMaxAbilities; j++) {
                        InsertVertialFoldout(ref abilityFoldOutArray1, j, "Ability " + (j + 1),
                            new SerializedProperty[] { charactersProp[i][3].GetArrayElementAtIndex(j), charactersProp[i][4].GetArrayElementAtIndex(j) },
                            new string[] { "Text", "PP" }
                        );
                    }
                    indentLevel--; 
                } 
                EditorGUILayout.EndVertical(); 
            }
            indentLevel--;
        } 
        serializedObject.ApplyModifiedProperties();
    }
}