using System;
using System.Collections;
using UnityEngine;

public class CombatDamageEffects : MonoBehaviourSingleton<CombatDamageEffects> {
    
    public ParticleSystem enemyParticleSystem;
    public ParticleSystem playerParticleSystem;

    public SpriteRenderer playerSpriteRenderer;
    public SpriteRenderer enemySpriteRenderer;


    /// <summary>
    /// Fades the alpha of the spriteRenders color component to the finalAlpha given over the fadeDuration provided
    /// </summary>
    /// <param name="sprite">SpriteRenderer to be faded</param>
    /// <param name="finalAlpha">What alpha value the fading stops (between 0 to 1)</param>
    /// <param name="fadeDuration">Time it takess to fade</param>
    /// <returns></returns>
    private static IEnumerator FadeEffect(SpriteRenderer sprite, float finalAlpha, float fadeDuration) {
        Color color = sprite.color;

        float fadeSpeed = Mathf.Abs(color.a - finalAlpha) / fadeDuration;

        while (!Mathf.Approximately(color.a, finalAlpha)) {
            color.a = Mathf.MoveTowards(color.a, finalAlpha, fadeSpeed * Time.deltaTime);
            sprite.color = color;
            yield return null;
        }
    }

    #region Fade For Hit
    /// <summary>
    /// Starts a coroutine of a fade effect for the player
    /// </summary>
    public IEnumerator FadeForPlayerHit () {
        yield return StartCoroutine(FadeForHit(playerSpriteRenderer, playerParticleSystem));
    }

    /// <summary>
    /// Starts a coroutine of a fade effect for the enemy
    /// </summary>
    public IEnumerator FadeForEnemyHit () {
        yield return StartCoroutine(FadeForHit(enemySpriteRenderer, enemyParticleSystem));
    }

    /// <summary>
    /// Does a flicker fade in an out effect for taking damage
    /// </summary>
    /// <param name="isPlayer">decides which spriterenderer to use for the effect</param>
    private IEnumerator FadeForHit(SpriteRenderer spriteRenderer, ParticleSystem particleSystem) {
        particleSystem.Play();
        yield return StartCoroutine(FadeEffect(spriteRenderer, 0.5f, 0.25f));
        yield return StartCoroutine(FadeEffect(spriteRenderer, 1f, 0.25f));
        yield return StartCoroutine(FadeEffect(spriteRenderer, 0.5f, 0.25f));
        yield return StartCoroutine(FadeEffect(spriteRenderer, 1f, 0.25f));
        particleSystem.Stop();
    }
    #endregion

    #region Fade For Death
    /// <summary>
    /// Starts are coroutine to fade the player sprite to 0 alpha
    /// If the player is vsing a boss then it prompts to continue fighting if there is a character alive still
    /// Else loads the mainmenu sence
    /// </summary>
    public IEnumerator FadeForPlayerDeath () {
        yield return StartCoroutine(FadeEffect(playerSpriteRenderer, 0f, 0.5f));

        yield return new WaitForSeconds(.3f);

        if (GameManager.Instance.SelectedEnemy.IsBoss && GameManager.Instance.IsaCharacterAlive()) {
            StartCoroutine(BattleSceneController.Instance.battleGUIManager.ContinueFighting());
        } else {
            ScenesController.Instance.LoadScene(ScenesEnum.mainmenu);
        }
    }

    /// <summary>
    /// Starts a coroutine to fade the enemy sprite to 0 alpha, loads the mainmen scene once done
    /// </summary> 
    public IEnumerator FadeForEnemyDeath () {
        yield return StartCoroutine(FadeEffect(playerSpriteRenderer, 0f, 0.5f));
        yield return new WaitForSeconds(.3f);
        ScenesController.Instance.LoadScene(ScenesEnum.mainmenu);
    }
    #endregion
}
