using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ViveSR.anipal.Eye;

public class GazeInfoDisp : MonoBehaviour
{
    public SRanipal_GazeRaySample gazeRay;
    public Text pixelText;
    public Text durationText;

    private Vector3 prevPoint;
    private System.DateTime prevTime;
    private System.TimeSpan timeSpan;
    
    public GameObject Range;

    private bool dispGaze;
    private bool dispText;

    void Start()
    {
        prevPoint = new Vector2(0.0f, 0.0f);
        prevTime = System.DateTime.Now;
        timeSpan = System.DateTime.Now.Subtract(prevTime);
    }

    void Update()
    {
        UpdateGazeInfo();
        DisplayInfo();
    }

    private void UpdateGazeInfo()
    {
        var point = new Vector2((float)gazeRay.inter_x, (float)gazeRay.inter_y);
        var range = Range.GetComponent<GazeRangeShow>();

        // Point in Range - calculate timespan
        if (PointInRange(prevPoint, new Vector2(range.x_radius, range.y_radius), point))
        {
            timeSpan = System.DateTime.Now.Subtract(prevTime);
        }
        // If out of range - reset
        else
        {
            prevPoint = point;
            prevTime = System.DateTime.Now;
        }
    }

    private bool PointInRange(Vector2 origin, Vector2 range, Vector2 target)
    {
        // Compute distance
        float x_square = (origin.x - target.x) * (origin.x - target.x);
        float y_square = (origin.y - target.y) * (origin.y - target.y);
        // Check distance
        float ans = x_square / (range.x * range.x) + y_square / (range.y * range.y);
        return (ans <= 1)? true : false;
    }

    private void DisplayInfo()
    {
        // Check if display is changed
        if (Input.GetKeyDown(KeyCode.G))
        {
            dispGaze = !dispGaze;
            
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            dispText = !dispText;
            pixelText.gameObject.SetActive(dispText);
            durationText.gameObject.SetActive(dispText);
        }
            
        // Display
        if (dispGaze)
        {
            // display gaze
        }

        if (dispText)
        {
            // display pixel
            float x = (float)gazeRay.Pixel_X;
            float y = (float)gazeRay.Pixel_Y;
            pixelText.text = "Pixel Coordinates: (" + x.ToString("0.0") + ", " + y.ToString("0.0") + ")";

            // display duration
            float gazeTime = float.Parse(timeSpan.Seconds.ToString() + "." + timeSpan.Milliseconds.ToString()); 
            durationText.text = "Gaze Stopping Time: " + gazeTime.ToString("0.0") + "s";
        }
    }
}
