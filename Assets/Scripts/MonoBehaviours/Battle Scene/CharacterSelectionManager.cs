using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Do note that CharacterSelectionManager script has an Editor created for it, any variables added with this script have to be added in the Editor script as well
/// </summary>
public class CharacterSelectionManager : MonoBehaviour {
    // Public
    public static Action CharacterSelectionMade;
    
    // Serialized Private Fields
    [SerializeField] private Color deathColor;
    [SerializeField] private Image enemyBlur;
    [SerializeField] private GameObject characterSelectionContainer;
    [SerializeField] private RawImage[] charactersRawImageArray = new RawImage[GameManager.numberOfCharacters];
    [SerializeField] private HealthBar[] charactersHealthBarArray = new HealthBar[GameManager.numberOfCharacters];

    // Private
    private GameManager gmInstance;
    private Player[] characters;
    
    private void Start() {
        gmInstance = GameManager.Instance;
        characters = gmInstance.GetCharactersArrayAsPlayer();
        
        DisplaySelectionMessage();
        EnableSelectionWindow();
    }
    
    private void DisplaySelectionMessage () { 
        string message;

        if (!gmInstance.IsaCharacterAlive()) { 
            var potionAmount = StoreData.Instance.PotionAmount;
            var toStore = false;

            if (potionAmount <= 0) {
                message = "All characters are dead, buy potions to revive";
                toStore = true;
            } else {
                message = "All characters are dead, use a potion to revive a character";
            }

            FindObjectOfType<BattleGUIManager>().AllDead(toStore);
        } else if (gmInstance.SelectedEnemy.IsBoss) {
            message = "You have encountered a boss! \n Please select a character";
        } else {
            message = "Please select a character";
        }

        TextMessager.Instance.DirectMessage(message);
    }

    public void EnableSelectionWindow() {
        // check all characters hp's
        // if a character is dead then they can not be selected
        characterSelectionContainer.SetActive(true);
        CharacterButtonSetup(characters);
        HealthBarSetup(characters);
    }

    // if a character is dead than the buttons will be disabled and the colour of the 
    // character will be set to the deathColor
    private void CharacterButtonSetup(Player[] characters) {
        int index = 0;
        foreach (Player p in characters) {
            if (p.Stats_Component.Hp == 0) {
                RawImage rawImage = charactersRawImageArray[index];
                rawImage.color = deathColor;

                var button = rawImage.gameObject.GetComponent<Button>();
                if (button != null)
                    button.enabled = false;
            }
            index++;
        }
    }

    private void HealthBarSetup (Player[] characters) {
        for (int i = 0; i < charactersHealthBarArray.Length; i++) {
            charactersHealthBarArray[i].HealthBarSetup(characters[i]);
        }
    }
    
    public void CharacterSelected(IndexData indexData) {

        // check for invalid inputs
        if (indexData.IndexValue < 0 || indexData.IndexValue >= GameManager.numberOfCharacters) {
            Debug.LogWarning("index passed for characterselected is out of range");
            return;
        }

        gmInstance.SetSelectedCharacterByIndex(indexData.IndexValue);
        
        // Invoke the Action if it's not null
        if (CharacterSelectionMade != null) {
            CharacterSelectionMade();
        } else {
            Debug.LogWarning("No subscription to CharacterSelectionMade");
        }

        // Disable the enemyBlur object, the characte zone is within the SelectionWindow hierarchy 
        enemyBlur.gameObject.SetActive(false);
        // Close selection widnow
        CloseSelectionWindow();
    }

    private void DisableBlurs () {
    }

    public void CloseSelectionWindow () {
        // disable character selection
        characterSelectionContainer.SetActive(false);
    }
}
