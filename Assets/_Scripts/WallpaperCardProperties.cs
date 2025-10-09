using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class WallpaperCardProperties : MonoBehaviour
{
    public Image cardImg;
    public Button deleteBtn;
    public Button selectCardBtn;

    private string imgPath;
    [HideInInspector] public WallpaperManager manager;

    private void Start() {
        deleteBtn.onClick.AddListener(DeleteCard);
        selectCardBtn.onClick.AddListener(CardSelected);
    }

    public void InitializeCard(Sprite sprite, string path, WallpaperManager wallpaperManager){
        cardImg.sprite = sprite;
        imgPath = path;
        manager = wallpaperManager;
    }

    public void CardSelected(){
        if(GameManager_AIMA.Instance == null){
            Debug.LogError("Game Manager not found!");
            return;
        }

        GameManager_AIMA.Instance.selectedWallpaperPath = imgPath;
        manager.SaveCardData();
        GameManager_AIMA.Instance.GoToSceneAIMA();
        // Debug.Log($"Card {name} has been selected with the path of {imgPath}");
    }

    public void DeleteCard(){
        if(GameManager_AIMA.Instance == null){
            Debug.LogError("Game Manager not found!");
            return;
        }
        
        manager.DeleteCard(this);

        if(File.Exists(imgPath)){
            File.Delete(imgPath);
        }
        Destroy(gameObject);
    }

    public string GetImgPath(){
        return imgPath;
    }
}
