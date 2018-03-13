using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour {
    public void GenerateEnemyAndLoadBattleScene() {
        // Create enemy
        GameManager.Instance.GenerateEnemy();
        // Load scene
        ScenesController.Instance.LoadScene(ScenesEnum.battlescene);
    }
}
