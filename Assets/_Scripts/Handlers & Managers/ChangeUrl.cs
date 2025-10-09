using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class ChangeUrl : MonoBehaviour
{
    [SerializeField] AzureAPIHandler azureAPIHandler;

    [SerializeField] GameObject mainBox;
    [SerializeField] TextMeshProUGUI currUrlText;
    [SerializeField] TMP_InputField inputUrlField;
    [SerializeField] UnityEvent successEvent, failedEvent;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("urlAPI"))
        {
            azureAPIHandler.apiURL = PlayerPrefs.GetString("urlAPI");
        }
        else
        {
            Debug.Log("No saved value found.");
        }

        currUrlText.text = azureAPIHandler.apiURL;
    }

    public void UpdateNewUrl()
    {
        azureAPIHandler.apiURL = inputUrlField.text;
        // StartCoroutine(GetDataFromAPI());
        successEvent.Invoke();
    }

    IEnumerator GetDataFromAPI()
    {
        UnityWebRequest request = UnityWebRequest.Get(azureAPIHandler.apiURL);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            //currUrlText.text = azureAPIHandler.apiURL;
            successEvent.Invoke();
            Debug.Log("API hit successfully!");
            Debug.Log("Response Text: " + request.downloadHandler.text);
        }
        else
        {
            failedEvent.Invoke();
            Debug.LogError("Failed to hit API. Response Code: " + request.responseCode);
        }
    }

    public void DisableSuccessBox(GameObject successBox)
    {
        StartCoroutine(AfterSuccess(successBox));
    }

    IEnumerator AfterSuccess(GameObject successBox)
    {
        currUrlText.text = azureAPIHandler.apiURL;
        PlayerPrefs.SetString("urlAPI", azureAPIHandler.apiURL);
        PlayerPrefs.Save();

        yield return new WaitForSeconds(3f);

        successBox.SetActive(false);
        mainBox.SetActive(true);
        currUrlText.text = azureAPIHandler.apiURL;
    }

    public void PasteFromClipboard()
    {
        string clipboardText = GUIUtility.systemCopyBuffer;
        inputUrlField.text = clipboardText;
    }
}
