using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class UIHandler : MonoBehaviour
{
    public Button sendBtn;
    public Button startRecBtn;
    public Button stopRecBtn;
    public Button refreshMicBtn;
    public Button backBtn;
    public Dropdown micDeviceDropdown;
    public Image voiceLvImg;
    public Image backgroundUI;
    public TMP_InputField inputFld;
    public TextMeshProUGUI outputText_AI;
    public Text resultTxt;

    [Header("References")]
    public SpeechRecognitionHandler speechRecogHandler;

    
    [DllImport("user32.dll")]
    private static extern bool ShowWindow(System.IntPtr windowHandler, int nCmdShow);
    private const int SW_MINIMIZE = 6;
    [DllImport("user32.dll", SetLastError = true)]
    private static extern System.IntPtr GetForegroundWindow();

    private void Start() {
        sendBtn.onClick.AddListener(SendMessage);
        backBtn.onClick.AddListener(BackToBackgroundSelectionScene);
        refreshMicBtn.onClick.AddListener(RefreshScene);
        GameManager_AIMA.Instance.LoadSelectedBackground(backgroundUI);
    }

    private void Update() {
        // if(inputFld.text.Length <= 0){
        //     sendBtn.interactable = false;
        // }else{
        //     sendBtn.interactable = true;
        // }
    }
    
    private void SendMessage(){
        if(inputFld.text.Length <= 0){
            Debug.Log("Enter your prompt first!");
            return;
        }

        speechRecogHandler.SendMsg();
        sendBtn.interactable = false;
    }

    public void ExitAPP(){
    Debug.LogWarning("Exiting App");
    Application.Quit();
    }

    public void MinimizeApp(){
        System.IntPtr windowHandler = GetForegroundWindow();
        ShowWindow(windowHandler, SW_MINIMIZE);
    }

    private void BackToBackgroundSelectionScene(){
        GameManager_AIMA.Instance.GoToSceneBackgroundSelect();
    }
    private void RefreshScene(){
        GameManager_AIMA.Instance.ReloadCurrentScene();
    }

}
