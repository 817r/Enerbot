using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    public AzureTTSRest azureTTS;
    [Tooltip("Automatically send the message after Voice Input")]
    public bool autoSend = true;
    int responseCount = 0;


    private void Start()
    {
        string folderPath = Path.Combine(Application.persistentDataPath, azureTTS.outputFolderName);

        if (Directory.Exists(folderPath))
        {
            string[] files = Directory.GetFiles(folderPath);
            responseCount = files.Length;
        }

    }

    public void SetInputFldText(string voiceInput){
        uiHandler.inputFld.text = voiceInput;

        if(autoSend){
            SendMsg();
        }

    }

    // Send input from user to API
    public void SendMsg(){
        string myText = uiHandler.inputFld.text;
        StartCoroutine(azureTTS.Speak("Berfikir", "thinking"));
        StartCoroutine(azureAPI.SendChatReqNew(myText, PostReq));
    }

    // Set the OutputText to the response of the Bot
    void PostReq(string botMsg){
        //gcTTS.SynthesizeButtonOnClickHandler(botMsg);
        //vidHandler.StartTalking();
        StartCoroutine(azureTTS.Speak(botMsg, $"Resp{responseCount}"));
        responseCount++;
    }
    
}
