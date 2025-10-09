using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;
using System;

[System.Serializable]
public class ChatRequestData{
    // All Case Sensitive
    public DataSource[] data_sources;
    public MessageData[] messages;
    public float temperature;
    public bool use_semantic_captions;
    public bool use_semantic_ranker;
    public float top;
    // public VectorField vector_fields;
    // public bool use_text_search;
    public string model;
    public string gpt4v_input;
    public bool use_gpt4v;
    // public string deployment;

    // For Testing
    public object stop;
    public int max_tokens;
    public float top_p;
    public bool stream;
}

[System.Serializable]
public class DataSource{
    // All Case Sensitive
    public string type;
    public DataSourceParameters parameters;
}

[System.Serializable]
public class DataSourceParameters{
    // All Case Sensitive
    public string endpoint;
    public string index_name;
    public string semantic_configuration;
    public string query_type;
    public bool in_scope;
    public string role_information;
    public string filter;
    public int strictness;
    public int top_n_documents;
    public Authentication authentication;
    public FieldsMapping fields_mapping;
}
[System.Serializable]
public class VectorField{

}

[System.Serializable]
public class FieldsMapping{

}

[System.Serializable]
public class Authentication{
    public string type;
    public string key;
}

[System.Serializable]
public class MessageData
{
    // All Case Sensitive
    public string role;
    public string content;
}

public class AzureAPIHandler : MonoBehaviour
{
    // https://app-backend-zdnr77ukc6pu4.azurewebsites.net/chat (OLD, didnt work)
    // https://app-backend-poophqnjqo6k6.azurewebsites.net/chat (OLD V2)
    public string apiURL = "";
    public string apiKey = "";
    
    public delegate void OnSendChatRequestFinished(string _botMessageContent); 

    public IEnumerator SendChatReq(string msgContent, OnSendChatRequestFinished callback){
        ChatRequestData reqData = new ChatRequestData();

        reqData.data_sources = new DataSource[]{
            new DataSource
            {
                type = "azure_search",
                parameters = new DataSourceParameters
                {
                    endpoint = @"https://azureopenaisearchptmn.search.windows.net",
                    index_name = "sieraexternal",
                    semantic_configuration = "default",
                    query_type = "semantic",
                    fields_mapping = new FieldsMapping(),
                    in_scope = true,
                    role_information = "",
                    filter = null,
                    strictness = 3,
                    top_n_documents = 5,
                    authentication = new Authentication
                    {
                        type = "api_key",
                        key = "giDkRr6y8m2diEAex9WDpBqLn1qFZlZQpamzgV2NHCAzSeCTJpfG"
                    }
                }
            }
        };

        reqData.messages = new MessageData[]{
            new MessageData{
                role = "user",
                content = msgContent
            }
        };


#region Aima
        // (OLD)
        // reqData.temperature = 0.3f;
        // reqData.use_semantic_captions = false;
        // reqData.use_semantic_ranker = true;
        // reqData.top = 3f;
        // reqData.vector_fields = new VectorField();
        // reqData.use_text_serach = true;
        // reqData.model = "gpt-4o";
        // reqData.deployment = "gpt-4o";

        reqData.gpt4v_input = "textAndImages";
        reqData.use_semantic_captions = false;
        reqData.use_semantic_ranker = true;
        reqData.temperature = 0.3f;
        reqData.top = 3;
        reqData.use_gpt4v = true;
        reqData.max_tokens = 800;
        reqData.stop = null;
        reqData.stream = false;

#endregion
       
#region For Testing
        // reqData.temperature = 0;
        // reqData.top_p = 1;
        // reqData.max_tokens = 800;
        // reqData.stop = null;
        // reqData.stream = false;
#endregion
       
        // Create HTTP Request
        string jsonReq = JsonUtility.ToJson(reqData);
        UnityWebRequest request = new UnityWebRequest(apiURL, "POST");

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonReq);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("api-key", apiKey);

        yield return request.SendWebRequest();

        if(request.result == UnityWebRequest.Result.Success){
            string jsonResponse = request.downloadHandler.text;
            Debug.Log("Response: " + jsonResponse);

            JObject jsonObj = JObject.Parse(jsonResponse);
            
            // string _botMsg = jsonObj["choices"][0]["message"]["content"].ToString().Split('[')[0].Trim();
            string _botMsg = jsonObj["message"]["content"].ToString().Split('[')[0].Trim();
            Debug.Log("botMessage: " + _botMsg);
            callback(_botMsg);

        }
        else{
            Debug.LogError("Error-Error: " + request.error);
            Debug.LogError("Error-Payload: " + jsonReq);
            Debug.LogError("Error-Result: " + request.responseCode + " " + request.result);
            Debug.LogError("Error-ApiURL: " + apiURL);
            Debug.LogError("Error-ApiKey: " + apiKey);
            Debug.LogError("Error-Content " + msgContent);
        }

    }
}
