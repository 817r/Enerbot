using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testings : MonoBehaviour
{
    public AzureTTSRest azureTTSRest;
    [TextArea] public string textToSpeak = "Halo, nama saya Aima!";
    void Start()
    {
        StartCoroutine(azureTTSRest.Speak(textToSpeak, "greeting"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
