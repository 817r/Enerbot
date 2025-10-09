using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Diagnostics;
using System.IO;

public class VirtualKeyboard : MonoBehaviour
{
    // public GameObject virtualKeyboard; // If u have custom Virtual Keyboard
    public RectTransform uiContainer; // UI that will be moved up & down
    public TMP_InputField inputFld;
    public float keyboardHeight = 300f; // Approx height of keyboard

    private Vector2 originalUIPos;
    private Process oskProcess;

    void Start()
    {
        originalUIPos = uiContainer.anchoredPosition;
        // HideKeyboard();

        inputFld.onSelect.AddListener(delegate {ShowKeyboard();});
        inputFld.onDeselect.AddListener(delegate {HideKeyboard();});

    }

    public void ShowKeyboard(){
        uiContainer.anchoredPosition = new Vector2(uiContainer.anchoredPosition.x, uiContainer.anchoredPosition.y + keyboardHeight);
        // virtualKeyboard.SetActive(true);

        try{
            // TabTip.exe for Touch Keyboard
            // osk.exe for On-Screen Keyboard
            oskProcess = Process.Start("osk.exe");
        }
        catch(System.Exception ex){
            UnityEngine.Debug.LogError("Failed to open On Screen Keyboard: " + ex.Message);
        }

    }

    public void HideKeyboard(){
        uiContainer.anchoredPosition = originalUIPos;
        // virtualKeyboard.SetActive(false);

        // Close the OSK if its open
        if(oskProcess != null && !oskProcess.HasExited){
            oskProcess.Kill();
            oskProcess = null;
        }

    }

#region Custom Keyboard Functions
    public void OnKeyPressVK(string key){
        inputFld.text += key;

    }

    public void OnBackspaceVK(){
        if(inputFld.text.Length > 0){
            inputFld.text = inputFld.text.Substring(0, inputFld.text.Length - 1);
        }
    }
    
#endregion

}
