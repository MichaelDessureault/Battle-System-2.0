using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarController : MonoBehaviour {
    public HealthBar playerHealthBar;
    public HealthBar enemyHealthBar;

    public void PlayerSetup() {
        playerHealthBar.HealthBarSetup(GameManager.Instance.SelectedPlayer);
    }

    public void EnemySetup() {
        enemyHealthBar.HealthBarSetup(GameManager.Instance.SelectedEnemy);
    }
}