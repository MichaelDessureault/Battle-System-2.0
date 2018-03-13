using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Backpack))]
public class BackpackEditor : EditorExtension
{

    #region SerializezdProperties
    SerializedProperty potionAmountProp;
    SerializedProperty potionButtonProp;
    SerializedProperty etherAmountProp;
    SerializedProperty etherButtonProp;
    SerializedProperty characterGuiSetupProp;
    
    #region assassin variables
    SerializedProperty assassinBoarderProp;
    SerializedProperty assassinButtonProp;
    #endregion


    #region guardian variables
    SerializedProperty guardianBoarderProp;
    SerializedProperty guardianButtonProp;
    #endregion


    #region wizard variables
    SerializedProperty wizardBoarderProp;
    SerializedProperty wizardButtonProp;
    #endregion
    #endregion

    private bool[] charactersFoldOutArray = new bool[GameManager.numberOfCharacters];

    private SerializedProperty[] boarderProps;
    private SerializedProperty[] buttonProps;

    private void OnEnable() {
        potionAmountProp = serializedObject.FindProperty("potionAmountText");
        potionButtonProp = serializedObject.FindProperty("potionButton");
        etherAmountProp = serializedObject.FindProperty("etherAmountText");
        etherButtonProp = serializedObject.FindProperty("etherButton");
        characterGuiSetupProp = serializedObject.FindProperty("characterGuiSetup");
        
        assassinBoarderProp = serializedObject.FindProperty("assassinBoarder");
        assassinButtonProp = serializedObject.FindProperty("assassinButton");

        guardianBoarderProp = serializedObject.FindProperty("guardianBoarder");
        guardianButtonProp = serializedObject.FindProperty("guardianButton");

        wizardBoarderProp = serializedObject.FindProperty("wizardBoarder");
        wizardButtonProp = serializedObject.FindProperty("wizardButton");

        boarderProps = new SerializedProperty[] { assassinBoarderProp, guardianBoarderProp, wizardBoarderProp };
        buttonProps  = new SerializedProperty[] { assassinButtonProp,  guardianButtonProp,  wizardButtonProp  };

    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        ShowScript(typeof(Backpack)); 

        characterGuiSetupProp.PropertyField();

        EditorGUILayout.LabelField("Potion");
        indentLevel++;
        potionAmountProp.PropertyField("Potion Amount");
        potionButtonProp.PropertyField("Potion Button");
        indentLevel--;

        EditorGUILayout.LabelField("Ether");
        indentLevel++;
        etherAmountProp.PropertyField("Ether Amount");
        etherButtonProp.PropertyField("Ether Button");
        indentLevel--;
        
        EditorGUILayout.LabelField("Characters"); 
        for (int i = 0; i < 3; i++) {
            InsertVertialFoldout(ref charactersFoldOutArray, i, GameManager.characterTypeStrings[i], 
                new SerializedProperty[] { boarderProps[i], buttonProps[i] }, 
                new string[] { "Boarder", "Button" }
            );
        }

        serializedObject.ApplyModifiedProperties();
    }
}