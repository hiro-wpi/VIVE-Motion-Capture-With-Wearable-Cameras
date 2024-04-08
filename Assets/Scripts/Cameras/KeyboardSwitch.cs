using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardSwitch : MonoBehaviour
{
    public WebCamStream webCams;
    public RealsenseStream realsenseCams;

    void Start()
    {
    }

    void Update()
    {
        // 0 - 5: Head, Left, Right, Clavicle and World cameras
        // Switch cameras
        if (Input.GetKeyDown(KeyCode.H))
        {
            Debug.Log("Switching to Head Camera");
            LoadCamera(0);
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("Switching to Left-hand Camera");
            LoadCamera(1);
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Switching to Right-hand Camera");
            LoadCamera(2);
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("Switching to Clavicle Camera");
            LoadCamera(3);
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("Switching to World Camera");
            LoadCamera(4);
        }
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
