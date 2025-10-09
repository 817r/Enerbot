using System.Collections;
using System.Collections.Generic;
using FrostweepGames.Plugins.GoogleCloud.SpeechRecognition.Examples;
using UnityEngine;

public class InteractionHandler : MonoBehaviour
{
    public bool IsRecording;

    [Header("References")]
    public UIHandler uiHandler;
    public SpeechRecognitionHandler speechHandler;
    public Custom_GCSR_Example gcsr;


    private void Update() {
         if(Input.GetKeyDown(KeyCode.Return) && uiHandler.inputFld.text.Length != 0){
            speechHandler.SendMsg();
            // speechHandler.inputFld.text = "";
            Debug.Log("Enter: Send Text Input Fld| " + uiHandler.inputFld.isFocused);
            uiHandler.inputFld.interactable = false;
            // Debug.Log("Inpt Fld set NOT Interactable");

        }

        // For Voice Record
        if(!uiHandler.inputFld.isFocused){
            // Delete Text in Input Fld
            if(uiHandler.inputFld.text.Length != 0 && Input.GetKeyDown(KeyCode.Space)){
                uiHandler.inputFld.text = "";
            }

            // If theres no text in Input Field
            if(uiHandler.inputFld.text.Length == 0){
                if(Input.GetKeyDown(KeyCode.Space) && !IsRecording){
                    IsRecording = true;
                    gcsr.StartRecordButtonOnClickHandler();
                    Debug.Log("Space: Start Record| " + uiHandler.inputFld.isFocused);
                }
            
                // Stop Voice Record & Send it
                if (Input.GetKeyUp(KeyCode.Space) && IsRecording){
                    IsRecording = false;
                    gcsr.StopRecordButtonOnClickHandler();
                    Debug.Log("Space: Stop Record| " + uiHandler.inputFld.isFocused);
                }
                
            }
            
        }
    }

}
