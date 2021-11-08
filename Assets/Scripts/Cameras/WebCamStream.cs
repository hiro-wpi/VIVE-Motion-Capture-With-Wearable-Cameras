using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WebCamStream : MonoBehaviour
{
    public RawImage rawImage;
    public bool usePredefinedCameras;
    public string[] deviceNames;

    private WebCamDevice[] devices;
    private WebCamTexture webcamTexture;
    private int currentIndex = 0;

    void Start()
    {
        // Predefined names
        deviceNames = new string[] 
        {
            "Intel(R) RealSense(TM) Depth Camera 435 with RGB Module RGB",
            "Intel(R) RealSense(TM) Depth Camera 435 with RGB Module RGB 1"
        };

        // Get all connected devices
        devices = WebCamTexture.devices;

        // Render to texture
        webcamTexture = new WebCamTexture();
        rawImage.texture = webcamTexture;
        rawImage.material.mainTexture = webcamTexture;

        // Initialization
        if (devices.Length > 0)
        {
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
    }

    void Update()
    {
        if (devices.Length == 0)
            return;
        
        // Switch cameras
        if (Input.GetKeyDown(KeyCode.Q))
        {
            LoadCamera(0);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            LoadCamera(1);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            LoadCamera(2);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            LoadCamera(3);
        }

        // Increment index
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            currentIndex = (currentIndex+1) % deviceNames.Length;
            LoadCamera(currentIndex);
        }
    }

    void LoadCamera(int index)
    {
        if (index < deviceNames.Length)
        {
            currentIndex = index;
            // change texture
            webcamTexture.Stop();
            webcamTexture.deviceName = deviceNames[currentIndex];
            webcamTexture.Play();

            Debug.Log("Current camera name: " + deviceNames[currentIndex]);
        }
    }
}
