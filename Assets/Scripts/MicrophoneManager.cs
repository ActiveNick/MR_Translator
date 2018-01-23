using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class MicrophoneManager : MonoBehaviour {

    public static MicrophoneManager instance;
    int frequency = 44100;
    AudioSource audioSource;
    bool microphoneDetected;
    bool isCapturingAudio;
    DictationRecognizer dictationRecognizer;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (Microphone.devices.Length > 0)
        {
            Results.instance.SetMicrophoneStatus("Initialising...");
            audioSource = GetComponent<AudioSource>();
            microphoneDetected = true;
        }
        else
        {
            Results.instance.SetMicrophoneStatus("No Microphone detected");
        }
    }

    // Update is called once per frame
    void Update () {
		
	}

    /// <summary>
    /// This handler is called every time the Dictation detects a pause in the speech. 
    /// Debugging message is delivered to the Results class.
    /// </summary>
    private void DictationRecognizer_DictationResult(string text, ConfidenceLevel confidence)
    {
        Results.instance.SetDictationResult(text);
        StartCoroutine(Translator.instance.TranslateWithUnityNetworking(text));
    }


    /// <summary>
    /// Start microphone capture. Debugging message is delivered to the Results class.
    /// </summary>
    public void StartCapturingAudio()
    {
        if (microphoneDetected)
        {
            isCapturingAudio = true;
            audioSource.clip = Microphone.Start(null, true, 30, frequency);
            audioSource.loop = true;
            audioSource.Play();

            dictationRecognizer = new DictationRecognizer();
            dictationRecognizer.DictationResult += DictationRecognizer_DictationResult;
            dictationRecognizer.Start();

            Results.instance.SetMicrophoneStatus("Capturing...");
        }
    }

    /// <summary>
    /// Stop microphone capture. Debugging message is delivered to the Results class.
    /// </summary>
    public void StopCapturingAudio()
    {
        Results.instance.SetMicrophoneStatus("Mic sleeping");
        isCapturingAudio = false;
        Microphone.End(null);
        dictationRecognizer.DictationResult -= DictationRecognizer_DictationResult;
        dictationRecognizer.Dispose();
    }

}
