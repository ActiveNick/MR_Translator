using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Runtime.Serialization;
using System.Xml;
using UnityEngine;
using UnityEngine.Networking;

public class Translator : MonoBehaviour {

    public static Translator instance;

    string translationTokenEndpoint = "https://api.cognitive.microsoft.com/sts/v1.0/issueToken";
    string translationTextEndpoint = "https://api.microsofttranslator.com/v2/http.svc/Translate?";

    private const string ocpApimSubscriptionKeyHeader = "Ocp-Apim-Subscription-Key";

    //Sobstitute the value of authorizationKey with your own Key
    private const string authorizationKey = "30661fea592d4b66897b7e88e2f769ed";

    private string authorizationToken;

    // languages set below are:
    // English
    // French
    // Italian
    // Japanese
    // Korean
    public enum Languages { en, fr, it, ja, ko };
    public Languages from = Languages.en;
    public Languages to = Languages.it;

    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start()
    {
        StartCoroutine("GetTokenCoroutine", authorizationKey);
    }

    // Update is called once per frame
    void Update () {
		
	}

    /// <summary>
    /// Request a Token from Azure Translation Service by providing the access key.
    /// Debugging result is delivered to the Results class.
    /// </summary>
    private IEnumerator GetTokenCoroutine(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            throw new InvalidOperationException("Authorization key not set.");
        }

        WWWForm form = new WWWForm();

        using (UnityWebRequest www = UnityWebRequest.Post(translationTokenEndpoint, form))
        {
            www.SetRequestHeader("Ocp-Apim-Subscription-Key", key);
            www.downloadHandler = new DownloadHandlerBuffer();

            yield return www.SendWebRequest();

            authorizationToken = www.downloadHandler.text;

            if (www.isNetworkError || www.isHttpError)
            {
                Results.instance.azureResponseText.text = www.error;
            }

            long responseCode = www.responseCode;
            Results.instance.SetAzureResponse(responseCode.ToString());
        }

        MicrophoneManager.instance.StartCapturingAudio();
        StopCoroutine("GetTokenCoroutine");
        yield return null;
    }

    /// <summary>
    /// Request a translation from Azure Translation Service by providing a string. 
    /// Debugging result is delivered to the Results class.
    /// </summary>
    public IEnumerator TranslateWithUnityNetworking(string text)
    {
        WWWForm form = new WWWForm();
        string result;
        string queryString;

        queryString = string.Concat("text=", Uri.EscapeDataString(text), "&from=", from, "&to=", to);

        using (UnityWebRequest www = UnityWebRequest.Get(translationTextEndpoint + queryString))
        {
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Authorization", "Bearer " + authorizationToken);
            www.SetRequestHeader("Accept", "application/xml");

            yield return www.SendWebRequest();

            string s = www.downloadHandler.text;

            DataContractSerializer serializer;
            serializer = new DataContractSerializer(typeof(string));

            using (Stream stream = GenerateStreamFromString(s))
            {
                Results.instance.SetTranslatedResult((string)serializer.ReadObject(stream));
            }

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }


            StopCoroutine("TranslateWithUnityNetworking");

        }
    }

    public static Stream GenerateStreamFromString(string s)
    {
        MemoryStream stream = new MemoryStream();
        StreamWriter writer = new StreamWriter(stream);
        writer.Write(s);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }

}
