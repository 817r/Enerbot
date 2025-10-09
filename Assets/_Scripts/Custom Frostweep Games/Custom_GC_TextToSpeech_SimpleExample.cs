using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using FrostweepGames.Plugins.Core;
using FrostweepGames.Plugins.GoogleCloud.TextToSpeech;

namespace FrostweepGames.Plugins.GoogleCloud.TextToSpeech.Example
{
    public class Custom_GC_TextToSpeech_SimpleExample : MonoBehaviour
    {
        private GCTextToSpeech _gcTextToSpeech;

        public Button synthesizeButton;

        public AudioSource audioSource;
        [Tooltip("Will be override by GameManager_AIMA.cs")]
        public string voiceName;
        public float speakRate = 1.0f;

        [Header("Additional References")]
        public VideoHandler vidHandler;
        public UIHandler uiHandler;

        private Coroutine goIdleCoroutine;

        private void Start()
        {
            _gcTextToSpeech = GCTextToSpeech.Instance;

            _gcTextToSpeech.SynthesizeSuccessEvent += _gcTextToSpeech_SynthesizeSuccessEvent;

            _gcTextToSpeech.GetVoicesFailedEvent += _gcTextToSpeech_GetVoicesFailedEvent;
            _gcTextToSpeech.SynthesizeFailedEvent += _gcTextToSpeech_SynthesizeFailedEvent;

            if(GameManager_AIMA.Instance.genderAI == AIGender.Female){
                voiceName = GameManager_AIMA.Instance.GetFemaleVoiceID();
            }else{
                voiceName = GameManager_AIMA.Instance.GetMaleVoiceID();
            }

            // synthesizeButton.onClick.AddListener(SynthesizeButtonOnClickHandler);
        }

        public void SynthesizeButtonOnClickHandler(string content)
        {
            // string content = uiHandler.outputText_AI.text;

            if (string.IsNullOrEmpty(content))
                return;

            if (GeneralConfig.Config.betaAPI)
            {
                _gcTextToSpeech.Synthesize(content, new VoiceConfig()
                {
                    gender = Enumerators.SsmlVoiceGender.FEMALE,
                    // languageCode = _gcTextToSpeech.PrepareLanguage(Enumerators.LanguageCode.en_US),
                    // name = "en-US-Neural2-C"
                    languageCode = _gcTextToSpeech.PrepareLanguage(Enumerators.LanguageCode.en_US),
                    name = voiceName
                },
                false,
                0.0,
                speakRate,
                Constants.DEFAULT_SAMPLE_RATE, new Enumerators.EffectsProfileId[] { Enumerators.EffectsProfileId.SMALL_HOME_SPEAKER },
                new Enumerators.TimepointType[] { Enumerators.TimepointType.TIMEPOINT_TYPE_UNSPECIFIED }
                //,
                //new CustomVoiceParams()
                //{
                //    reportedUsage = ReportedUsage.REALTIME,
                //    model = "projects/{project_id}/locations/us-central1/models/{model_id}"
                //}
                );
            }
            else
            {
                _gcTextToSpeech.Synthesize(content, new VoiceConfig()
                {
                    gender = Enumerators.SsmlVoiceGender.FEMALE,
                    // languageCode = _gcTextToSpeech.PrepareLanguage(Enumerators.LanguageCode.en_US),
                    // name = "en-US-Neural2-C"
                    languageCode = _gcTextToSpeech.PrepareLanguage(Enumerators.LanguageCode.en_US),
                    name = voiceName
                },
                false,
                0.0,
                speakRate,
                Constants.DEFAULT_SAMPLE_RATE, new Enumerators.EffectsProfileId[] { Enumerators.EffectsProfileId.SMALL_HOME_SPEAKER });
            }
        }

        #region failed handlers

        private void _gcTextToSpeech_SynthesizeFailedEvent(string error, long requestId)
        {
            Debug.Log(error);
        }

        private void _gcTextToSpeech_GetVoicesFailedEvent(string error, long requestId)
        {
            Debug.Log(error);
        }

        #endregion failed handlers

        #region sucess handlers

        private void _gcTextToSpeech_SynthesizeSuccessEvent(PostSynthesizeResponse response, long requestId)
        {
            audioSource.clip = _gcTextToSpeech.GetAudioClipFromBase64(response.audioContent, Constants.DEFAULT_AUDIO_ENCODING);
            audioSource.Play();

            if(goIdleCoroutine != null){
                StopCoroutine(goIdleCoroutine);
            }
                goIdleCoroutine = StartCoroutine(vidHandler.WaitGoIdle(audioSource.clip.length + 0.5f));

            // ADDED
            
			
            // SAVE FILE TO LOCAL STORAGE
			/* ServiceLocator.Get<IMediaManager>().
				SaveAudioFileAsFile(response.audioContent, 
									Application.dataPath, 
									"sound_" + System.DateTime.Now.ToString(), 
									Enumerators.AudioEncoding.LINEAR16); */
        }

        #endregion sucess handlers
    }
}