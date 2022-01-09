using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ViveSR.anipal.Eye;

public class GazeTimeCalc : MonoBehaviour
{
    public Text TimeDispText;
    public GameObject Range;
    public GameObject Gaze;
    public double gazeTime;

    public System.TimeSpan ts;
    private System.DateTime stopTimePoint;

    private int stopFrame = 0;

    private Vector3 stopPoint2Count;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var GazeRay = Gaze.GetComponent<SRanipal_GazeRaySample>();
        var point = new Vector3((float)GazeRay.inter_x, (float)GazeRay.inter_y, (float)GazeRay.inter_z);
        var range = Range.GetComponent<GazeRangeShow>();

        // If Not In Recording
        if (stopFrame == 0)
        {
            //Debug.Log("Start Recording Gaze Time!!!");
            stopFrame = Time.frameCount;
            stopPoint2Count = point;
            stopTimePoint = System.DateTime.Now;
        }
        
        // Point Not in Range, Break Recording
        else if (!range.IfPointInRange(stopPoint2Count, new Vector2(range.x_radius, range.y_radius), point))
        {
            //ts = System.DateTime.Now.Subtract(stopTimePoint);          
            //gazeTime = Convert.ToDouble(ts.Seconds.ToString() + "." + ts.Milliseconds.ToString());
            //Debug.Log("Gaze Time Duration: " + gaze_time);
            gazeTime = 0;
            TimeDispText.text = "Gaze Stopping Time: " + gazeTime + "s";
            stopFrame = 0;
        }

        // Point In Range, Continue Recording
        else
        {
            ts = System.DateTime.Now.Subtract(stopTimePoint);
            gazeTime = Convert.ToDouble(ts.Seconds.ToString() + "." + ts.Milliseconds.ToString());
            TimeDispText.text = "Gaze Stopping Time: " + gazeTime + "s";
            //Debug.Log("Continue Recording Gaze Time: " + gaze_time);
        }
    }
}
