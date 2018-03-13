using System.Collections;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BattleGUIManager))]
public class BattleGUIManagerEditor : EditorExtension {
    #region SerializedProperties 
    #region Selection / Ability Window
    SerializedProperty selectionWindowProp;
    SerializedProperty abilityButtonsTextProp;
    SerializedProperty abilityPPTextProp;
    SerializedProperty backButtonTextProp;
    #endregion

    #region BackPack Store window
    SerializedProperty backpackStoreWindowProp;
    SerializedProperty backpackStoreButtonText;
    #endregion

    #region Action Window 
    SerializedProperty actionWindowProp;
    SerializedProperty runButtonProp;
    SerializedProperty bossButtonProp;
    #endregion

    #region player
    SerializedProperty playerNameProp;
    SerializedProperty playerLevelProp;
    SerializedProperty playerImageLocProp;
    #endregion

    #region enemy
    SerializedProperty enemyNameProp;
    SerializedProperty enemyLevelProp;
    SerializedProperty enemyImageLocProp;
    #endregion
    
    SerializedProperty yesNoWindowProp;
    SerializedProperty backgroundQuadProp; 
    #endregion

    private bool[] showAbilitySlots = new bool[Ability.KMaxAbilities];
    private bool[] boolArray = new bool[6];

    private void OnEnable() {
        selectionWindowProp = serializedObject.FindProperty("selectionWindow");
        abilityButtonsTextProp = serializedObject.FindProperty("abilityButtonsText");
        abilityPPTextProp = serializedObject.FindProperty("abilityPPText");
        backButtonTextProp = serializedObject.FindProperty("backButtonText"); 

        backpackStoreWindowProp = serializedObject.FindProperty("backpackStoreWindow");
        backpackStoreButtonText = serializedObject.FindProperty("backpackStoreButtonText");

        actionWindowProp = serializedObject.FindProperty("actionWindow");
        runButtonProp = serializedObject.FindProperty("runButton");
        bossButtonProp = serializedObject.FindProperty("bossButton");

        playerNameProp = serializedObject.FindProperty("playerName");
        playerLevelProp = serializedObject.FindProperty("playerLevel");
        playerImageLocProp = serializedObject.FindProperty("playerSpriteRenderer");
        
        enemyNameProp = serializedObject.FindProperty("enemyName");
        enemyLevelProp = serializedObject.FindProperty("enemyLevel");
        enemyImageLocProp = serializedObject.FindProperty("enemySpriteRenderer");
        
        yesNoWindowProp = serializedObject.FindProperty("yesNoWindow");
        backgroundQuadProp = serializedObject.FindProperty("backgroundQuad");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();
        
        ShowScript(typeof(BattleGUIManager)); 

        // Text wait time
        backgroundQuadProp.PropertyField("Background Quad");

        // Yes no window
        yesNoWindowProp.PropertyField("Yes No Window");

        // Ability Window 
        EditorGUILayout.BeginVertical(); 
        boolArray[0] = EditorGUILayout.Foldout(boolArray[0], "Ability Window");
        if (boolArray[0]) {
            indentLevel++;
            for (int i = 0; i < Ability.KMaxAbilities; i++) {
                InsertVertialFoldout(ref showAbilitySlots, i, "Ability " + (i + 1), 
                    new SerializedProperty[] { abilityButtonsTextProp.GetArrayElementAtIndex(i), abilityPPTextProp.GetArrayElementAtIndex(i) },
                    new string[] { "Name", "PP Amount" }
                );
            }
            indentLevel--;
        } 
        EditorGUILayout.EndVertical();

        // Backpack store window
        InsertVertialFoldout(ref boolArray, 1, "Backpack",
            new SerializedProperty[] { backpackStoreWindowProp, backpackStoreButtonText }, 
            new string[] { "Backpack Store Window", "Backpack Store Button Text" }
        );

        // Action window
        InsertVertialFoldout(ref boolArray, 2, "Action Window",
            new SerializedProperty[] { actionWindowProp, runButtonProp, bossButtonProp }, 
            new string[] { "Action Window", "Rune Button", "Boss Button" }
        );

        // Selection / Ability windows
        InsertVertialFoldout(ref boolArray, 3, "Selection Window",
            new SerializedProperty[] { selectionWindowProp, backButtonTextProp },
            new string[] { "Selection Window", "Back Button Text" }
        );

        // Player
        InsertVertialFoldout(ref boolArray, 4, "Player",
            new SerializedProperty[] { playerNameProp, playerLevelProp, playerImageLocProp },
            new string[] { "Name", "Level", "SpriteRenderer" }
        );

        // Enemy
        InsertVertialFoldout(ref boolArray, 5, "Enemy",
            new SerializedProperty[] { enemyNameProp, enemyLevelProp, enemyImageLocProp }, 
            new string[] { "Name", "Level", "SpriteRenderer" }
        );
        serializedObject.ApplyModifiedProperties();
    }
}