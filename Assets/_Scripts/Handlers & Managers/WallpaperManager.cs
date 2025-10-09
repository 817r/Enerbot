using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleFileBrowser;
using System.IO;
using System;

[System.Serializable]
public class CardData{
    public string imagePath;
}

[System.Serializable]
public class CardDataList{
    public List<CardData> cards = new List<CardData>();
}

[System.Serializable]
public class WallpaperData{
    public CardDataList cardDataList = new CardDataList();
    public string selectedWallpaperPath;
    public bool isPresetLoaded;
}

public class WallpaperManager : MonoBehaviour
{
    public GameObject wallpaperCardPrefab;
    public Transform cardContainer;
    public Button addCardBtn;
    public int cardCount;

    public Image testChangeBG;
    public Button testChangeImgBtn;

    [Header("References")]
    public BackgroundSelection_UIHandler bgUIHandler;

    private string saveDataPath;
    private string saveImgsPath;
    private WallpaperData wallpaperData = new WallpaperData();
    private string[] presetNames = {"WP-1", "WP-2", "WP-3"};

    void Start()
    {
        // Check if wallpaperData have been created or not
        if(wallpaperData != null){
            // Set the wallpaperData.isPresetLoaded to be the same value as the one stored in Player Prefs
            wallpaperData.isPresetLoaded = SetIsPresetLoaded_PlayerPrefs();
            // Debug.Log("[START] Wallpaper Is Preset Loaded: " + wallpaperData.isPresetLoaded);
            // Debug.Log("[START] PLAYER PREFS Is Preset Loaded: " + SetIsPresetLoaded_PlayerPrefs());
        }else{
            Debug.Log("WallpaperData is null");
        }

        // Specify the path to save the json file and Imgs
        saveDataPath = Path.Combine(Application.persistentDataPath, "wallpaperData.json");
        saveImgsPath = Path.Combine(Application.persistentDataPath, "Images");
        
        // Create Folder if the folder isn't there yet (To save Images)
        if(!Directory.Exists(saveImgsPath)){
            Directory.CreateDirectory(saveImgsPath); 
        }

        addCardBtn.onClick.AddListener(CreateNewCard);

        // Load Preset Wallpapers & Saved Data
        LoadPresetWallpapers();
        LoadSavedCards();

        // Check the count of Wallpapers when opening the Background Selection Scene
        cardCount = CheckCardCount();

        // Show/Hide the Max Wallpaper Popup
        if(cardCount == 12){
            ShowAddCardBtn(false);
            bgUIHandler.ShowMaxWallpaperPopup();
            // Debug.Log(false);
        }else{
            ShowAddCardBtn(true);
            bgUIHandler.HideMaxWallpaperPopup();
            // Debug.Log(true);
        }
    }

#region Player Prefs Functions
    public void SaveIsPresetLoaded_PlayerPrefs(bool isPresetLoaded){
        // Function to save the isPresetLoaded boolean to Player Prefs
        PlayerPrefs.SetInt("WallpaperData_IsPresetLoaded", isPresetLoaded ? 1 : 0);
        PlayerPrefs.Save();
    }
    public bool SetIsPresetLoaded_PlayerPrefs(){
        // Function to return the value of isPresetLoaded from Player Prefs
        if(PlayerPrefs.HasKey("WallpaperData_IsPresetLoaded")){
            if(PlayerPrefs.GetInt("WallpaperData_IsPresetLoaded") == 1){
                return true;

            }
        }
        return false;
    }
#endregion

    public void CreateNewCard(){
        if(GameManager_AIMA.Instance == null){
            Debug.LogError("Game Manager not found!");
            return;
        }

        // Open file to select card's background
        OpenFileManager();

    }
    public void DeleteCard(WallpaperCardProperties card){
        // Store the img path of the Wallpaper to be Deleted
        string imgPath = card.GetImgPath();

        // If the To be Deleted Wallpaper is the same as the one being used as Background, Remove it
        if(imgPath == GameManager_AIMA.Instance.selectedWallpaperPath){
            GameManager_AIMA.Instance.selectedWallpaperPath = "";
        }

        // Remove the To be Deleted Wallpaper from Card Data List
        wallpaperData.cardDataList.cards.RemoveAll(c => c.imagePath == imgPath);
        wallpaperData.selectedWallpaperPath = "";
        
        // Save the changes (Update it, because a Card has been Deleted)
        SaveCardData();

        // Decrease the Count for Wallpaper & if the count isn't 12 (MAX), Show the Add Wallpaper Button
        cardCount--;
        if(cardCount != 12){
            ShowAddCardBtn(true);
        }
    }

    public void OpenFileManager()
    {
        // Define the Downloads Folder Path
        string downloadsPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads";

        // Set the File Browser's Filters, default Filter 
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Images", ".jpg", ".jpeg", ".png"));
        FileBrowser.SetDefaultFilter(".png");
        
        // Add Folder to the Quick Link Section of the File Browser (Left Side of the File Browser)
        FileBrowser.AddQuickLink("Users", "C:\\Users", null);
        FileBrowser.AddQuickLink("Downloads", downloadsPath, null);
        
        // Show a file browser window
        StartCoroutine(ShowLoadDialogCoroutineAndCreateCard());
    }

    private IEnumerator ShowLoadDialogCoroutineAndCreateCard()
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, false, "Select an Image", "Load");

        // After a file has been Selected and Valid
        if (FileBrowser.Success)
        {
            string srcPath = FileBrowser.Result[0]; // Get the selected Img's Path
            string destinationPath = Path.Combine(saveImgsPath, Path.GetFileName(srcPath)); // destination to save the selected img (in this case **/Images/*selected Img*)
            
            // // Duplicate allowed (Only max to 2 same image, edit the additional name [the "(1)"] so it can be more than 2)
            // if(File.Exists(destinationPath)){
            //     string newSrcPath = Path.GetFileNameWithoutExtension(srcPath) + " (1)" + Path.GetExtension(srcPath); // Add a new additional name
            //     destinationPath = Path.Combine(saveImgsPath, newSrcPath);
            // }

            // Duplicate not allowed
            if(File.Exists(destinationPath)){
                Debug.LogWarning("File already uploaded!");
                bgUIHandler.ShowDupeWallpaperPopup();
                yield break; // Skip duplication
            }
        
            File.Copy(srcPath, destinationPath, false); // Copy the selected image (because it will be saved in local later), won't overwrite

            byte[] imgBytes = FileBrowserHelpers.ReadBytesFromFile(srcPath); // Read the Img Bytes

            // Create the Card for Wallpaper
            CreateCardWithImg(imgBytes, destinationPath);
        }
    }
    
    // Function to Check the Wallpaper Count
    private int CheckCardCount(){
        cardCount = 0;

        for(int i = 0; i < cardContainer.childCount-1; i++){
            // Checking by counting the Child Count of the Card Container
            if(cardContainer.GetChild(i).CompareTag("WallpaperCard")){
                cardCount++;
                // Debug.Log(cardContainer.GetChild(i).name);
            }
        }
        
        return cardCount;
    }

    private void CreateCardWithImg(byte[] imgBytes, string imgPath){
        // Create a new GameObject for the Wallpaper Card + Set the Parent to cardContainer
        GameObject newCard = Instantiate(wallpaperCardPrefab, cardContainer);

        // Create a new empty Texture and Load the Img to the Texture from the Bytes
        Texture2D texture = new Texture2D(2,2);
        texture.LoadImage(imgBytes);

        // Create a Base Sprite to Assign the Texture and Set it to the Card's Img Component
        Image cardImg = newCard.GetComponentInChildren<Image>();
        Sprite cardSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        cardImg.sprite = cardSprite;

        // Initialize the Card's Properties
        WallpaperCardProperties cardProperties = newCard.GetComponent<WallpaperCardProperties>();
        cardProperties.InitializeCard(cardSprite, imgPath, this);

        // Save the new card to Card Data List
        wallpaperData.cardDataList.cards.Add(new CardData {imagePath = imgPath});
        // Debug.Log("Added Img Path: " + imgPath);
        
        // Save the card Data to Json (Update it)
        SaveCardData();

        // Adjust the Add Wallpaper Btn Pos
        AdjustAddCardBtn();

        // Increase the Wallpaper Count + Check if the Count is 12 (MAX), if so Show Popup
        cardCount++;
        if(cardCount == 12){
            ShowAddCardBtn(false);
            bgUIHandler.ShowMaxWallpaperPopup();
            // Debug.LogWarning("Wallpaper sudah mencapai 12");
        }

    }

    private void AdjustAddCardBtn(){
        // Transform addCardBtn = cardContainer.GetChild(cardContainer.childCount - 2); // -2 cause after Instantiating the new card, the add button card will be the second from last
        addCardBtn.transform.SetSiblingIndex(cardContainer.childCount - 1); // Set the Add Wallpaper Btn Card Pos as the Last
        // Debug.Log($"Card Name: {addCardBtn.name}");
    }
    private void ShowAddCardBtn(bool state){
        addCardBtn.gameObject.SetActive(state);
    }

    public void SaveCardData(){
        // Set the Selected Wallpaper Stored in wallpaperData for Json with the Value Stored in Game Manager
        wallpaperData.selectedWallpaperPath = GameManager_AIMA.Instance.selectedWallpaperPath;

        string json = JsonUtility.ToJson(wallpaperData, true); // Convert to Json
        // Debug.Log($"json: {json}");
        File.WriteAllText(saveDataPath, json); // Create the json file if not exist & Save the json in that path
    }

    public void SaveSortedCard(){
        // Delete the current cards in the cardDataList
        wallpaperData.cardDataList.cards.Clear();

        // Add the new Sorted Cards to the cardDataList
        for(int i = 0; i < cardContainer.childCount-1; i++){ // -1 to exclude The Add New Card
            WallpaperCardProperties cardProperties = cardContainer.GetChild(i).GetComponent<WallpaperCardProperties>();
            wallpaperData.cardDataList.cards.Add(new CardData {imagePath = cardProperties.GetImgPath()});
            // Debug.Log($"Cards {i}: {cardContainer.GetChild(i).GetComponent<WallpaperCardProperties>().GetImgPath()}");
        }

        // Save the newly Sorted Cards to the json file
        SaveCardData();
    }

    private void LoadSavedCards(){
        // Load existing Wallpaper
        if(File.Exists(saveDataPath)){
            string json = File.ReadAllText(saveDataPath); // Get the json from the Save Path
            wallpaperData = JsonUtility.FromJson<WallpaperData>(json); // Convert & Load the json data to Card Data List

            // Load Cards
            foreach(CardData cardData in wallpaperData.cardDataList.cards){
                if(File.Exists(cardData.imagePath)){
                    byte[] imgBytes = File.ReadAllBytes(cardData.imagePath);

                    // Same Process to Create the Wallpaper Cards
                    GameObject newCard = Instantiate(wallpaperCardPrefab, cardContainer);
                    
                    Texture2D texture = new Texture2D(2, 2);
                    texture.LoadImage(imgBytes);

                    Image cardImg = newCard.GetComponentInChildren<Image>();
                    Sprite cardSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                    cardImg.sprite = cardSprite;

                    WallpaperCardProperties cardProperties = newCard.GetComponent<WallpaperCardProperties>();
                    cardProperties.InitializeCard(cardSprite, cardData.imagePath, this);
                    // Debug.Log("Path: " + cardData.imagePath);

                }
                
            }
            
            AdjustAddCardBtn();

            // Load the selected image path into the GameManager (if any)
            if(!string.IsNullOrEmpty(wallpaperData.selectedWallpaperPath)){
                GameManager_AIMA.Instance.selectedWallpaperPath = wallpaperData.selectedWallpaperPath;
                // Debug.Log("Set Saved Selected Wallpaper...");
            }

        }
    }

    private void LoadPresetWallpapers(){
        // Debug.Log("IS JSON EXIUST? " + !File.Exists(saveDataPath));
        // Create Starting Json File, if JSON file doesnt exist
        if(!File.Exists(saveDataPath)){

            // Define the starting value of variables (wallpaperData & cardList already declared by default)
            wallpaperData.selectedWallpaperPath = "";
            // wallpaperData.isPresetLoaded = SetIsPresetLoaded_PlayerPrefs();

            string json = JsonUtility.ToJson(wallpaperData, true); // true, to make it easier for readability
            File.WriteAllText(saveDataPath, json); // Create New / Save to the existing Json according to the Path

        }

        // Debug.Log("PRESET LOADED? " + wallpaperData.isPresetLoaded);
        // Load Preset Wallpapers
        if(!wallpaperData.isPresetLoaded){
            Debug.Log("Loading Wallpaper Preset");

            // Set the Preset Folder Path Name
            string presetFolderName = "/WallpaperPresets";

            // Loop on how many Presets there are
            for(int i = 0; i < 3; i++){
                // Define the Path for each Preset's Path Name (Includes Extension)
                string srcFile = Path.Combine(Application.streamingAssetsPath + presetFolderName, presetNames[i] + GetFileExtensionFromStreamingAssets(presetFolderName, presetNames[i]));
                // Debug.Log($"StreamingAssets Path: {srcFile}");

                // Check if the Wallpaper is in the StreamingAssets Folder or not
                if(!File.Exists(srcFile)){
                    Debug.Log("Wallpaper not found in StreamingAssets!");
                    return;
                }

                // Define the New Location for the Image from StreamingAssets (Location to save the Copy of the Wallpaper Preset from StreamingAssets)
                string newSrcFile = Path.Combine(saveImgsPath, Path.GetFileName(srcFile));
                // Debug.Log($"New Src File: {newSrcFile}");
                
                // Copy the Image from StreamingAssets to Local Storage
                File.Copy(srcFile, newSrcFile);

                // Update the Data in Card Data List
                wallpaperData.cardDataList.cards.Add(new CardData {imagePath = newSrcFile});
                // Debug.Log("Added Img Path: " + newSrcFile);

                // Set the boolean for isPresetLoaded both on json and Player Prefs (So it wont be Loaded everytime even after being Deleted)
                wallpaperData.isPresetLoaded = true;
                SaveIsPresetLoaded_PlayerPrefs(true);
                // Debug.Log("PLAYER PREFS SAVED");
            }

            SaveCardData();

        }
    }

    private string GetFileExtensionFromStreamingAssets(string folderName, string fileName){      
        // Debug.Log($"Directory: {Application.streamingAssetsPath + folderName}");  
        string[] files = Directory.GetFiles(Application.streamingAssetsPath + folderName);
        
        foreach(string file in files){
            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(file);

            if(fileNameWithoutExt == fileName){
                // Debug.Log($"Fetched Extension: {Path.GetExtension(file)}");
                return Path.GetExtension(file);
            }
        }

        return null;
    }
}
