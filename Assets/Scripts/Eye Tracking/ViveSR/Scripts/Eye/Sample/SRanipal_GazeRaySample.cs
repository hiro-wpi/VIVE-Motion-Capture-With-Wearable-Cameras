﻿//========= Copyright 2018, HTC Corporation. All rights reserved. ===========
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using ViveSR.anipal.Eye;


namespace ViveSR
{
    namespace anipal
    {
        namespace Eye
        {
            public class SRanipal_GazeRaySample : MonoBehaviour
            {
                public int LengthOfRay = 100;
                public GameObject camera_source;

                public System.TimeSpan ts;
                private System.DateTime stopTimePoint;

                //private int stopFrame = 0;
                //private Vector3 stopPoint2Count;

                //private Transform planeTrans;

                public double inter_x, inter_y, inter_z;
                public double Pixel_X, Pixel_Y;

                public double gaze_range;

                [SerializeField] public LineRenderer GazeRayRenderer;
                [SerializeField] public RectTransform rect;

                private static EyeData eyeData = new EyeData();
                private bool eye_callback_registered = false;

                public double origin_x, origin_y, origin_z;
                public double direction_x, direction_y, direction_z;

                private void Start()
                {
                    //stopPoint2Count.z = 0;
                    GazeRayRenderer.SetWidth(0.02f, 0.02f);
                    if (!SRanipal_Eye_Framework.Instance.EnableEye)
                    {
                        enabled = false;
                        return;
                    }
                    Assert.IsNotNull(GazeRayRenderer);

                }

                /******** Calculate the Intersect Point of a line and a plane *********/
                Vector3 CalcIntersectPoint(LineRenderer linerenderer, Transform planeTrans)
                {
                    //Transform planeTrans = this.gameObject.transform;

                    //Debug.Log("The transform of mesh is:" + planeTrans.position);
                    Vector3 p1 = linerenderer.GetPosition(0);
                    Vector3 p2 = linerenderer.GetPosition(1);

                    //Debug.Log("P1 position is:" + p1);
                    //Debug.Log("P2 position is:" + p2);

                    Vector3 direction = p2 - p1;

                    //Debug.Log("Direction position is:" + direction);


                    //Vector3 n = plane.normal;
                    //planeTrans.forward
                    Vector3 p_0 = planeTrans.position;
                    //a normal (defining the orientation of the plane), should be negative if we are firing the ray from above
                    Vector3 n = -planeTrans.forward;
                    //Debug.Log("The normal of mesh is:" + planeTrans.forward);

                    //We are intrerested in calculating a point in this plane called p
                    //The vector between p and p0 and the normal is always perpendicular: (p - p_0) . n = 0

                    //A ray to point p can be defined as: l_0 + l * t = p, where:
                    //the origin of the ray

                    Vector3 l_0 = p1;
                    //l is the direction of the ray

                    Vector3 l = direction;
                    //t is the length of the ray, which we can get by combining the above equations:
                    //t = ((p_0 - l_0) . n) / (l . n)

                    //But there's a chance that the line doesn't intersect with the plane, and we can check this by first
                    //calculating the denominator and see if it's not small. 
                    //We are also checking that the denominator is positive or we are looking in the opposite direction
                    float denominator = Vector3.Dot(l, n);

                    float t = Vector3.Dot(p_0 - l_0, n) / denominator;

                    //Debug.Log("The t is:" + t);

                    //Where the ray intersects with a plane
                    Vector3 p = l_0 + l * t;
                    return p;
                }

                /*********** Assume p is on the same plane with rt, calc pixel coordinates to rt ***********/
                Vector2 GetPixelCoordinates(Vector3 p, RectTransform rt)
                {
                    Vector2 vt;
                    vt.x = -1;
                    vt.y = -1;

                    Vector3[] corners = new Vector3[4];
                    rt.GetWorldCorners(corners);
                    
                    Vector2 LeftBotPoint = new Vector2();
                    Vector2 RightUpPoint = new Vector2();

                    LeftBotPoint.x = corners[0].x;
                    LeftBotPoint.y = corners[0].y;
                    RightUpPoint.x = corners[0].x;
                    RightUpPoint.y = corners[0].y;

                    for(int i = 1; i < 4; i++)
                    {
                        if (corners[i].x < LeftBotPoint.x)
                        {
                            LeftBotPoint.x = corners[i].x;
                        }
                        if (corners[i].y < LeftBotPoint.y)
                        {
                            LeftBotPoint.y = corners[i].y;
                        }
                        if (corners[i].x > RightUpPoint.x)
                        {
                            RightUpPoint.x = corners[i].x;
                        }
                        if (corners[i].y > RightUpPoint.y)
                        {
                            RightUpPoint.y = corners[i].y;
                        }
                    }

                    double width = System.Math.Abs(RightUpPoint.x - LeftBotPoint.x);
                    double height = System.Math.Abs(RightUpPoint.y - LeftBotPoint.y);


                    //Order: ↙ ↖ ↗ ↘ clockwise beginning with left-down side

                    // Whether in the range of matrix
                    if (p.x < LeftBotPoint.x || p.x > RightUpPoint.x || p.y < LeftBotPoint.y || p.y > RightUpPoint.y)
                    {
                        //Debug.Log("Out of Range!!!");
                        return vt;
                    }
                    else
                    {
                        vt.x = (float)((p.x - LeftBotPoint.x) / width * rt.rect.width);
                        vt.y = (float)((p.y - LeftBotPoint.y) / height * rt.rect.height);
                        return vt;
                    }

                }
                private void Update()
                {

                    //Debug.Log("The current status is:" + SRanipal_Eye_Framework.Status);
                    //Debug.Log("The current frame is:" + Time.frameCount);

                    if (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING &&
                        SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.NOT_SUPPORT) return;

                    // Whether to turn on eye callback register
                    if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == true && eye_callback_registered == false)
                    {
                        SRanipal_Eye.WrapperRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye.CallbackBasic)EyeCallback));
                        eye_callback_registered = true;
                    }
                    else if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == false && eye_callback_registered == true)
                    {
                        SRanipal_Eye.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye.CallbackBasic)EyeCallback));
                        eye_callback_registered = false;
                    }

                    Vector3 GazeOriginCombinedLocal, GazeDirectionCombinedLocal;

                    if (eye_callback_registered)
                    {
                        if (SRanipal_Eye.GetGazeRay(GazeIndex.COMBINE, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal, eyeData)) { }
                        else if (SRanipal_Eye.GetGazeRay(GazeIndex.LEFT, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal, eyeData)) { }
                        else if (SRanipal_Eye.GetGazeRay(GazeIndex.RIGHT, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal, eyeData)) { }
                        else return;
                    }
                    else
                    {
                        if (SRanipal_Eye.GetGazeRay(GazeIndex.COMBINE, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal)) { }
                        else if (SRanipal_Eye.GetGazeRay(GazeIndex.LEFT, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal)) { }
                        else if (SRanipal_Eye.GetGazeRay(GazeIndex.RIGHT, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal)) { }
                        else return;
                    }

                    origin_x = GazeOriginCombinedLocal.x;
                    origin_y = GazeOriginCombinedLocal.y;
                    origin_z = GazeOriginCombinedLocal.z;

                    direction_x = GazeDirectionCombinedLocal.x;
                    direction_y = GazeDirectionCombinedLocal.y;
                    direction_z = GazeDirectionCombinedLocal.z;


                    //Vector3 GazeDirectionCombined = Camera.main.transform.TransformDirection(GazeDirectionCombinedLocal);
                    //GazeRayRenderer.SetPosition(0, Camera.main.transform.position - Camera.main.transform.up * 0.05f);
                    //GazeRayRenderer.SetPosition(1, Camera.main.transform.position + GazeDirectionCombined * LengthOfRay);

                    Vector3 GazeDirectionCombined = camera_source.transform.TransformDirection(GazeDirectionCombinedLocal);
                    GazeRayRenderer.SetPosition(0, camera_source.transform.position - camera_source.transform.up * 0.05f);
                    GazeRayRenderer.SetPosition(1, camera_source.transform.position + GazeDirectionCombined * LengthOfRay);

                    //var point = CalcIntersectPoint(GazeRayRenderer, GameObject.Find("RealSense Canvas").transform);

                    var point = CalcIntersectPoint(GazeRayRenderer, rect.transform);

                    inter_x = point.x;
                    inter_y = point.y;
                    inter_z = point.z;

                    //GazeRayRenderer.SetPosition(0, point);
                    //GazeRayRenderer.SetPosition(1, new Vector3(0, 0, 0));

                    var pixel = GetPixelCoordinates(point, rect);
                    
                    Pixel_X = pixel.x;
                    Pixel_Y = pixel.y;

                }
                private void Release()
                {
                    if (eye_callback_registered == true)
                    {
                        SRanipal_Eye.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye.CallbackBasic)EyeCallback));
                        eye_callback_registered = false;
                    }
                }
                private static void EyeCallback(ref EyeData eye_data)
                {
                    eyeData = eye_data;
                }
            }

        }
    }
    
}


