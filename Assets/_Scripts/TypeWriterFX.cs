using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TypeWriterFX : MonoBehaviour
{
    public bool useTypeWriter = true;
    public float startDelay = 0;
    public float typeDelay = 0.1f;
    [Tooltip("Typing effect before the chars")]
    public string leadingChar = "";
    public bool leadingCharBeforeDelay = false;


    private Coroutine currCoroutine;

    [Header("References")]
    public UIHandler uiHandler;

    public void UseTypewriterFX(string text){
        if(currCoroutine != null){
            StopCoroutine(currCoroutine);
            Debug.Log("Stopping current Typewriter Coroutine");
        }

        currCoroutine = StartCoroutine(TypeWriterCoroutine(text));
    }

    IEnumerator TypeWriterCoroutine(string text){
        uiHandler.outputText_AI.text = leadingCharBeforeDelay ? leadingChar : "";

        yield return new WaitForSeconds(startDelay);

        foreach(char c in text){
            if(uiHandler.outputText_AI.text.Length > 0){
                uiHandler.outputText_AI.text = uiHandler.outputText_AI.text.Substring(0, uiHandler.outputText_AI.text.Length - leadingChar.Length);
            }
            uiHandler.outputText_AI.text += c;
            uiHandler.outputText_AI.text += leadingChar;

            yield return new WaitForSeconds(typeDelay);
        }

        if(leadingChar != ""){
            uiHandler.outputText_AI.text = uiHandler.outputText_AI.text.Substring(0, uiHandler.outputText_AI.text.Length - leadingChar.Length);
        }

    }

}
