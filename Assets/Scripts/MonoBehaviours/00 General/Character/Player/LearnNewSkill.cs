using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LearnNewSkill : MonoBehaviour {

    public BattleGUIManager battleGUIManager;

    private Player player;
    
    private int learningAttempt = 0;

    private bool finished = false;

    /// <summary>
    /// If a new ability is found at the level passed it will prompt with the learning of the new skill
    /// </summary>
    /// <param name="level">level being checked for new skill</param>
    public IEnumerator PromptChatWithNewAbility(int level) {
        player = GameManager.Instance.SelectedPlayer; 
        Skill s = player.GetNewAbilityForLevel(level);

        if (s == null)
            yield break;
        
        StartCoroutine(WantToLearn(s));

        yield return new WaitUntil(() => finished == true);

        battleGUIManager.EnableYesNoWindow(false);
    }

    /// <summary>
    /// A new skill was found to be tought, prompts the user if they want to learn it or not with yes or no window for input
    /// </summary>
    private IEnumerator WantToLearn(Skill s) {
        if (learningAttempt >= 2) {
            GiveUpOnAbility(s);
            yield break;
        }

        learningAttempt++;
        StartCoroutine(TextMessager.Instance.Message(message: "Do you want to learn " + s.name + "?"));
        yield return StartCoroutine(battleGUIManager.WaitForYesOrNoInput());
        
        int yesno = battleGUIManager.yesNoValue;
        battleGUIManager.yesNoValue = -1;
        // -1 = no answer, 0 = no, 1 = yes
        switch (yesno) {
            case 0:  StartCoroutine(AreYouSureYouWantToForget(s)); break;
            case 1:  StartCoroutine(WhichAibilityToForget(s)); break;
            default: GiveUpOnAbility(s); break;
        }
    }

    /// <summary>
    /// User entered no when being asked to learn the skill, prompt them the second time for a confirmation give up on the skill
    /// </summary>
    private IEnumerator AreYouSureYouWantToForget(Skill s) {
        StartCoroutine(TextMessager.Instance.Message (message: "Are you sure you do not want to learn " + s.name + "?"));

        yield return StartCoroutine(battleGUIManager.WaitForYesOrNoInput());
        int yesno = battleGUIManager.yesNoValue;
        battleGUIManager.yesNoValue = -1;

        // -1 = no answer, 0 = no, 1 = yes
        switch (yesno) {
            case 0:
                // restart
                StartCoroutine(WantToLearn(s));
                break;
            default:
                GiveUpOnAbility(s);
                break;
        }
    }

    /// <summary>
    /// User selected yes to wanting to learn the skill, prompt with asking which skill to remove
    /// Ability selection window will display for the user to select which index to remove.
    /// </summary>
    private IEnumerator WhichAibilityToForget(Skill skill) {
        battleGUIManager.removeAbility = true;
        StartCoroutine(TextMessager.Instance.Message(message: "Select the ability you would like to forget"));
        yield return StartCoroutine(battleGUIManager.WaitForAbilitySelected());

        int indexForAbilityToRemove = battleGUIManager.removeAbilityIndex;
        // reset the removeAbilityIndex
        battleGUIManager.removeAbilityIndex = -1;
        battleGUIManager.removeAbility = false;

        // If the index is the KMabAbility value then the give up (back button) was selected
        if (indexForAbilityToRemove != -1 && indexForAbilityToRemove != Ability.KMaxAbilities) {
            player.RemoveAndReplaceAbilityAtIndex(indexForAbilityToRemove, skill);
            yield return new WaitForSeconds(.5f);
        }
        finished = true;
    }

    /// <summary>
    /// User gave up on learning the new skill
    /// </summary>
    private void GiveUpOnAbility(Skill s) {
        StartCoroutine(TextMessager.Instance.Message("You have given up on learning " + s.name));
        finished = true;
    }

}