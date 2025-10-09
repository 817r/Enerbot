using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class GenderSelection_UI : MonoBehaviour
{
    public Button maleBtn;
    public Button femaleBtn;
    
    [DllImport("user32.dll")]
    private static extern bool ShowWindow(System.IntPtr windowHandler, int nCmdShow);
    private const int SW_MINIMIZE = 6;
    [DllImport("user32.dll", SetLastError = true)]
    private static extern System.IntPtr GetForegroundWindow();

    void Start()
    {
        maleBtn.onClick.AddListener(MaleBtnOnClick);
        femaleBtn.onClick.AddListener(FemaleBtnOnClick);
        ResetScaleButton(maleBtn);
        ResetScaleButton(femaleBtn);
    }

    
    public void ExitAPP(){
        Debug.LogWarning("Exiting App");
        Application.Quit();
    }

    public void MinimizeApp(){
        System.IntPtr windowHandler = GetForegroundWindow();
        ShowWindow(windowHandler, SW_MINIMIZE);
    }


    private void ScaleUpButton(Button btn){
        btn.gameObject.transform.localScale = new Vector3(1.25f, 1.25f, 1.25f);
    }
    private void ResetScaleButton(Button btn){
        btn.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    private void MaleBtnOnClick(){
        ScaleUpButton(maleBtn);
        GameManager_AIMA.Instance.SetGenderMale();
        GameManager_AIMA.Instance.GoToSceneBackgroundSelect();
        // ResetScaleButton(maleBtn);
    }
    private void FemaleBtnOnClick(){
        ScaleUpButton(femaleBtn);
        GameManager_AIMA.Instance.SetGenderFemale();
        GameManager_AIMA.Instance.GoToSceneBackgroundSelect();
        // ResetScaleButton(femaleBtn);
    }
    
}
