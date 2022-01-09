using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViveSR.anipal.Eye;

public class GazeRuntimeDebug : MonoBehaviour
{
    public GameObject GazeRayDisp;
    public GameObject GazeRangeDisp;
    public GameObject GazeTimeText;
    public GameObject GazePixelText;
    //var gb1 = GameObject.Find("Gaze Range Display").GetComponent<GazeRangeShow>();
    //var gb2 = GameObject.Find("Gaze Ray Sample").GetComponent<SRanipal_GazeRaySample>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            GazeTrajectoryDebugger(GazeRayDisp, GazeRangeDisp);
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            GazeTimeDebugger(GazeTimeText);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            GazePixelDebugger(GazePixelText);
        }

    }


    void GazeTimeDebugger(GameObject gzTime)
    {
        if (gzTime.activeSelf)
        {
            gzTime.SetActive(false);
        }
        else
        {
            gzTime.SetActive(true);
        }
    }

    void GazePixelDebugger(GameObject gzPixel)
    {
        if (gzPixel.activeSelf)
        {
            gzPixel.SetActive(false);
        }
        else
        {
            gzPixel.SetActive(true);
        }
    }

    void GazeTrajectoryDebugger(GameObject gzRay, GameObject gzRange)
    {
        var gb1 = gzRange.GetComponent<GazeRangeShow>();
        var gb2 = gzRay.GetComponent<SRanipal_GazeRaySample>();
        
        if (gb1.line.enabled && gb2.GazeRayRenderer.enabled)
        {
            Debug.Log("Keyboard G has been pressed, Debugging ...");
            gb1.line.enabled = false;
            gb2.GazeRayRenderer.enabled = false;
        }
        else if (!gb1.line.enabled && !gb2.GazeRayRenderer.enabled)
        {
            Debug.Log("Exiting Debugging Mode Now ...");
            gb1.line.enabled = true;
            gb2.GazeRayRenderer.enabled = true;
        }
    }

}
