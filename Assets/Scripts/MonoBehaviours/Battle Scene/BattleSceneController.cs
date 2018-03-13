using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BattleSceneController : MonoBehaviourSingleton<BattleSceneController> {

    public bool combatActive;

    // Private
    private Enemy enemy;
    private Player player;
    private Text[] abilityPPText;
    
    private int extraLevelsForBoss = 3;
    private string ppString = "PP     "; // 5 spaces
    private string choiceMessage = "Which will you choose?";

    // Properties
    public HealthBarController healthBarController { get; private set; }
    public ExperienceBarController experienceBarController { get; private set; }
    public BattleGUIManager battleGUIManager { get; private set; }
    public CombatController combatController { get; private set; }

    private GameManager gmInstance;

    // Override the Awake within the gameobject singleton script
    new private void Awake() {
        gmInstance = GameManager.Instance;
        if (gmInstance.SelectedEnemy == null) {
            gmInstance.GenerateEnemy();
        }

        Player[] players = gmInstance.GetCharactersArrayAsPlayer();
        if (players == null || players.Length == 0)
            gmInstance.Start(); 
    }

    private void Start() {
        battleGUIManager = GetComponent<BattleGUIManager>();
        experienceBarController = GetComponent<ExperienceBarController>();
        healthBarController = GetComponent<HealthBarController>();
        combatController = GetComponent<CombatController>();

        CharacterSelectionManager.CharacterSelectionMade += PlayerSetup;
        CharacterSelectionManager.CharacterSelectionMade += EnemySetup;
    }

    private void OnDisable() {

        CharacterSelectionManager.CharacterSelectionMade -= PlayerSetup;
        CharacterSelectionManager.CharacterSelectionMade -= EnemySetup;
    }

    public void PlayerSetup() {
        player = gmInstance.SelectedPlayer;

        battleGUIManager.PlayerGuiSetup(player);
        healthBarController.PlayerSetup();
        experienceBarController.ExpSetup(player);
        PopulateAbilityWindow();

        // Prompt with next move choice
        StartCoroutine(TextMessager.Instance.Message(choiceMessage));
        
    }
    
    public void EnemySetup() {

        enemy = gmInstance.SelectedEnemy;

        if (enemy.IsBoss) {
            enemy.Stats_Component.RandomStatsForLevel(player.Stats_Component.Level + extraLevelsForBoss);
            battleGUIManager.HandleBoss();
        } else {
            enemy.Stats_Component.RandomLevelWithRandomStats(player.Stats_Component.Level);
        }

        healthBarController.EnemySetup();
        battleGUIManager.EnemyGuiSetup(enemy);
        // unsubcribe from the CharacterSelectionMade because it's not required to re-setup the enemy when making another character change.
        CharacterSelectionManager.CharacterSelectionMade -= EnemySetup;
    }


    public void PopulateAbilityWindow() {
        Text[] abilityButtonsText = battleGUIManager.AbilityButtonsText;
        abilityPPText = battleGUIManager.AbilityPPText;

        for (int i = 0; i < Ability.KMaxAbilities; i++) {
            var ability = player.GetAbilityAtIndex(i);
            abilityButtonsText[i].text = ability.name;

            int currentPP = ability.abilityInformation.current_pp;
            
            abilityPPText[i].text = ppString + currentPP + "/" + ability.abilityInformation.max_pp; // 5 spaces between PP and the current pp text

            Button parentsBtn = abilityButtonsText[i].transform.parent.GetComponent<Button>();

            if (parentsBtn != null) {
                parentsBtn.interactable = (ability.Equals(Ability.Instance.noSkill) || currentPP <= 0) ? false : true;
            }
        }
    }
     
    public IEnumerator TurnHandler(int index) {
        Skill playerSelectedAbility = player.MoveSelected(index);
        Skill enemeySelectedAbility = enemy.EnemiesTurn();

        // begin attacks, combat active is updated within the CombatController Begin function
        StartCoroutine(combatController.BeginCombat(player, playerSelectedAbility, enemy, enemeySelectedAbility));

        yield return new WaitUntil(() => combatActive == false);

        player.DecreasePPOnAbilitySelected();
        
        if (playerSelectedAbility != Ability.Instance.healSkill) {
            UpdatePPForAbility(index, playerSelectedAbility);
        }

        // validate that the enemy has not died
        if (enemy.IsDead) {
            enemy.Dead();
            yield break;
        }

        // validate that the player has no died
        if (player.IsDead) {
            player.Dead();
            yield break;
        }

        // neather have died prompt with next move
        StartCoroutine(TextMessager.Instance.Message(choiceMessage));
        // enable action window buttons
        battleGUIManager.UpdateActionWindowButtons(true);
    }

    public void UpdatePPForAbility(int index, Skill s) {
        var currentPP = s.abilityInformation.current_pp;
        abilityPPText[index].text = ppString + currentPP + "/" + s.abilityInformation.max_pp;
        if (currentPP <= 0) {
            Button btn = abilityPPText[index].transform.parent.GetComponent<Button>();

            if (btn != null) {
                btn.interactable = false;
            }
        }
    }
    public void Run() {
        Destroy(enemy);
    }
}