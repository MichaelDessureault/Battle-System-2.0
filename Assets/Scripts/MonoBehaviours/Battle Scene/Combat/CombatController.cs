using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour {
    private WaitForSeconds wait = new WaitForSeconds(.3f);

    private Player player;
    private Enemy enemy;

    private Skill playerSkill;
    private Skill enemySkill;

    private HealthBar playerHealthBar;
    private HealthBar enemyHealthBar;

    public IEnumerator BeginCombat (Player _player, Skill _playerAbility, Enemy _enemy, Skill _enemyAbility) {
        BattleSceneController battleSceneController = BattleSceneController.Instance;
        HealthBarController hbc = battleSceneController.healthBarController;
        battleSceneController.combatActive = true;

        player = _player;
        playerSkill = _playerAbility;
        playerHealthBar = hbc.playerHealthBar;

        enemy = _enemy;
        enemySkill = _enemyAbility;
        enemyHealthBar = hbc.enemyHealthBar;
        
        yield return StartCoroutine (Attacking());

        battleSceneController.combatActive = false;
    }
    
    /// <summary>
    /// Checks to see if the abilities used for character and enemy are heal if so it heals the character
    /// Checks to see who should attack first 
    /// During each heal or attack it 
    ///     Waits a frame to allow the update method within the HealthBar script to begin
    ///     Starts a WaitUntil for the HealthBar passed to be done updating
    ///     Starts a WaitForSeconds for a small delay before continuing
    /// </summary>
    public IEnumerator Attacking() {
        bool isPlayerHealing = (playerSkill.Equals(Ability.Instance.healSkill));
        bool isEnemyHealing  = (enemySkill .Equals(Ability.Instance.healSkill));

        bool playerdone = isPlayerHealing;
        bool enemydone  = isEnemyHealing;
        
        // If player is healing, heal the player and wait until the health bar is done updating continuing
        if (isPlayerHealing) {
            player.Heal();
            yield return StartCoroutine(PlayerHealthBarWait());
        }

        // If enemy is healing, heal the player and wait until the health bar is done updating continuing
        if (isEnemyHealing) {
            enemy.Heal();
            yield return StartCoroutine(EnemyHealthBarWait());
        }
        
        // If both used heal then these will both be true so just leave
        if (playerdone && enemydone)
            yield break;

        // Now attacking comes into play, check who is faster to decided who attacks first
        // After finding who goes first it validates that the turn is not done for that character
        // If the turn is not done for that character they attack and wait until the healthbar is done before continuing
        if (IsPlayerFaster()) {
            if (!playerdone) {
                PlayerAttacksEnemy();
                yield return StartCoroutine(EnemyHealthBarWait());

                // check if the enemy has died, if so end the Attacking Coroutine
                if (enemy.IsDead)
                    yield break;
            }

            if (!enemydone) {
                EnemyAttacksPlayer();
                yield return StartCoroutine(PlayerHealthBarWait());
            }
        } else {
            if (!enemydone) {
                EnemyAttacksPlayer();
                yield return StartCoroutine(PlayerHealthBarWait());

                // check if the enemy has died, if so end the Attacking Coroutine
                if (player.IsDead)
                    yield break;
            }

            if (!playerdone) {
                PlayerAttacksEnemy();
                yield return StartCoroutine(EnemyHealthBarWait());
            }
        }
    }

    private IEnumerator PlayerHealthBarWait() {
        yield return null;
        yield return new WaitUntil(() => HealthBarDoneUpdating(playerHealthBar));
        yield return wait;
    }

    private IEnumerator EnemyHealthBarWait() {
        yield return null;
        yield return new WaitUntil(() => HealthBarDoneUpdating(enemyHealthBar));
        yield return wait;
    }


    /// <summary>
    /// Checks the isUpdating variable within the healthbar
    /// </summary>
    /// <param name="healthBar">Healthbar that gets checked if it's updating</param>
    /// <returns>Returns false if the isUpdating is true.  Returns true if the isUpdating is false.</returns>
    private bool HealthBarDoneUpdating(HealthBar healthBar) {
        return (healthBar.IsUpdating == false);
    }

    /// <summary>
    /// Checks to see who is faster
    /// </summary>
    /// <returns>Returns True if player is faster, false if the player is slower</returns>
    private bool IsPlayerFaster() {
        var cSpeed = player.Stats_Component.Speed;
        var eSpeed = enemy.Stats_Component.Speed;
        if (cSpeed == eSpeed)
            return (Random.Range(0, 2) == 0);
        return (cSpeed > eSpeed);
    }

    private void PlayerAttacksEnemy () {
        int playerDamageToDeal = CalculateDamage(player, enemy, playerSkill);

        if (playerDamageToDeal == 0) {
            StartCoroutine(TextMessager.Instance.Message(player.name + " has missed..."));
        } else {
            enemy.TakeDamage(playerDamageToDeal);
        }
    }

    private void EnemyAttacksPlayer () {
        int enemyDamageToDeal = CalculateDamage(enemy, player, enemySkill);

        if (enemyDamageToDeal == 0) {
            StartCoroutine(TextMessager.Instance.Message(enemy.name + " has missed..."));
        } else {
            player.TakeDamage(enemyDamageToDeal);
        }
    }
    
    // returns the amount of damage the attacking character deals to the taking character
    private int CalculateDamage (Character attackingCharacter, Character takingCharacter, Skill skillUsed) {
        return Mathf.RoundToInt(DamageCalculation.CalculateDamage(attackingCharacter, takingCharacter, skillUsed));
    }
} 
