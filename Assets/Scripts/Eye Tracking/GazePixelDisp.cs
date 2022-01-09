using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ViveSR.anipal.Eye;

public class GazePixelDisp : MonoBehaviour
{
    public GameObject GazeInterPoint;
    public Text PixelText;

    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        var gaze_comp = GazeInterPoint.GetComponent<SRanipal_GazeRaySample>();
        var point = new Vector2((float)gaze_comp.Pixel_X, (float)gaze_comp.Pixel_Y);
        PixelText.text = "Pixel Coordinates: (" + point.x.ToString("0.00") + ", " + point.y.ToString("0.00") + ")";
    }
}
