using UnityEngine;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System.Collections.Concurrent;
using System;

public class AzureeSTT : MonoBehaviour
{
    [Header("Azure Settings")]
    public string azureKey = "YOUR_AZURE_KEY";
    public string azureRegion = "YOUR_AZURE_REGION";
    public string recognitionLanguage = "en-US"; // or "id-ID"
    public UIHandler uiHandler;
    public SpeechRecognitionHandler srh;
    private SpeechRecognizer recognizer;
    private readonly ConcurrentQueue<Action> mainThreadActions = new ConcurrentQueue<Action>();

    void Update()
    {
        while (mainThreadActions.TryDequeue(out var action))
            action?.Invoke();
    }

    public void UpdateTMPFromThread(string text)
    {
        mainThreadActions.Enqueue(() => {
            uiHandler.inputFld.text = text;
        });
    }

    async void Start()
    {
        await InitializeRecognizer();
    }

    async Task InitializeRecognizer()
    {
        var config = SpeechConfig.FromSubscription(azureKey, azureRegion);
        config.SpeechRecognitionLanguage = recognitionLanguage;

        // Use the default system microphone
        var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
        recognizer = new SpeechRecognizer(config, audioConfig);

        recognizer.Recognizing += (s, e) =>
        {
            Debug.Log($"Recognizing: {e.Result.Text}");
        };

        recognizer.Recognized += (s, e) =>
        {
            if (e.Result.Reason == ResultReason.RecognizedSpeech)
            {
                Debug.Log($"Final: {e.Result.Text}");
                UpdateTMPFromThread(e.Result.Text);
            }
            else if (e.Result.Reason == ResultReason.NoMatch)
                Debug.Log("No speech recognized.");
        };

        recognizer.Canceled += (s, e) =>
        {
            Debug.LogWarning($"Canceled: {e.Reason}");
        };

        recognizer.SessionStopped += (s, e) =>
        {
            Debug.Log("Session stopped.");
        };

        Debug.Log("Recognizer ready. Call StartRecognition() to begin.");
    }

    public async void StartRecognition()
    {
        if (recognizer == null)
        {
            Debug.LogError("Recognizer not initialized yet.");
            return;
        }

        Debug.Log("Listening... Speak into your mic.");
        await recognizer.StartContinuousRecognitionAsync();
    }
     
    public async void StopRecognition()
    {
        if (recognizer != null)
        {
            Debug.Log("Stopping recognition...");
            srh.SendMsg();
            await recognizer.StopContinuousRecognitionAsync();
        }
    }

    async void OnApplicationQuit()
    {
        if (recognizer != null)
        {
            await recognizer.StopContinuousRecognitionAsync();
            recognizer.Dispose();
        }
    }
}
