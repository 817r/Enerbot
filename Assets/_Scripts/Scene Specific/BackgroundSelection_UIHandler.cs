using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class BackgroundSelection_UIHandler : MonoBehaviour
{
    public Button backBtn;
    public Button closeMaxWallpaperPopupBtn;
    public Button closeDupeWallpaperPopupBtn;
    public GameObject maxWallpaperPopupPanel;
    public GameObject dupeWallpaperPopupPanel;

    
    [DllImport("user32.dll")]
    private static extern bool ShowWindow(System.IntPtr windowHandler, int nCmdShow);
    private const int SW_MINIMIZE = 6;
    [DllImport("user32.dll", SetLastError = true)]
    private static extern System.IntPtr GetForegroundWindow();

    private void Start() {
        backBtn.onClick.AddListener(BackBtnOnClick);
        closeMaxWallpaperPopupBtn.onClick.AddListener(HideMaxWallpaperPopup);
        closeDupeWallpaperPopupBtn.onClick.AddListener(HideDupeWallpaperPopup);
    }

    public void BackBtnOnClick(){
        GameManager_AIMA.Instance.GoToSceneGenderSelect();
    }

    public void ShowMaxWallpaperPopup(){
        maxWallpaperPopupPanel.SetActive(true);
    }
    public void HideMaxWallpaperPopup(){
        maxWallpaperPopupPanel.SetActive(false);
    }

    public void ShowDupeWallpaperPopup(){
        dupeWallpaperPopupPanel.SetActive(true);
    }
    public void HideDupeWallpaperPopup(){
        dupeWallpaperPopupPanel.SetActive(false);
    }
    
    public void ExitAPP(){
        Debug.LogWarning("Exiting App");
        Application.Quit();
    }

    public void MinimizeApp(){
        System.IntPtr windowHandler = GetForegroundWindow();
        ShowWindow(windowHandler, SW_MINIMIZE);
    }

}
