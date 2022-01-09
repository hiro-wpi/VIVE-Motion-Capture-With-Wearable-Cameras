using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WebCamStream : MonoBehaviour
{
    public RawImage rawImage;
    public bool usePredefinedCameras;
    public string[] deviceNames;
    public int currentIndex = 0;

    private WebCamDevice[] devices;
    private WebCamTexture webcamTexture;

    void Start()
    {
        // Render to texture
        webcamTexture = new WebCamTexture();
        rawImage.texture = webcamTexture;
        rawImage.material.mainTexture = webcamTexture;

        // Get all connected devices
        devices = WebCamTexture.devices;
        if (devices.Length == 0)
        {
            Debug.Log("No camera is connected");
        }

        // Predefined names
        /*
        deviceNames = new string[] 
        {
            "Intel(R) RealSense(TM) Depth Camera 435i RGB",
            "Intel(R) RealSense(TM) Depth Camera 435 with RGB Module RGB 1",
            "Intel(R) RealSense(TM) Depth Camera 435 with RGB Module RGB",
            "Intel(R) RealSense(TM) 435 with RGB Module RGB"
        };
        */
        // If no predefined names are given
        if (!usePredefinedCameras)
        {
            deviceNames = new string[devices.Length];
            for (int i = 0; i < devices.Length; ++i)
            {
                deviceNames[i] = devices[i].name;
            }
        }
        LoadCamera(0);
    }

    void Update()
    {
        if (devices.Length == 0)
            return;
        
        // Increment index
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            currentIndex = (currentIndex+1) % deviceNames.Length;
            LoadCamera(currentIndex);
        }
    }

    public void LoadCamera(int index)
    {
        if (index < deviceNames.Length)
        {
            currentIndex = index;
            // change texture
            webcamTexture.Stop();
            webcamTexture.deviceName = deviceNames[currentIndex];
            webcamTexture.Play();
        }
    }
}
