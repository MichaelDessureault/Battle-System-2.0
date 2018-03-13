using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Do note that GameManager script has an Editor created for it, allow deletion of locally saved files
/// </summary>
public class GameManager : MonoBehaviourSingleton<GameManager> {
    
    #region Appdelegate
    private void OnApplicationQuit() {
        SaveData.Save();
    }

    private void OnApplicationPause(bool pause) {
        if (pause) {
            SaveData.Save();
        }
    }
    #endregion

    public const int numberOfCharacters = 3;

    public static readonly string[] characterTypeStrings = new string[numberOfCharacters] { "Assassin", "Guardian", "Wizard" };
    
    private Player character1_Assassin;
    private Player character2_Guardian;
    private Player character3_Wizard;

    //private GameObject[] charactersArrayOfGameObject; //
    private Player[] charactersArrayOfPlayer;
    
    private Player selectedPlayer;

    public Player SelectedPlayer {
        get {
            if (selectedPlayer == null)
                Debug.LogError("Trying to get selected character when a character has not been selected");
            return selectedPlayer;
        }
    }

    public Enemy SelectedEnemy { get; set; }

    public Enemy SelectedCode { get; set; }

    public Player Assassin { get { return character1_Assassin; } }

    public Player Guardian { get { return character2_Guardian; } }

    public Player Wizard { get { return character3_Wizard; } }

    public override void Awake() {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        // Load the saved data
        SaveData.LoadStoreData();
    }

    public void Start() {
        List<Stats> charactersStatsArray = SaveData.LoadCharacterStats();
        List<List<Skill>> charactersAbilitySetArray = SaveData.LoadCharacterAbilitySets();

        // Generate the characters
        if (character1_Assassin == null || character2_Guardian == null || character3_Wizard == null) {
            GenerateCharacters(charactersStatsArray, charactersAbilitySetArray);
        }
    }
    
    private void GenerateCharacters(List<Stats> characterStatsArray, List<List<Skill>> charactersAbilitySetArray) {
        // Find the character holder to store the characters in
        GameObject characterHolder = GameObject.Find("Character Holder");
        if (characterHolder == null)
            characterHolder = new GameObject("Character Holder");

        string[] characterNames = new string[numberOfCharacters] { "Shroud", "Vitalus", "Merlin" };
        List<StylesEnum.CombatTypes> cb = new List<StylesEnum.CombatTypes>() { StylesEnum.CombatTypes.assassin, StylesEnum.CombatTypes.guardian, StylesEnum.CombatTypes.wizard };

        List<GameObject> objs = new List<GameObject>();

        for (int i = 0; i < numberOfCharacters; i++)
        {
            try
            {
                GameObject temp = CreateGameObjectForPlayerOrEnemy.NewCharacter(characterNames[i], cb[i], characterStatsArray[i], charactersAbilitySetArray[i]);
                temp.GetComponent<Transform>().parent = characterHolder.transform;
                objs.Add(temp);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e.Message + " index: " + i);
            }
        }

        // Characters 
        // names: shroud, vitalus, merlin
        character1_Assassin = objs[0].GetComponent<Player>();
        character2_Guardian = objs[1].GetComponent<Player>();
        character3_Wizard   = objs[2].GetComponent<Player>();

        charactersArrayOfPlayer = new Player[] { character1_Assassin, character2_Guardian, character3_Wizard };

        DontDestroyOnLoad(characterHolder);
    }
    
    public void GenerateEnemy () {
        // Create the enemy
        StylesEnum.CombatTypes type = (StylesEnum.CombatTypes) UnityEngine.Random.Range(0, 3);
        bool isBoss = (UnityEngine.Random.Range(0, 2) == 1);
        GameObject enemy = CreateGameObjectForPlayerOrEnemy.CreateEnemy(EnemyNames.GetRandomName(), type, Sprites.Instance.GetRandomEnemySprite(), isBoss);

        // Update the selectedEnemy property
        SelectedEnemy = enemy.GetComponent<Enemy>();

        // Prevent it from being destroyed when changing scenes
        DontDestroyOnLoad(enemy);
    }

    public void SetSelectedCharacterByIndex(int index) {
        if (index >= 0 && index < numberOfCharacters)
            selectedPlayer = charactersArrayOfPlayer[index];
    }
    
    public int GetHighestLevelCharacter() {
        if (character1_Assassin != null && character2_Guardian != null && character3_Wizard != null)
        {
            var c1level = character1_Assassin.GetComponent<Player>().Stats_Component.Level;
            var c2level = character2_Guardian.GetComponent<Player>().Stats_Component.Level;
            var c3level = character3_Wizard.GetComponent<Player>().Stats_Component.Level;

            return Mathf.Max(new int[3] { c1level, c2level, c3level });
        }

        return 1;
    }
    
    public Player[] GetCharactersArrayAsPlayer() {
        return charactersArrayOfPlayer;
    }
    
    public bool IsaCharacterAlive () {
        foreach (Player p in charactersArrayOfPlayer) {
            if (!p.IsDead && p.Stats_Component.Hp >= 0) {
                return true;
            }
        }

        return false;
    }
    
    public void ResetGameData () {
        SaveData.Deletefiles();
    }
}
