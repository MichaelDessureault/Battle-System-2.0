using UnityEditor;
using UnityEngine;

[CustomEditor (typeof(CharacterSelectionManager))]
class CharacterSelectionManagerEditor : EditorExtension
{
    #region SerializedProperties
    SerializedProperty deathColorProp;
    SerializedProperty enemyBlurProp;
    SerializedProperty characterSelectionContainerProp;
    SerializedProperty charactersRawImageArrayProp;
    SerializedProperty charactersHealthBarArrayProp;
    #endregion

    private bool[] showCharacterSlots = new bool[GameManager.numberOfCharacters];
    private bool characterFoldout;

    private void OnEnable() {
        deathColorProp = serializedObject.FindProperty("deathColor");
        enemyBlurProp  = serializedObject.FindProperty("enemyBlur");
        characterSelectionContainerProp = serializedObject.FindProperty("characterSelectionContainer");
        charactersRawImageArrayProp  = serializedObject.FindProperty("charactersRawImageArray");
        charactersHealthBarArrayProp = serializedObject.FindProperty("charactersHealthBarArray");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update(); 

        ShowScript(typeof(CharacterSelectionManager));

        deathColorProp.PropertyField("Death Color");
        enemyBlurProp.PropertyField("Enemy Blur Image");
        characterSelectionContainerProp.PropertyField("Character Selection Container");
        
        characterFoldout = EditorGUILayout.Foldout(characterFoldout, "Character Raw Images And Healthbars");
        if (characterFoldout) {
            indentLevel++;
            for (int i = 0; i < GameManager.numberOfCharacters; i++) {
                InsertVertialFoldout(ref showCharacterSlots, i, GameManager.characterTypeStrings[i], 
                    new SerializedProperty[] { charactersRawImageArrayProp.GetArrayElementAtIndex(i), charactersHealthBarArrayProp.GetArrayElementAtIndex(i) },
                    new string[] { GameManager.characterTypeStrings[i], "Health Bar" }
                );
            }
            indentLevel--;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
