using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enemy is used for the enemies combat script it will make actions depending on hp level
public class Enemy : Character {
    private int numberOfTimesHealed = 0;

    public bool IsBoss { get; set; }

    public Sprite FrontSprite { get; set; }

    //public QRCode code;

    #region Enemies Decision Selection

    /// <summary>
    /// This is basic Enemy decision making
    /// 1. It will check to see if the hap is less then or equal to 10% of the maxhp, is so it will then attempt to heal
    ///     Attempting to heal has a 16.66% chance to fail, enemy can only heal up to 3 times in a fight
    /// 2. It will check if the hp is between 10% and 30%, if so it calls the LowHp
    ///     LowHp has a 20% chance to heal and to 80% to attack
    ///     Again, there is a 16.66% chance to fail, and can only heal 3 times
    /// 3. Does a random attack out of it's skill set
    /// 
    /// The decision making can get more complex/smarter by checking the player's character speed 
    ///   comparing it to there's and if the enemy finds out it's faster it can then
    ///   have a more likly chance to get attack then heal when critical hp 
    /// </summary>
    /// <returns></returns>

    public Skill EnemiesTurn() {
        // check health condition...
        var hp = stats_component.Hp;
        var maxhp = stats_component.MaxHp;

        if (hp <= maxhp / 10 && numberOfTimesHealed < 3)
            CriticalHp();
        else if (hp > maxhp / 10 && hp <= maxhp / 30 && numberOfTimesHealed < 3)
            LowHp();
        else
            RandomAttack();

        return selectedAbility;
    }

    private void CriticalHp() {
        // 16.66% chance it still wont heal 
        if (Random.Range(1, 7) != 1) {
            selectedAbility = Ability.Instance.healSkill;
            numberOfTimesHealed++;
        } else {
            RandomAttack();
        }
    }

    // ten ix no included
    private void LowHp() {
        // 20% chance to be CriticalHp check and 80% chance to attack 
        if (Random.Range(1, 6) == 1)
            CriticalHp();
        else
            RandomAttack();
    }

    private void RandomAttack() {
        int x = 0;
        foreach (Skill skill in abilitySet) {
            if (!skill.name.Equals("None"))
                x++;
        }
        selectedAbility = abilitySet[Random.Range(0, x)];
    }
    #endregion
    
    public override void TakeDamage(int damage) {
        int newHp = stats_component.Hp - damage;

        // Check if the new hp is below zero or zero if so the enemy has died
        // Reset the newHp value to 0 to make sure it doesn't go below
        // Update the IsDead variable
        if (newHp <= 0) {
            IsDead = true;
            newHp = 0;
        }

        stats_component.Hp = newHp;

        Player player = GameManager.Instance.SelectedPlayer;
        StartCoroutine(TextMessager.Instance.Message(player.name + " is attacking " + name + " with " + player.SelectedAbility.name));
        StartCoroutine(CombatDamageEffects.Instance.FadeForEnemyHit());
    }
    

    public override void Dead() {
        // Display that the player has died
        StartCoroutine(TextMessager.Instance.Message(name + " has died."));

        // Player now gains xp for killing the enemy
        Player player = GameManager.Instance.SelectedPlayer;
        player.GainExp(stats_component.Level);

        // Player gains coins for winning the battle
        StoreData storeData = StoreData.Instance;
        if (IsBoss) {
            storeData.CoinAmount += 10;
        } else {
            storeData.CoinAmount += Random.Range(2, 5); // 2 - 4 coins
        }

        Debug.Log("Enemy Dead :: The death has come to an end");

        //StartCoroutine(CombatDamageEffects.Instance.FadeForEnemyDeath());
    }
}
