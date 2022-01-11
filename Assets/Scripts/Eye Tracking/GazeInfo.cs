using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ViveSR.anipal.Eye;

public class GazeInfo : MonoBehaviour
{
    public SRanipal_GazeRaySample gazeRay;
    public Text pixelText;
    public Text durationText;
    public LineRenderer lineRenderer;

    private float range;
    private float distance;
    private int segments;
    private float deltaAngle;

    private Vector2 currPoint;
    private Vector2 prevPoint;
    private System.DateTime prevTime;
    private System.TimeSpan timeSpan;

    public float pixelX;
    public float pixelY;
    public float gazeStayTime;

    private bool dispGaze;
    private bool dispText;

    void Start()
    {
        // Initialization
        currPoint = new Vector2(0.0f, 0.0f);
        prevPoint = new Vector2(0.0f, 0.0f);
        prevTime = System.DateTime.Now;
        timeSpan = System.DateTime.Now.Subtract(prevTime);

        // Gaze info parameter
        dispGaze = true;
        dispText = true;
        // circle
        range = 3.0f;
        distance = 99.0f;
        segments = 18;
        lineRenderer.positionCount = segments + 2;
        deltaAngle = 2*Mathf.PI / segments;
        // hide ray
        gazeRay.GazeRayRenderer.enabled = false;
    }

    void Update()
    {
        UpdateGazeInfo();
        DisplayInfo();
    }

    private void UpdateGazeInfo()
    {
        currPoint = new Vector2((float)gazeRay.inter_x, (float)gazeRay.inter_y);

        // Point in Range - calculate timespan
        if (PointInRange(prevPoint, currPoint, distance))
        {
            timeSpan = System.DateTime.Now.Subtract(prevTime);
        }
        // If out of range - reset
        else
        {
            prevPoint = currPoint;
            prevTime = System.DateTime.Now;
        }
    }

    private bool PointInRange(Vector2 origin, Vector2 target, float range)
    {
        // Compute distance
        float x_square = (origin.x - target.x) * (origin.x - target.x);
        float y_square = (origin.y - target.y) * (origin.y - target.y);
        // Check distance
        float ans = (range * range) - (x_square + y_square);
        return (ans >= 0)? true : false;
    }

    private void DisplayInfo()
    {
        // Check if display is changed
        if (Input.GetKeyDown(KeyCode.G))
        {
            dispGaze = !dispGaze;
            lineRenderer.enabled = dispGaze;
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
            DrawCircle(currPoint, range);
        }

        if (dispText)
        {
            // display pixel
            pixelX = (float)gazeRay.Pixel_X;
            pixelY = (float)gazeRay.Pixel_Y;
            pixelText.text = "Pixel Coordinates: (" + pixelX.ToString("0.0") + ", " + 
                                                      pixelY.ToString("0.0") + ")";

            // display duration
            gazeStayTime = float.Parse(timeSpan.Seconds.ToString() + "." + 
                                       timeSpan.Milliseconds.ToString()); 
            durationText.text = "Gaze Stopping Time: " + gazeStayTime.ToString("0.0") + "s";
        }
    }

    private void DrawCircle(Vector2 point, float radius)
    {
        float x;
        float y;

        float angle = 0.0f;
        for (int i = 0; i < (segments + 2); i++)
        {
            x = Mathf.Cos(angle) * range + point.x;
            y = Mathf.Sin(angle) * range + point.y;
            lineRenderer.SetPosition(i, new Vector3(x, y, distance));

            angle += deltaAngle;
        }
    }
}
