using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public enum ScenesEnum {
    mainmenu,
    battlescene,
    backpack,
    store,
    previousScene
}

public class ScenesController : MonoBehaviourSingleton<ScenesController> {
    private string previousSceneName = "MainMenu"; // MainMenu is default
    private readonly string[] sceneNames = new string[] { "MainMenu", "BattleScene", "Backpack", "Store"};
    
    public void OnEnable() {
        LoadScene(ScenesEnum.mainmenu);
    }
    
    // Called in files
    public void LoadScene(ScenesEnum scenesEnum) {
        string name = (scenesEnum == ScenesEnum.previousScene) ? previousSceneName : sceneNames[(int)scenesEnum];
        StartCoroutine(LoadingSceneModeSingle(name));
    }

    private IEnumerator LoadingSceneModeSingle(string sceneName) {
        previousSceneName = SceneManager.GetActiveScene().name;
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
    }
}
