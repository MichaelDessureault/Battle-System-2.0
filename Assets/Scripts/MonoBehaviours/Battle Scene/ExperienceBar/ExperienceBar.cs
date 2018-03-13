using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ExperienceBar : MonoBehaviour {

    [SerializeField] private float slideDuration;
    [SerializeField] private Image xpBar;
    [SerializeField] private Text  characterLevel;

    private Stats stats_component;
    private int level;
    private float currentXp;

    private int previousLevelXp;
    private int currentLevelXp;
    private int levelXpDifference;

    private LearnNewSkill learnNewSkill;

    private void Start() {
        learnNewSkill = FindObjectOfType<LearnNewSkill>();
        if (learnNewSkill == null) {
            GameObject obj = new GameObject("Learn New Skill");
            learnNewSkill = obj.AddComponent<LearnNewSkill>();
        }
    }

    public void Setup (Player player) {
        stats_component = player.Stats_Component;
        level = stats_component.Level;
        currentXp = stats_component.Current_Xp;
        LevelXpSetup();
    }

    private void LevelXpSetup () {
        previousLevelXp = Experience.LevelExp(level - 1);
        currentLevelXp = Experience.LevelExp(level);
        levelXpDifference = currentLevelXp - previousLevelXp;
    }

    private float speed = -1;
    public bool _isUpdating;
    public bool isUpdating {
        get { return _isUpdating; }
    }

    private void Update() {
        if (stats_component == null)
            return;

        if (!Mathf.Approximately(currentXp, (float) stats_component.Current_Xp)) { 
            _isUpdating = true;
            var newXp = stats_component.Current_Xp;

            if (speed == -1) {
                speed = Mathf.Abs(currentXp - newXp) / 100f;
            }

            currentXp = Mathf.Clamp(currentXp + speed, previousLevelXp, currentLevelXp);

            UpdateExpBar();

            if (currentXp == currentLevelXp) {
                StartCoroutine(UpdateCurrentLevel());
            }
        } else {
            speed = -1;
            _isUpdating = false;
        }
    }
    
    private void UpdateExpBar () { 
        var scale = currentXp / (float)levelXpDifference;
        xpBar.transform.localScale = new Vector3(scale, 1f, 1f);
    } 

    private IEnumerator UpdateCurrentLevel () {
        level++;
        LevelXpSetup();
        // Check for a new ability 
        yield return StartCoroutine(learnNewSkill.PromptChatWithNewAbility(level));
    }
}
