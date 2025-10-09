using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;

public enum AIGender{
    Male,
    Female,
}

public class GameManager_AIMA : MonoBehaviour
{
    public static GameManager_AIMA Instance;
    public string voiceIdTTS_Female = "id-ID-Standard-A";
    public string voiceIdTTS_Male = "id-ID-Standard-B";
    public AIGender genderAI;
    public string selectedWallpaperPath;

    private void Awake() {
        if(Instance != null && Instance != this){
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy() {
        if(Instance == this){
            Instance = null;
        }
    }

    private void Start() {
        // For testing
        // genderAI = AIGender.Female;
    }

    
    public void LoadSelectedBackground(Image background){
        if(string.IsNullOrEmpty(Instance.selectedWallpaperPath)){
            Debug.Log("Wallpaper hasn't been selected yet");
            return;
        }

        Debug.Log($"Loading image, Path: {Instance.selectedWallpaperPath}");
        background.sprite = CreateSpriteFromLocalImg(Instance.selectedWallpaperPath);
    }
    public Sprite CreateSpriteFromLocalImg(string selectedImgPath){
        // Read the Img Bytes
        byte[] imgBytes = File.ReadAllBytes(selectedImgPath);

        // Prep & Load the texture from the Bytes
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imgBytes);

        // Return the Loaded Sprite to the Background's Img
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

    }


    public void SetGenderMale(){
        genderAI = AIGender.Male;
    }
    public void SetGenderFemale(){
        genderAI = AIGender.Female;
    }

    public string GetFemaleVoiceID(){
        return voiceIdTTS_Female;
    }
    public string GetMaleVoiceID(){
        return voiceIdTTS_Male;
    }

    public void GoToSceneAIMA(){
        SceneManager.LoadScene("Aima AI");
    }
    public void GoToSceneGenderSelect(){
        SceneManager.LoadScene("Gender Selection");
    }
    public void GoToSceneBackgroundSelect(){
        SceneManager.LoadScene("Background Selection");
    }
    public void ReloadCurrentScene(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
