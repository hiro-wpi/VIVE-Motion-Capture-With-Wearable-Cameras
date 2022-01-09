using ViveSR.anipal.Eye;
using UnityEngine;
using System.Collections;

//[RequireComponent(typeof(LineRenderer))]
public class GazeRangeShow : MonoBehaviour
{
    [Range(0, 50)]
    public int segments = 50;
    /*[Range(0, 5)]
    public float xradius = 5;
    [Range(0, 5)]
    public float yradius = 5;*/
    public LineRenderer line;

    public GameObject GazeRay;
    public float x_radius = 3;
    public float y_radius = 2;

    private void Start()
    {
        line.startWidth = 0.3f;
        line.endWidth = 0.3f;
    }

    void LateUpdate()
    {
        var gaze = GazeRay.GetComponent<SRanipal_GazeRaySample>();
 
        line.positionCount = segments + 1;

        CreatePoints(new Vector2(x_radius, y_radius), new Vector3((float)gaze.inter_x, (float)gaze.inter_y, (float)gaze.inter_z));
    }

    // Generate Circle Around A Point With Given Range
    void CreatePoints(Vector2 range, Vector3 pt)
    {
        //var gb = GameObject.Find("Gaze Ray Sample").GetComponent<SRanipal_GazeRaySample>();
        
        float x;
        float y;

        float angle = 20f;

        for (int i = 0; i < (segments + 1); i++)
        {
            x = (float)(Mathf.Sin(Mathf.Deg2Rad * angle) * range.x + pt.x);
            y = (float)(Mathf.Cos(Mathf.Deg2Rad * angle) * range.y + pt.y);

            line.SetPosition(i, new Vector3(x, y, 99));

            angle += (360f / segments);
        }
    }


    // Judge Whether A Given Point is In Range of Original Point
    public bool IfPointInRange(Vector3 origin, Vector2 range, Vector3 target)
    {
        float x_square = (origin.x - target.x) * (origin.x - target.x);
        float y_square = (origin.y - target.y) * (origin.y - target.y);

        var ans = x_square / (range.x * range.x) + y_square / (range.y * range.y);
        if (ans < 1 || ans == 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}