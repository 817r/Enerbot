using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;

public class AzureTTSRest : MonoBehaviour
{
    public string azureKey = "YOUR_AZURE_KEY";
    public string azureRegion = "YOUR_AZURE_REGION";
    public string TTSurl = "YOUR_URL";
    public AudioSource audioSource;

    [Space(10)]
    [Header("Output Settings")]
    public string outputFolderName = "TTS_Audio";
    public AimaBotHandler enerbotHandler;

    public IEnumerator Speak(string text, string fileName)
    {
        
        string ttsUrl = $"https://{azureRegion}.tts.speech.microsoft.com/cognitiveservices/v1";
        string ssml = $@"
<speak version='1.0' xml:lang='id-ID'>
  <voice xml:lang='id-ID' name='id-ID-GadisNeural'>
    {text}
  </voice>
</speak>";

        UnityWebRequest request = new UnityWebRequest(ttsUrl, "POST");
        byte[] body = Encoding.UTF8.GetBytes(ssml);
        request.uploadHandler = new UploadHandlerRaw(body);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Ocp-Apim-Subscription-Key", azureKey);
        request.SetRequestHeader("Content-Type", "application/ssml+xml");
        request.SetRequestHeader("X-Microsoft-OutputFormat", "audio-16khz-32kbitrate-mono-mp3");

        Debug.Log("Sending TTS request to Azure...");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            byte[] audioData = request.downloadHandler.data;

            // Ensure output directory exists
            string folderPath = Path.Combine(Application.persistentDataPath, outputFolderName);
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string filePath = Path.Combine(folderPath, fileName + ".mp3");
            File.WriteAllBytes(filePath, audioData);
            Debug.Log($"Azure TTS saved: {filePath}");

            // Convert MP3 to AudioClip and play
            yield return StartCoroutine(LoadAndPlayAudio(filePath));

            enerbotHandler.SetOutputText(text);
        }
        else
        {
            Debug.LogError("Azure TTS Error: " + request.error);
        }
    }

    private IEnumerator LoadAndPlayAudio(string filePath)
    {
        using UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + filePath, AudioType.MPEG);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
            audioSource.clip = clip;
            audioSource.Play();
            Debug.Log("Playing Azure TTS Audio");
        }
        else
        {
            Debug.LogError("❌ Failed to load audio: " + www.error);
        }
    }
}
