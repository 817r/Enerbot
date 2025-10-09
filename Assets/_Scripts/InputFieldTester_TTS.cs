using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldTester_TTS : MonoBehaviour
{
    public TMP_InputField inputFld;

    private void Start() {
        inputFld.text = "Hii, my name Aima, i'm an AI from Pertmaina Trans Kontinental. How can i help you?";
        // hai nama saya Aima, sebuah AI dari Pertamina Trans Kontinental. Bagaimana saya bisa membantu?
    }
    
}
