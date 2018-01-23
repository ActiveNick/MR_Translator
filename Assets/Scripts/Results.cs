using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Results : MonoBehaviour
{

    public static Results instance;

    public string azureResponseCode;
    public string translationResult;
    public string dictationResult;
    public string micStatus;

    public Text azureResponseText;
    public Text translatedResultText;
    public Text dictatedResultText;
    public Text microphoneStatusText;

    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// Stores the Azure response value in the static instance of Result class. 
    /// </summary>
    public void SetAzureResponse(string result)
    {
        azureResponseCode = result;
        azureResponseText.text = azureResponseCode;
    }

    /// <summary>
    /// Stores the translated result from dictation in the static instance of Result      class. 
    /// </summary>
    public void SetDictationResult(string result)
    {
        dictationResult = result;
        dictatedResultText.text = dictationResult;
    }

    /// <summary>
    /// Stores the translated result from Azure Service in the static instance of Result  class. 
    /// </summary>
    public void SetTranslatedResult(string result)
    {
        translationResult = result;
        translatedResultText.text = translationResult;
    }

    /// <summary>
    /// Stores the status of the Microphone in the static instance of Result class. 
    /// </summary>
    public void SetMicrophoneStatus(string result)
    {
        micStatus = result;
        microphoneStatusText.text = micStatus;
    }
}
