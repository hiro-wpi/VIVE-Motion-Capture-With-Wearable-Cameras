using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Windows.Speech;
using System;

public class VoiceSwitch : MonoBehaviour
{
    public WebCamStream webCams;
    public RealsenseStream realsenseCams;

    private KeywordRecognizer kr;
    private Dictionary<string, Action> actions = new Dictionary<string, Action>();

    void Start()
    {
        // Link keyword with functions
        actions.Add("head", Switch2HeadCam);
        actions.Add("clavicle", Switch2ClavicleCam);
        actions.Add("left", Switch2LeftCam);
        actions.Add("right", Switch2RightCam);
        actions.Add("world", Switch2WorldCam);

        kr = new KeywordRecognizer(actions.Keys.ToArray());
        kr.OnPhraseRecognized += RecognizedSpeech;
        kr.Start();
    }

    private void RecognizedSpeech(PhraseRecognizedEventArgs speech)
    {
        //Debug.Log(speech.text);
        actions[speech.text].Invoke();
    }

    // Update is called once per frame
    void Update()
    {
    }
    
    // To Head Camera
    private void Switch2HeadCam()
    {
        Debug.Log("Switching to Head Camera");
        LoadCamera(0);
    }

    // To Left-hand Camera
    private void Switch2LeftCam()
    {
        Debug.Log("Switching to Left-hand Camera");
        LoadCamera(1);
    }

    // To Right-hand Camera
    private void Switch2RightCam()
    {
        Debug.Log("Switching to Right-hand Camera");
        LoadCamera(2);
    }

    // To Clavicle Camera
    private void Switch2ClavicleCam()
    {
        Debug.Log("Switching to Clavicle Camera");
        LoadCamera(3);
    }

    // To World Camera
    private void Switch2WorldCam()
    {
        Debug.Log("Switching to World Camera");
        LoadCamera(3);
    }

    private void LoadCamera(int index)
    {
        if(realsenseCams == null)
        {
            webCams.LoadCamera(index);
        }
        else
        {
            realsenseCams.LoadCamera(index);
        }
    }
}


