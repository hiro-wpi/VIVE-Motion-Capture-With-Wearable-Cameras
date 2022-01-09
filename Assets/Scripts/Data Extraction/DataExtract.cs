using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExperimentDataRecord;
using OfficeOpenXml;
using System.IO;
using ViveSR.anipal.Eye;
using System;
//using HTC.UnityPlugin.Vive;
using Valve.VR;

public class DataExtract : MonoBehaviour
{
    // Trial Count For Sheet Save
    public int trialCount = 1;

    // RecordFlag
    public bool IsRecording = false;

    // Frame
    public float updateRate = 10;

    // Gaze
    public double gazeDurationThreshold;    // Threshold for gaze duration record
    public GameObject gazeRay;
    public GameObject gazeTime;
    private double lastGazeTime;
    public Vector2 lastPixel;

    // Tracker For Pose
    public GameObject[] TrackerList;

    // ExcelFile
    public string ExcelFilePath;
    public string ExcelSheetName;
    private FileInfo newFile;
    public ExcelPackage exlpackage;
    ExcelWorksheet worksheet;
    public int raw;
    public int col;

    // Screenshot
    public string ScreenshotDirPath;
    public string ScreenshotDemoName;
    public Camera cam;
    public Rect rect;

    // Time Recording
    public System.Diagnostics.Stopwatch stopwatch;

    // Record Icon
    public GameObject startIcon;
    public GameObject stopIcon;

    // Start is called before the first frame update
    void Start()
    {
        stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();

        lastGazeTime = 0;
        lastPixel.x = -1;
        lastPixel.y = -1;

        var strName = ExcelFilePath.Split('/');
        ScreenshotDemoName = strName[strName.Length - 1].Split('.')[0];

        DataRecordUtil.DeleteFile(ExcelFilePath);
        newFile = new FileInfo(Application.dataPath + "/" + ExcelFilePath);
        exlpackage = new ExcelPackage(newFile);
        InitialExcelSheet(exlpackage, ExcelFilePath, ExcelSheetName + trialCount.ToString(), out worksheet);
        raw = 2;
        col = 1;

        DataRecordUtil.DeleteDirectory(ScreenshotDirPath);
        
        // Call Function to collect Data every 10 times a second
        InvokeRepeating("UpdateData", 1f, 1 / updateRate);

    }

    // Collect Data
    void UpdateData()
    {
        if (IsRecording)
        {
            worksheet.Cells[raw, col++].Value = Time.time;
            worksheet.Cells[raw, col++].Value = lastPixel.x.ToString();
            worksheet.Cells[raw, col++].Value = lastPixel.y.ToString();
            worksheet.Cells[raw, col++].Value = lastGazeTime.ToString();

            for (int i = 0; i < TrackerList.Length; i++)
            {
                var tf = TrackerList[i].GetComponent<Transform>();
                worksheet.Cells[raw, col++].Value = tf.position.x;
                worksheet.Cells[raw, col++].Value = tf.position.y;
                worksheet.Cells[raw, col++].Value = tf.position.z;
                worksheet.Cells[raw, col++].Value = tf.rotation.eulerAngles.x;
                worksheet.Cells[raw, col++].Value = tf.rotation.eulerAngles.y;
                worksheet.Cells[raw, col++].Value = tf.rotation.eulerAngles.z;
            }
            raw++;
            col = 1;
        }
    }


    // Update is called once per frame
    void LateUpdate()
    {
        // Start or stop recording
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (IsRecording == true)
            {
                startIcon.SetActive(false);
                stopIcon.SetActive(true);
                Debug.Log("Recording Off...");
                Debug.Log("Saving Data To Excel File!");
                exlpackage.Save();
                Debug.Log("Data has been saved successfully!");
            }
            else
            {
                startIcon.SetActive(true);
                stopIcon.SetActive(false);
                Debug.Log("Recording On...");
            }
            IsRecording = !IsRecording;
        }

        // Save data to file
        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("Saving Data To Excel File!");
            exlpackage.Save();
            Debug.Log("Data has been saved successfully!");
        }

        // Respawn a new sheet for new trial in each specific task
        if (Input.GetKeyDown(KeyCode.N))
        {
            Debug.Log("Trial " + trialCount.ToString() + " has ended, saving the data to excel sheet Now...");
            exlpackage.Save();
            DataRecordUtil.TakeScreenshot(ScreenshotDirPath, ScreenshotDemoName, trialCount);
            trialCount++;
            Debug.Log("Trial " + trialCount.ToString() + " starts at another sheet");
            //DataRecordUtil.SheetRespawn(exlpackage, ExcelSheetName + trialCount.ToString(), out worksheet);
            InitialExcelSheet(exlpackage, ExcelFilePath, ExcelSheetName + trialCount.ToString(), out worksheet);
            //IsRecording = false;
            raw = 2;
            col = 1;
        }

        // Recording to another file
        if (Input.GetKeyDown(KeyCode.L))
        {
            trialCount = 1;
            DataRecordUtil.DeleteFile(ExcelFilePath);
            newFile = new FileInfo(Application.dataPath + "/" + ExcelFilePath);
            exlpackage = new ExcelPackage(newFile);
            InitialExcelSheet(exlpackage, ExcelFilePath, ExcelSheetName + trialCount.ToString(), out worksheet);
            raw = 2;
            col = 1;
            var strName = ExcelFilePath.Split('/');
            ScreenshotDemoName = strName[strName.Length - 1].Split('.')[0];
        }

        var gb = gazeTime.GetComponent<GazeTimeCalc>();
        var gb2 = gazeRay.GetComponent<SRanipal_GazeRaySample>();

        // Gaze Screenshot
        if (IsRecording)
        {
            //DataRecordUtil.CaptureCamera(cam, rect);

            /*Debug.Log("lastFrameGazeTime: " + lastGazeTime);
            Debug.Log("gazeTime: " + gb.gazeTime);
            Debug.Log("LastGazePixel: x:" + lastPixel.x + ", y:" + lastPixel.y);
            Debug.Log("This-time GazePixel: x:" + gb2.Pixel_X + ",y:" + gb2.Pixel_Y);*/

            if (lastGazeTime > gazeDurationThreshold && gb.gazeTime < gazeDurationThreshold)
            {
                // Take Screenshot
                /** (Here I don't know why I save this frame but it seems screenshot of last frame got recorded, which caters to my willingness exactly -_-, well just let it go) **/
                DataRecordUtil.TakeScreenshot(ScreenshotDirPath, ScreenshotDemoName, trialCount);
                Debug.Log("Screenshot of the display is taken successfully!");
            }
        }
        // Refresh
        lastGazeTime = gb.gazeTime;
        lastPixel.x = (float)gb2.Pixel_X;
        lastPixel.y = (float)gb2.Pixel_Y;

    }


    // Initialize Excel File Sheet
    public void InitialExcelSheet(ExcelPackage exlpkg, string excelFilePath, string sheetName, out ExcelWorksheet wksheet)
    {
        // Define Excel FilePath
        string path = Application.dataPath + "/" + excelFilePath;

        // If directory name doesn't exist, create one
        string dirPath = System.IO.Path.GetDirectoryName(path);
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }

        FileInfo newFile = new FileInfo(path);

        // Open Excel File

        int cnt = 1;
        //ExcelWorksheet worksheet = null;
        DataRecordUtil.SheetRespawn(exlpkg, sheetName, out wksheet);
        //worksheet.Cells[1, cnt++].Value = "Frames";
        wksheet.Cells[1, cnt++].Value = "TimeStamp(s)";
        wksheet.Cells[1, cnt++].Value = "Gaze(X)";
        wksheet.Cells[1, cnt++].Value = "Gaze(Y)";
        wksheet.Cells[1, cnt++].Value = "Gaze Duration(s)";
            
        for(int i = 0; i < TrackerList.Length; i++)
        {
            var trackerName = TrackerList[i].name;
            wksheet.Cells[1, cnt++].Value = trackerName + "(X)";
            wksheet.Cells[1, cnt++].Value = trackerName + "(Y)";
            wksheet.Cells[1, cnt++].Value = trackerName + "(Z)";
            wksheet.Cells[1, cnt++].Value = trackerName + "(Raw)";
            wksheet.Cells[1, cnt++].Value = trackerName + "(Pitch)";
            wksheet.Cells[1, cnt++].Value = trackerName + "(Yaw)";
        }
        exlpkg.Save();
        Debug.Log(sheetName + " is initialied.");
    }

    /*
    private void RecordStart()
    {
        var gb = dataExtract.GetComponent<DataExtract>();
        gb.startIcon.SetActive(true);
        gb.stopIcon.SetActive(false);
        Debug.Log("Recording On...");
        gb.IsRecording = !gb.IsRecording;
    }

    private void RecordOff()
    {
        var gb = dataExtract.GetComponent<DataExtract>();
        gb.startIcon.SetActive(false);
        gb.stopIcon.SetActive(true);

        Debug.Log("Recording Off...");
        gb.IsRecording = !gb.IsRecording;
        Debug.Log("Saving Data To Excel File!");
        gb.exlpackage.Save();
        Debug.Log("Data has been saved successfully!");
    }
    */
}
