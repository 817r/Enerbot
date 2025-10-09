using System.Collections;
using System.Collections.Generic;
using FrostweepGames.Plugins.GoogleCloud.SpeechRecognition.Examples;
using FrostweepGames.Plugins.GoogleCloud.TextToSpeech.Example;
using TMPro;
using UnityEngine;

public class SpeechRecognitionHandler : MonoBehaviour
{
    [Header("References")]
    public UIHandler uiHandler;
    public AzureAPIHandler azureAPI;
    // public TestAzureAPI testAzureAPI;
    public Custom_GC_TextToSpeech_SimpleExample gcTTS;
    public AimaBotHandler aimaHandler;
    public VideoHandler vidHandler;
    [Tooltip("Automatically send the message after Voice Input")]
    public bool autoSend = true;

    public void SetInputFldText(string voiceInput){
        uiHandler.inputFld.text = voiceInput;

        if(autoSend){
            SendMsg();
        }

    }

    // Send input from user to API
    public void SendMsg(){
        string myText = uiHandler.inputFld.text;

        StartCoroutine(azureAPI.SendChatReq(myText, PostReq)); // PostReq is the result after sending input from user, the response from AI Bot
    }

    // Set the OutputText to the response of the Bot
    void PostReq(string botMsg){
        gcTTS.SynthesizeButtonOnClickHandler(botMsg);
        aimaHandler.SetOutputText(botMsg);
        vidHandler.StartTalking();
    }
    
}
