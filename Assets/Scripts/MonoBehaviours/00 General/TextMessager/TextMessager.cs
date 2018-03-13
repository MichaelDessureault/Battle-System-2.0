using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextMessager : MonoBehaviourSingleton<TextMessager> {
    [SerializeField] private GameObject textWindow;
    [SerializeField] private Text text;

    private bool isMessaging;

    /// <summary>
    /// Displays the text to the screen instantly (1 frame)
    /// </summary>
    /// <param name="message">message</param>
    public void DirectMessage (string message) {
        SetText(message);
    }
    
    /// <summary>
    /// Diplays each char of the message to the screen with a coroutine
    /// </summary>
    /// <param name="message"> message </param>
    
    public IEnumerator Message(string message) {
        if (isMessaging)
            yield break;

        isMessaging = true;
        SetText(message);
        isMessaging = false;
    }

    private void SetText(string message) {
        text.text = message;
    }

    private void AppendChar(char c) {
        text.text += c;
    }
}