using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System.IO;
using System.Xml.Linq;
using SimpleFileBrowser;
using FrostweepGames.Plugins.GoogleCloud.SpeechRecognition;

public class AzureTTSRest : MonoBehaviour
{
    public string azureKey = "YOUR_AZURE_KEY";
    public string azureRegion = "YOUR_AZURE_REGION";
    public string TTSurl = "YOUR_URL";
    public AudioSource audioSource;

    [Space(10)]
    [Header("Output Settings")]
    public string outputFolderName = "TTS_Audio";
    public AimaBotHandler aimaHandler;

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
            string folderPath = Path.Combine(Application.dataPath, outputFolderName);
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string filePath = Path.Combine(folderPath, fileName + ".wav");
            File.WriteAllBytes(filePath, audioData);
            Debug.Log($"Saved WAV to: {filePath}");
            yield return StartCoroutine(LoadAndPlay(filePath));
            aimaHandler.SetOutputText(text);
        }
        else
        {
            Debug.LogError("Azure TTS Error: " + request.error);
        }
    }

    private IEnumerator PlayAudioClip(string filePath)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + filePath, AudioType.WAV))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                audioSource.clip = clip;
                audioSource.Play();
                Debug.Log("Playing synthesized audio...");
            }
            else
            {
                Debug.LogError("Failed to load WAV: " + www.error);
            }
        }
    }

    private AudioClip CreateAudioClipFromPCM(byte[] pcmData, int offset, int length, int sampleRate, int channels)
    {
        int totalSamples = length / 2;
        float[] samples = new float[totalSamples];

        int sampleIndex = 0;
        for (int i = offset; i < offset + length; i += 2)
        {
            short value = System.BitConverter.ToInt16(pcmData, i);
            samples[sampleIndex++] = Mathf.Clamp(value / 32768.0f, -1f, 1f);
        }

        AudioClip clip = AudioClip.Create("AzureTTS_Clip", totalSamples / channels, channels, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private int DetectPCMOffset(byte[] bytes)
    {
        // Look for the "data" chunk header if it's a WAV
        for (int i = 0; i < bytes.Length - 4; i++)
        {
            if (bytes[i] == 'd' && bytes[i + 1] == 'a' && bytes[i + 2] == 't' && bytes[i + 3] == 'a')
            {
                // Skip "data" + 4 bytes of chunk size
                int offset = i + 8;
                Debug.Log($"Detected WAV header, skipping first {offset} bytes");
                return offset;
            }
        }
        return 0; // assume pure PCM
    }

    private IEnumerator LoadAndPlay(string filePath)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + filePath, AudioType.WAV))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"Loaded WAV from: {filePath} type : {www.GetType()}");
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                Debug.Log($"clip load {clip.loadState}");
                audioSource.clip = clip;
                audioSource.Play();
                Debug.Log($"Playing {clip.name} saved WAV audio");
            }
            else
            {
                Debug.LogError("Failed to load WAV: " + www.error);
            }
        }
    }
}
