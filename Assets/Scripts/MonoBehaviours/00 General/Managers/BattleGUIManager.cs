using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Do note that BattleGUIManager script has an Editor created for it, any variables added with this script have to be added in the Editor script as well
/// </summary>
public class BattleGUIManager : MonoBehaviour {
    
    // Privates
    private BattleSceneController battleSceneController;
    private ScenesController scenesController;

    #region private serialized variables (See able in inspector but no other classes)
    [SerializeField] private GameObject backgroundQuad;
    
    [SerializeField] private GameObject actionWindow;
    [SerializeField] private GameObject selectionWindow;
    [SerializeField] private GameObject yesNoWindow;

    // Backpack store window
    [SerializeField] private GameObject backpackStoreWindow;
    [SerializeField] private Text backpackStoreButtonText;

    [SerializeField] private GameObject runButton;
    [SerializeField] private GameObject bossButton;

    [SerializeField] private Text[] abilityButtonsText = new Text[Ability.KMaxAbilities];
    [SerializeField] private Text[] abilityPPText = new Text[Ability.KMaxAbilities];
    
    [SerializeField] private Text backButtonText;
    
    [SerializeField] private Text playerName;
    [SerializeField] private Text playerLevel;
    [SerializeField] private SpriteRenderer playerSpriteRenderer;

    [SerializeField] private Text enemyName;
    [SerializeField] private Text enemyLevel;
    [SerializeField] private SpriteRenderer enemySpriteRenderer;

    #endregion

    // Properties
    public bool SwapActive { get; set; }
    public Text[] AbilityButtonsText { get { return abilityButtonsText; } }
    public Text[] AbilityPPText { get { return abilityPPText; } }
     
    public bool removeAbility = false;
    public int removeAbilityIndex = -1;
    public int yesNoValue = -1;
    
    private void Start() {
        battleSceneController = BattleSceneController.Instance;
        scenesController = ScenesController.Instance;
        RandomBackground();

        // enemy Sprite
        enemySpriteRenderer.sprite = GameManager.Instance.SelectedEnemy.FrontSprite;
    }

    private void OnEnable() {
        playerSpriteRenderer.enabled = false;
        CharacterSelectionManager.CharacterSelectionMade += CharacterHasBeenSelected;
    }
    private void OnDisable() {
        CharacterSelectionManager.CharacterSelectionMade -= CharacterHasBeenSelected;
    }

    #region Setup

    public void EnemyGuiSetup (Enemy enemy) {
        // Name and Level
        enemyName.text = enemy.name.ToString();
        enemyLevel.text = enemy.Stats_Component.Level.ToString();
    }

    public void PlayerGuiSetup (Player player) {
        // Sprite
        playerSpriteRenderer.enabled = true;
        playerSpriteRenderer.sprite = player.BackSprite;

        // Name and Level
        playerName.text = player.name;
        playerLevel.text = player.Stats_Component.Level.ToString();
    }

    void RandomBackground() {
        var sInstance = Sprites.Instance;
        Material mat = sInstance.battleSceneBackgroundMaterials[Random.Range(0, sInstance.BackgroundLength)];
        backgroundQuad.GetComponent<MeshRenderer>().material = mat;
    }
    public void HandleBoss() {
        // remove the runButton
        runButton.SetActive(false);

        // enable the boss swap character button
        bossButton.SetActive(true);
    }

    public void AllDead(bool toStore) {
        if (toStore) {
            SetBackPackButtonText("Store");
        } else {
            SetBackPackButtonText("Backpack");
        }
        actionWindow.SetActive(false);
        backpackStoreWindow.SetActive(enabled);
    }
    #endregion

    #region WaitForInputs
    public IEnumerator WaitForYesOrNoInput() {
        EnableYesNoWindow(true);
        EnableYesNoButtons(true);

        float timePassed = 0;
        yield return new WaitUntil(() => ValidInput(ref timePassed));

        EnableYesNoButtons(false);
    }

    public IEnumerator WaitForAbilitySelected() {
        selectionWindow.SetActive(true);
        backButtonText.text = "Give Up";

        float timePassed = 0;
        yield return new WaitUntil(() => ValidInput(ref timePassed));

        selectionWindow.SetActive(false);
        backButtonText.text = "Back";
    }

    /// <summary>
    /// TimePassedTimer is used to prevent an infinite wait for event to happen
    /// </summary>
    /// <returns>true if time is over maxTimeAllowed else falsed</returns>
    private bool TimePassedTimer (ref float timePassed, float maxTimeAllowed) {
        timePassed += Time.deltaTime;
        return (timePassed >= maxTimeAllowed);
    }

    private bool ValidInput (ref float timePassed) {
        return (yesNoValue == 0 || yesNoValue == 1 || TimePassedTimer(ref timePassed, 30f));
    }

    #endregion

    #region yes no window
    // enabling yes no window, does the inverse to action window
    public void EnableYesNoWindow(bool enabled) {
        actionWindow.SetActive(!enabled);
        yesNoWindow.SetActive(enabled);
    }

    public void EnableYesNoButtons(bool enabled) {
        Button[] btns = yesNoWindow.GetComponentsInChildren<Button>();
        foreach (Button btn in btns) {
            btn.interactable = enabled;
        }
    }

    // yes is 1, no is 0
    public void YesNoSelected(IndexData indexData) {
        yesNoValue = indexData.IndexValue;
    }
    #endregion

    #region Character selection
    public void CharacterHasBeenSelected () {
        SwapActive = false;
        ResetPlayerSpriteRendererAlpha();
        UpdateActionWindowButtons(true);
    }

    // Reset the playerSpriteRender to full alpha in case the swap character was invoked from dead character (playerSpriteRender fades to 0)
    private void ResetPlayerSpriteRendererAlpha () {
        Color c = playerSpriteRenderer.color;
        c.a = 1;
        playerSpriteRenderer.color = c;
    }

    public void SwapCharacter () {
        CharacterSelectionManager csm = FindObjectOfType<CharacterSelectionManager>();

        if (SwapActive) {
            csm.CloseSelectionWindow();
        } else {
            // Restarting
            csm.EnableSelectionWindow();
            UpdateActionWindowButtons(false);
        }

        SwapActive = !SwapActive;
    }
    
    public void SelectionWindowBackButton () {
        if (removeAbility) {
            removeAbilityIndex = Ability.KMaxAbilities;
        }

        selectionWindow.SetActive(false);
    }
    #endregion

    // Update the action window buttosn to be interactable 
    // Attack, Heal, Run, Swap Character.  (Run and Swap Character will never be seen at the same time)
    public void UpdateActionWindowButtons(bool enabled) {
        foreach (Button b in actionWindow.GetComponentsInChildren<Button>()) {
            b.interactable = enabled;
        }
    }

    // enable backpack / store window 
    public void SetBackPackButtonText(string text) {
        backpackStoreButtonText.text = text;
    }

    public void BackPackButtonAction() {
        if (backpackStoreButtonText.text.Equals("Store")) {
            scenesController.LoadScene(ScenesEnum.store);
        } else {
            scenesController.LoadScene(ScenesEnum.backpack);
        }
    }
    // Ability selected also includes heal as an ability, passed with indexData index -1
    public void AbilitySelected(IndexData indexData) {
        var index = indexData.IndexValue;

        // if heal was selected, but the player is full health do nothing
        if (index == -1 && GameManager.Instance.SelectedPlayer.Stats_Component.IsFullHealth)
            return;

        // set all the action buttons to "false" not click able while the combat effects are happing
        UpdateActionWindowButtons(false);

        if (removeAbility) {
            removeAbilityIndex = index;
        } else {
            StartCoroutine(battleSceneController.TurnHandler(index));
        }
    }
    
    public IEnumerator ContinueFighting () {
        StartCoroutine(TextMessager.Instance.Message("Wold you like to continue fighting?"));
        yield return StartCoroutine(WaitForYesOrNoInput());

        // 1 is yes
        if (yesNoValue == 1) {
            SwapCharacter();
            EnableYesNoWindow(false);
        } else {
            ScenesController.Instance.LoadScene(ScenesEnum.mainmenu);
        }
    }
}
