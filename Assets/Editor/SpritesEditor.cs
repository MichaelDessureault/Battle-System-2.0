using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(Sprites))]
public class SpritesEditor : EditorExtension {
    #region SerializedProperties
    SerializedProperty enemySpritesProp;
    SerializedProperty characterFrontSpritesProp;
    SerializedProperty characterBackSprites;
    SerializedProperty battleSceneBackgroundMaterialsProp;
    #endregion
    
    private bool[] showCharacterSlots = new bool[GameManager.numberOfCharacters];
    private void OnEnable() {
        enemySpritesProp = serializedObject.FindProperty("enemySprites");
        characterFrontSpritesProp = serializedObject.FindProperty("characterFrontSprites");
        characterBackSprites = serializedObject.FindProperty("characterBackSprites");
        battleSceneBackgroundMaterialsProp = serializedObject.FindProperty("battleSceneBackgroundMaterials");
    }

    private bool characterSprites;

    public override void OnInspectorGUI() {
        serializedObject.Update(); 

        ShowScript(typeof(Sprites));

        enemySpritesProp.PropertyField(includeChildren: true);
        battleSceneBackgroundMaterialsProp.PropertyField(includeChildren: true);
         
        characterSprites = EditorGUILayout.Foldout(characterSprites, new GUIContent("Character Sprites"));
        
        if (characterSprites) {
            indentLevel++;
            for (int i = 0; i < 3; i++) {
                InsertVertialFoldout(ref showCharacterSlots, i, GameManager.characterTypeStrings[i],
                    new SerializedProperty[] { characterFrontSpritesProp.GetArrayElementAtIndex(i), characterBackSprites.GetArrayElementAtIndex(i) },
                    new string[] { "Front Image", "Back Image" }
                );
            }
            indentLevel--;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
