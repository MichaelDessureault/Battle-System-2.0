using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

public class CreateGameObjectForPlayerOrEnemy : MonoBehaviour {
    public static GameObject NewCharacter(string name, StylesEnum.CombatTypes type, Stats stats = null, List<Skill> skillset = null) {
        GameObject obj = new GameObject(name);

        if (stats == null) {
            stats = new Stats();
            stats.NewStatsSetup(type);
        }

        // Add the script components
        var player = obj.AddComponent<Player>();
        player.name = name;
        player.Stats_Component = stats;
        player.CombatType = type;

        if (skillset == null || skillset.Count == 0)
            player.AbilitySetSetup();
        else
            player.AbilitySet = skillset;

        Sprites spritesInstance = Sprites.Instance;
        player.FrontSprite = spritesInstance.GetCharacterFrontSpriteForCombatType(type);
        player.BackSprite = spritesInstance.GetCharacterBackSpriteForCombatType(type);
        player.Experience_Component = new Experience(stats);

        return obj;
    }

    public static GameObject CreateEnemy(string name, StylesEnum.CombatTypes type, Sprite sprite, bool isBoss) {
        GameObject obj = new GameObject(name);

        // Add the extra enemy related components
        var enemy = obj.AddComponent<Enemy>();
        enemy.name = name;
        enemy.Stats_Component = new Stats(type);
        enemy.AbilitySetSetup();
        enemy.FrontSprite = sprite;
        enemy.CombatType = type;
        enemy.IsBoss = isBoss;
        
        return obj;
    }
}
