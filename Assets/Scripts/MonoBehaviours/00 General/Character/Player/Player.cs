using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character {
    private Experience _experience_component;

    private Dictionary<int, Skill> newAbilitiesToLearn = new Dictionary<int, Skill>();

    public Experience Experience_Component {
        get { return _experience_component; }
        set { _experience_component = value; }
    }
    
    public Sprite FrontSprite { get; set; }

    public Sprite BackSprite { get; set; }

    public Skill MoveSelected(int index) {
        selectedAbility = (index == -1) ? Ability.Instance.healSkill : abilitySet[index];
        return selectedAbility;
    }

    public void GainExp(int enemylevel) {
        _experience_component.GainExp(enemylevel);
    }

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

        Enemy enemy = GameManager.Instance.SelectedEnemy;
        var message = enemy.name + " is attacking " + name + " with " + enemy.SelectedAbility.name;
        StartCoroutine(TextMessager.Instance.Message(message));
        StartCoroutine(CombatDamageEffects.Instance.FadeForPlayerHit());
    }

    public bool IsHealable() {
        return (stats_component.Hp != stats_component.MaxHp);
    }

    public bool IsEtherable() {
        bool etherable = false;

        foreach (Skill s in abilitySet) {
            if (s.abilityInformation.current_pp != s.abilityInformation.max_pp) {
                etherable = true;
                break;
            }
        }

        return etherable;
    }

    public void EtherUsed() {
        foreach (Skill s in abilitySet) {
            s.abilityInformation.current_pp = s.abilityInformation.max_pp;
        }
    }
    
    public override void Dead() {
        // Display that the player has died
        StartCoroutine(TextMessager.Instance.Message(name + " has died."));
        StartCoroutine(CombatDamageEffects.Instance.FadeForPlayerDeath());
    }

    public void DecreasePPOnAbilitySelected() {
        if (selectedAbility.Equals(Ability.Instance.healSkill)) {
            StoreData.Instance.PotionUsed();
        } else {
            selectedAbility.abilityInformation.current_pp--;
        }
    }
    
    public void CheckForNewAbility(int level) {
        Skill s = Ability.Instance.CheckForAbilityAfterLevelUp(CombatType, level);
        if (s != null) {
            newAbilitiesToLearn.Add(s.abilityInformation.level_required, s);
        }
    }

    public Skill GetNewAbilityForLevel (int level) {
        try {
            return newAbilitiesToLearn[level];
        } catch (System.Exception) {
            return null;
        }
    }

    public void RemoveAndReplaceAbilityAtIndex (int index, Skill newSkill) {
        Skill old = abilitySet[index];
        StartCoroutine(TextMessager.Instance.Message(old.name + " is being replaced with " + newSkill.name));
        abilitySet[index] = newSkill;
        battleSceneController.PopulateAbilityWindow();
    }

    public string AbilitySetToJson() {
        string jsonString = string.Empty;

        foreach (Skill s in abilitySet) {
            jsonString += s.abilityInformation.ToJsonString();
        }

        return jsonString;
    }

}