using System.Collections;
using System.Collections.Generic;
using System.IO;
using FrostweepGames.Plugins.GoogleCloud.TextToSpeech.Example;
using UnityEngine;

public class AimaBotHandler : MonoBehaviour
{
    public string initGreeting = "Halo ada yang bisa saya bantu?";
    public bool initUseTypewriter = true;

    [Header("References")]
    public UIHandler uiHandler;
    public TypeWriterFX typeWriter;
    public Custom_GC_TextToSpeech_SimpleExample gctts;

    private void Start() {
        if(initUseTypewriter){
            typeWriter.UseTypewriterFX(initGreeting);
            gctts.SynthesizeButtonOnClickHandler(initGreeting);
        }else{
            uiHandler.outputText_AI.text = initGreeting;
            gctts.SynthesizeButtonOnClickHandler(initGreeting);
        }
    }

    public void SetOutputText(string text){
        if (typeWriter.useTypeWriter){
            typeWriter.UseTypewriterFX(text);
        }else{
            uiHandler.outputText_AI.text = text;
        }

        uiHandler.inputFld.interactable = true;
        uiHandler.sendBtn.interactable = true;
        // Debug.Log("Inpt Fld set Interactable");
    }
    public bool IsWaiting(bool waiting)
    {
        SetOutputWaiting();
        return waiting;
    }
    public void SetOutputWaiting()
    {
        int i = 0;
        for (i = 0; i < 4; i++)
        {
            typeWriter.UseTypewriterFX("Berfikir " + new string('.', i));
        }
    }

}
