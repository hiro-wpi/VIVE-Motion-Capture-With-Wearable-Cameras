using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RealsenseStream : MonoBehaviour
{
    public GameObject[] cameraCanvases;
    public int currentIndex;

    void Start()
    {
        currentIndex = 0;
        LoadCamera(0);
    }

    void Update()
    {
        if (cameraCanvases.Length == 0)
            return;
        
        // Increment index
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            currentIndex = (currentIndex+1) % cameraCanvases.Length;
            LoadCamera(currentIndex);
        }
    }

    public void LoadCamera(int index)
    {
        if (index < cameraCanvases.Length)
        {
            // Enable only the index camera
            // and disable the others
            for (int i = 0; i < cameraCanvases.Length; i++)
            {
                if (i == index)
                {
                    cameraCanvases[i].SetActive(true);
                }
                else
                {
                    cameraCanvases[i].SetActive(false);
                }
            }
        }
    }
}
