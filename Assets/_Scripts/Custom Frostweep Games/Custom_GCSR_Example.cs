using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace FrostweepGames.Plugins.GoogleCloud.SpeechRecognition.Examples
{
	public class Custom_GCSR_Example : MonoBehaviour
	{
		[HideInInspector] public string rawVoiceInput; // Added by Marcel, (Utk debug Txt)

		[Header("Silent Timer")]
		public float maxWaitRecordTimer = 4f;
		public float currRecordTimer;

		[Header("References")]
		public SpeechRecognitionHandler speechHandler;
		public UIHandler uIHandler;

		private GCSpeechRecognition _speechRecognition;

		private Button _startRecordButton,
					   _stopRecordButton,
					   _getOperationButton,
					   _getListOperationsButton,
					   _detectThresholdButton,
					   _cancelAllRequestsButton,
					   _recognizeButton,
					   _refreshMicrophonesButton;

		private Image _speechRecognitionState;

		private Text _resultText;

		private Toggle _voiceDetectionToggle,
					   _recognizeDirectlyToggle,
					   _longRunningRecognizeToggle;

		private Dropdown _languageDropdown,
						 _microphoneDevicesDropdown;

		private InputField _contextPhrasesInputField,
						   _operationIdInputField;

		private Image _voiceLevelImage;

		private void Start()
		{
			// Init Record Timer
			currRecordTimer = maxWaitRecordTimer;

			_speechRecognition = GCSpeechRecognition.Instance;
			_speechRecognition.RecognizeSuccessEvent += RecognizeSuccessEventHandler;
			_speechRecognition.RecognizeFailedEvent += RecognizeFailedEventHandler;
			_speechRecognition.LongRunningRecognizeSuccessEvent += LongRunningRecognizeSuccessEventHandler;
			_speechRecognition.LongRunningRecognizeFailedEvent += LongRunningRecognizeFailedEventHandler;
			_speechRecognition.GetOperationSuccessEvent += GetOperationSuccessEventHandler;
			_speechRecognition.GetOperationFailedEvent += GetOperationFailedEventHandler;
			_speechRecognition.ListOperationsSuccessEvent += ListOperationsSuccessEventHandler;
			_speechRecognition.ListOperationsFailedEvent += ListOperationsFailedEventHandler;

			_speechRecognition.FinishedRecordEvent += FinishedRecordEventHandler;
			_speechRecognition.StartedRecordEvent += StartedRecordEventHandler;
			_speechRecognition.RecordFailedEvent += RecordFailedEventHandler;

			_speechRecognition.BeginTalkigEvent += BeginTalkigEventHandler;
			_speechRecognition.EndTalkigEvent += EndTalkigEventHandler;

			_startRecordButton = uIHandler.startRecBtn;
			_stopRecordButton = uIHandler.stopRecBtn;
			// _getOperationButton = transform.Find("Canvas/Button_GetOperation").GetComponent<Button>();
			// _getListOperationsButton = transform.Find("Canvas/Button_GetListOperations").GetComponent<Button>();
			// _detectThresholdButton = transform.Find("Canvas/Button_DetectThreshold").GetComponent<Button>();
			// _cancelAllRequestsButton = transform.Find("Canvas/Button_CancelAllRequests").GetComponent<Button>();
			// _recognizeButton = transform.Find("Canvas/Button_Recognize").GetComponent<Button>();
			_refreshMicrophonesButton = uIHandler.refreshMicBtn;

			// _speechRecognitionState = transform.Find("Canvas/Image_RecordState").GetComponent<Image>();
			_resultText = uIHandler.resultTxt;

			// _voiceDetectionToggle = transform.Find("Canvas/Toggle_DetectVoice").GetComponent<Toggle>();
			// _recognizeDirectlyToggle = transform.Find("Canvas/Toggle_RecognizeDirectly").GetComponent<Toggle>();
			// _longRunningRecognizeToggle = transform.Find("Canvas/Toggle_LongRunningRecognize").GetComponent<Toggle>();

			// _languageDropdown = transform.Find("Canvas/Dropdown_Language").GetComponent<Dropdown>();
			_microphoneDevicesDropdown = uIHandler.micDeviceDropdown;

			// _contextPhrasesInputField = transform.Find("Canvas/InputField_SpeechContext").GetComponent<InputField>();
			// _operationIdInputField = transform.Find("Canvas/InputField_Operation").GetComponent<InputField>();

			_voiceLevelImage = uIHandler.voiceLvImg;

			_startRecordButton.onClick.AddListener(StartRecordButtonOnClickHandler);
			_stopRecordButton.onClick.AddListener(StopRecordButtonOnClickHandler);
			// _getOperationButton.onClick.AddListener(GetOperationButtonOnClickHandler);
			// _getListOperationsButton.onClick.AddListener(GetListOperationsButtonOnClickHandler);
			// _detectThresholdButton.onClick.AddListener(DetectThresholdButtonOnClickHandler);
			// _cancelAllRequestsButton.onClick.AddListener(CancelAllRequetsButtonOnClickHandler);
			// _recognizeButton.onClick.AddListener(RecognizeButtonOnClickHandler);
			_refreshMicrophonesButton.onClick.AddListener(RefreshMicsButtonOnClickHandler);

			_microphoneDevicesDropdown.onValueChanged.AddListener(MicrophoneDevicesDropdownOnValueChangedEventHandler);

			// _startRecordButton.interactable = true;
			// _stopRecordButton.interactable = false;
			// _speechRecognitionState.color = Color.yellow;

			// _languageDropdown.ClearOptions();

			// for (int i = 0; i < Enum.GetNames(typeof(Enumerators.LanguageCode)).Length; i++)
			// {
			// 	_languageDropdown.options.Add(new Dropdown.OptionData(((Enumerators.LanguageCode)i).Parse()));
			// }

			// _languageDropdown.value = _languageDropdown.options.IndexOf(_languageDropdown.options.Find(x => x.text == Enumerators.LanguageCode.en_GB.Parse()));

			RefreshMicsButtonOnClickHandler();
		}

		private void OnDestroy()
		{
			_speechRecognition.RecognizeSuccessEvent -= RecognizeSuccessEventHandler;
			_speechRecognition.RecognizeFailedEvent -= RecognizeFailedEventHandler;
			_speechRecognition.LongRunningRecognizeSuccessEvent -= LongRunningRecognizeSuccessEventHandler;
			_speechRecognition.LongRunningRecognizeFailedEvent -= LongRunningRecognizeFailedEventHandler;
			_speechRecognition.GetOperationSuccessEvent -= GetOperationSuccessEventHandler;
			_speechRecognition.GetOperationFailedEvent -= GetOperationFailedEventHandler;
			_speechRecognition.ListOperationsSuccessEvent -= ListOperationsSuccessEventHandler;
			_speechRecognition.ListOperationsFailedEvent -= ListOperationsFailedEventHandler;

			_speechRecognition.FinishedRecordEvent -= FinishedRecordEventHandler;
			_speechRecognition.StartedRecordEvent -= StartedRecordEventHandler;
			_speechRecognition.RecordFailedEvent -= RecordFailedEventHandler;

			_speechRecognition.EndTalkigEvent -= EndTalkigEventHandler;
		}

		private void Update()
		{
			// No Voice Timer
			if(_speechRecognition.IsRecording){
					currRecordTimer -= Time.deltaTime;

				if(_speechRecognition.GetLastFrame() >= 0.01 && _speechRecognition.GetLastFrame() < 0.2f){
					currRecordTimer = maxWaitRecordTimer;
					// Debug.Log("IF: " + _speechRecognition.GetLastFrame());
					
				}else{
					// If timer is out, Stop Record
					if(currRecordTimer <= 0f){
						currRecordTimer = 0f;
						Debug.Log("Timer Ran out, Stopping Recording");
						StopRecordButtonOnClickHandler();
					}
					// Debug.Log("ELSE: " + _speechRecognition.GetLastFrame());

				}
			}else{
				currRecordTimer = maxWaitRecordTimer;
			}

			if(_speechRecognition.IsRecording)
			{
				if (_speechRecognition.GetMaxFrame() > 0)
				{
					float max = (float)_speechRecognition.configs[_speechRecognition.currentConfigIndex].voiceDetectionThreshold;
					float current = _speechRecognition.GetLastFrame() / max;

					if(current >= 1f)
					{
						_voiceLevelImage.fillAmount = Mathf.Lerp(_voiceLevelImage.fillAmount, Mathf.Clamp(current / 2f, 0, 1f), 30 * Time.deltaTime);
					}
					else
					{
						_voiceLevelImage.fillAmount = Mathf.Lerp(_voiceLevelImage.fillAmount, Mathf.Clamp(current / 2f, 0, 0.5f), 30 * Time.deltaTime);
					}

					// _voiceLevelImage.color = current >= 1f ? Color.green : Color.red;
				}
			}
			else
			{
				_voiceLevelImage.fillAmount = 0f;
			}
		}

		private void RefreshMicsButtonOnClickHandler()
		{
			_speechRecognition.RequestMicrophonePermission(null);

			_microphoneDevicesDropdown.ClearOptions();
			_microphoneDevicesDropdown.AddOptions(_speechRecognition.GetMicrophoneDevices().ToList());

			MicrophoneDevicesDropdownOnValueChangedEventHandler(0);
        }

		private void MicrophoneDevicesDropdownOnValueChangedEventHandler(int value)
		{
			if (!_speechRecognition.HasConnectedMicrophoneDevices())
				return;
			_speechRecognition.SetMicrophoneDevice(_speechRecognition.GetMicrophoneDevices()[value]);
		}

		public void StartRecordButtonOnClickHandler()
		{
			// _startRecordButton.interactable = false;
			// _stopRecordButton.interactable = true;
			_startRecordButton.gameObject.SetActive(false);
			_stopRecordButton.gameObject.SetActive(true);
			// _detectThresholdButton.interactable = false;
			_resultText.text = string.Empty;

			_speechRecognition.StartRecord(false); // was _voiceDetectionToggle.isOn
		}

		public void StopRecordButtonOnClickHandler()
		{
			// _stopRecordButton.interactable = false;
			// _startRecordButton.interactable = true;
			_startRecordButton.gameObject.SetActive(true);
			_stopRecordButton.gameObject.SetActive(false);
			// _detectThresholdButton.interactable = true;

			_speechRecognition.StopRecord();
		}

		private void GetOperationButtonOnClickHandler()
		{
			if(string.IsNullOrEmpty(_operationIdInputField.text))
			{
				_resultText.text = "<color=red>Operatinon name is empty</color>";
				return;
			}

			_speechRecognition.GetOperation(_operationIdInputField.text);
		}

		private void GetListOperationsButtonOnClickHandler()
		{
			// some parameters could be seted
			_speechRecognition.GetListOperations();
		}

		private void DetectThresholdButtonOnClickHandler()
		{
			_speechRecognition.DetectThreshold();
		}

		private void CancelAllRequetsButtonOnClickHandler()
		{
			_speechRecognition.CancelAllRequests();
		}

		private void RecognizeButtonOnClickHandler()
		{
			if (_speechRecognition.LastRecordedClip == null)
			{
				_resultText.text = "<color=red>No Record found</color>";
				return;
			}

			FinishedRecordEventHandler(_speechRecognition.LastRecordedClip, _speechRecognition.LastRecordedRaw);
		}

		private void StartedRecordEventHandler()
		{
			// _speechRecognitionState.color = Color.red;
		}

		private void RecordFailedEventHandler()
		{
			// _speechRecognitionState.color = Color.yellow;
			_resultText.text = "<color=red>Start record Failed. Please check microphone device and try again.</color>";

			_stopRecordButton.interactable = false;
			_startRecordButton.interactable = true;
		}

		private void BeginTalkigEventHandler()
		{
			_resultText.text = "<color=blue>Talk Began.</color>";
		}

		private void EndTalkigEventHandler(AudioClip clip, float[] raw)
		{
			_resultText.text += "\n<color=blue>Talk Ended.</color>";

			FinishedRecordEventHandler(clip, raw);
		}

		private void FinishedRecordEventHandler(AudioClip clip, float[] raw)
		{
			// if (!_voiceDetectionToggle.isOn && _startRecordButton.interactable)
			// {
			// 	_speechRecognitionState.color = Color.yellow;
			// }

			if (clip == null) // || !_recognizeDirectlyToggle.isOn
				return;

			RecognitionConfig config = RecognitionConfig.GetDefault();
			config.languageCode = Enumerators.LanguageCode.id_ID.Parse(); // ((Enumerators.LanguageCode)_languageDropdown.value).Parse()
			config.speechContexts = new SpeechContext[]
			{
				new SpeechContext()
				{
					phrases = null //_contextPhrasesInputField.text.Replace(" ", string.Empty).Split(',')
				}
			};
			config.audioChannelCount = clip.channels;
			// configure other parameters of the config if need

			GeneralRecognitionRequest recognitionRequest;

			// if (_longRunningRecognizeToggle.isOn)
			// {
            //     recognitionRequest = new LongRunningRecognitionRequest();
            // }
            // else
            // {
            //     recognitionRequest = new GeneralRecognitionRequest();
            // }
			recognitionRequest = new GeneralRecognitionRequest();

			recognitionRequest.audio = new RecognitionAudioContent() // for base64 data
            {
				content = raw.ToBase64(channels: clip.channels)
			};
			//recognitionRequest.audio = new RecognitionAudioUri() // for Google Cloud Storage object
			//{
			//	uri = "gs://bucketName/object_name"
			//};
			recognitionRequest.config = config;

			// if (_longRunningRecognizeToggle.isOn)
			// {
			// 	_speechRecognition.LongRunningRecognize(recognitionRequest);
			// }
			// else
			// {
			// 	_speechRecognition.Recognize(recognitionRequest);
			// }
			_speechRecognition.Recognize(recognitionRequest);
		}

		private void GetOperationFailedEventHandler(string error)
		{
			_resultText.text = "Get Operation Failed: " + error;
		}

		private void ListOperationsFailedEventHandler(string error)
		{
			_resultText.text = "List Operations Failed: " + error;
		}

		private void RecognizeFailedEventHandler(string error)
        {
            _resultText.text = "Recognize Failed: " + error;
        }

		private void LongRunningRecognizeFailedEventHandler(string error)
		{
			_resultText.text = "Long Running Recognize Failed: " + error;
		}

		private void ListOperationsSuccessEventHandler(ListOperationsResponse operationsResponse)
		{
			_resultText.text = "List Operations Success.\n";

			if (operationsResponse.operations != null)
			{
				_resultText.text += "Operations:\n";

				foreach (var item in operationsResponse.operations)
				{
					_resultText.text += "name: " + item.name + ";\n";
                }
			}
		}

		private void GetOperationSuccessEventHandler(Operation operation)
		{
			_resultText.text = "Get Operation Success.\n";
			_resultText.text += "name: " + operation.name + "; done: " + operation.done;

            if (operation.done && (operation.error == null || string.IsNullOrEmpty(operation.error.message)))
			{
				InsertRecognitionResponseInfo(operation.response);
			}		
		}

		private void RecognizeSuccessEventHandler(RecognitionResponse recognitionResponse)
        {
			_resultText.text = "Recognize Success.";
			InsertRecognitionResponseInfo(recognitionResponse);
        }

        private void LongRunningRecognizeSuccessEventHandler(Operation operation)
        {
			if (operation.error != null && !string.IsNullOrEmpty(operation.error.message))
			{
                _resultText.text = "Long Running Recognize Failed: " + operation.error.message + "; operation: " + operation.name;
                return;
			}

			_resultText.text = "Long Running Recognize Success.\n Operation name: " + operation.name;

			if (operation.done)
			{
				if (operation.response != null && operation.response.results.Length > 0)
				{
					_resultText.text = "Long Running Recognize Success.";
					_resultText.text += "\n" + operation.response.results[0].alternatives[0].transcript;

					string other = "\nDetected alternatives:\n";

					foreach (var result in operation.response.results)
					{
						foreach (var alternative in result.alternatives)
						{
							if (operation.response.results[0].alternatives[0] != alternative)
							{
								other += alternative.transcript + ", ";
							}
						}
					}

					_resultText.text += other;
				}
			}
        }
		
		// Hasil Speech Recognition msk sini
		private void InsertRecognitionResponseInfo(RecognitionResponse recognitionResponse)
		{
			if (recognitionResponse == null || recognitionResponse.results.Length == 0)
			{
				_resultText.text += "\nWords not detected.";
				return;
			}

			_resultText.text += "\n" + recognitionResponse.results[0].alternatives[0].transcript;
			rawVoiceInput = recognitionResponse.results[0].alternatives[0].transcript;
			speechHandler.SetInputFldText(rawVoiceInput);
			// Debug.Log("Detected: " + recognitionResponse.results[0].alternatives[0].transcript);

			var words = recognitionResponse.results[0].alternatives[0].words;

			if (words != null)
			{
				string times = string.Empty;

				foreach (var item in recognitionResponse.results[0].alternatives[0].words)
				{
					times += "<color=green>" + item.word + "</color> -  start: " + item.startTime + "; end: " + item.endTime + "\n";
				}

				_resultText.text += "\n" + times;
			}

			string other = "\nDetected alternatives: ";

			foreach (var result in recognitionResponse.results)
			{
				foreach (var alternative in result.alternatives)
				{
					if (recognitionResponse.results[0].alternatives[0] != alternative)
					{
						other += alternative.transcript + ", ";
					}
				}
			}

			_resultText.text += other;

		}
    }
}