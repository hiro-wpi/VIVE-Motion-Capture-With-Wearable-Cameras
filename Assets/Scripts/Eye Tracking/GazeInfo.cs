using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ViveSR.anipal.Eye;

public class GazeInfo : MonoBehaviour
{
    public SRanipal_GazeRaySample gazeRay;
    public Text[] pixelText;
    public Text[] durationText;
    public LineRenderer lineRenderer;

    private float range;
    //private float distance;
    private int segments;
    private float deltaAngle;

    private Vector2 currPoint;
    private Vector2 prevPoint;
    private System.DateTime prevTime;
    private System.TimeSpan timeSpan;

    public float pixelX;
    public float pixelY;
    public float gazeStayTime;

    public bool dispGaze;
    public bool dispText;

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
        //distance = 99.0f;
        segments = 18;
        lineRenderer.startWidth = 0.7f;
        lineRenderer.endWidth = 0.7f;
        lineRenderer.positionCount = segments + 2;
        
        deltaAngle = 2*Mathf.PI / segments;
        // hide ray
        //gazeRay.GazeRayRenderer.enabled = false;
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
        if (PointInRange(prevPoint, currPoint, range))
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
            for(int i = 0; i < pixelText.Length; i++)
            {
                pixelText[i].gameObject.SetActive(dispText);
                durationText[i].gameObject.SetActive(dispText);
            }
        }
        
        // Display
        if (dispGaze)
        {
            // display gaze
            //DrawCircle(currPoint, range);
            Vector3 p;
            p.x = (float)gazeRay.inter_x;
            p.y = (float)gazeRay.inter_y;
            p.z = (float)gazeRay.inter_z;

            DrawGazeRange(p, new Vector2(2, 2));
        }

        if (dispText)
        {
            // display pixel
            pixelX = (float)gazeRay.Pixel_X;
            pixelY = (float)gazeRay.Pixel_Y;
            for(int i = 0; i < pixelText.Length; i++)
            {
                pixelText[i].text = "Pixel Coordinates: (" + pixelX.ToString("0.0") + ", " +
                                                      pixelY.ToString("0.0") + ")";
            }
            // display duration
            gazeStayTime = float.Parse(timeSpan.Seconds.ToString() + "." + 
                                       timeSpan.Milliseconds.ToString()); 

            for(int i = 0; i < durationText.Length; i++)
            {
                durationText[i].text = "Gaze Stopping Time: " + gazeStayTime.ToString("0.0") + "s";
            }
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
            lineRenderer.SetPosition(i, new Vector3(x, y, radius));

            angle += deltaAngle;
        }
    }

    private void DrawGazeRange(Vector3 pt, Vector2 range)
    {
        float angle = 10f;
        for (int i = 0; i < (segments + 2); i++)
        {
            float x = (float)(Mathf.Sin(Mathf.Deg2Rad * angle) * range.x + pt.x);
            float y = (float)(Mathf.Cos(Mathf.Deg2Rad * angle) * range.y + pt.y);

            lineRenderer.SetPosition(i, new Vector3(x, y, 99));

            angle += (360f / segments);
        }
    }
}
