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
    // Record
    public bool isRecording = false;
    public float updateRate;
    
    // Gaze
    public GazeInfo gazeInfo;
    public double gazeDurationThreshold;    // Threshold for gaze duration record

    // Tracker For Pose
    public GameObject[] TrackerList;

    // ExcelFile
    private int fileNum;
    private int trial;
    private string excelFileName;
    private string excelSheetName;
    private ExcelPackage exlpackage;
    private ExcelWorksheet worksheet;
    private int raw;
    private int col;

    // Screenshot
    public Camera screenshotCam;
    private float lastGazeTime;

    // Record Icon
    public GameObject startIcon;
    public GameObject stopIcon;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize file info
        fileNum = 1;
        trial = 1;
        excelFileName = fileNum.ToString();
        excelSheetName = "trial";
        InitializeExcelSheet(excelFileName, excelSheetName + trial.ToString());

        // Screenshot
        lastGazeTime = 100.0f;

        // Call Function to update Data
        updateRate = 10;
        InvokeRepeating("UpdateData", 1f, 1/updateRate);
    }

    void UpdateData()
    {
        if (isRecording)
        { 
            // DATA
            // time, gaze.x, gaze.y, gaze.t
            // tracker.x, tracker.y, tracker.z
            // tracker.euler.x, tracker.euler.y, tracker.euler.z

            worksheet.Cells[raw, col++].Value = Time.time;
            worksheet.Cells[raw, col++].Value = gazeInfo.pixelX;
            worksheet.Cells[raw, col++].Value = gazeInfo.pixelY;
            worksheet.Cells[raw, col++].Value = gazeInfo.gazeStayTime;
            for (int i = 0; i < TrackerList.Length; i++)
            {
                var tf = TrackerList[i].transform;
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

    void Update()
    {
        // Start or stop recording
        if (Input.GetKeyDown(KeyCode.C))
        {
            if(isRecording)
                StopRecording();
            else
                StartRecording();
        }

        // Respawn a new sheet for new trial in each specific task
        if (Input.GetKeyDown(KeyCode.N))
        {
            StopRecording();
            trial++;
            InitializeExcelSheet(excelFileName, excelSheetName + " " + trial.ToString());
        }

        // Recording in a new file
        if (Input.GetKeyDown(KeyCode.F))
        {
            StopRecording();
            fileNum++;
            trial = 1;
            excelFileName = fileNum.ToString();
            InitializeExcelSheet(excelFileName, excelSheetName + " " + trial.ToString());
        }

        /*
        // Gaze Screenshot
        if (isRecording)
        {
            if (lastGazeTime > gazeDurationThreshold && 
                gazeInfo.gazeStayTime < gazeDurationThreshold)
            {
                // Take Screenshot
                // (Here I don't know why I save this frame but it seems screenshot of last frame 
                // got recorded, which caters to my willingness exactly -_-, well just let it go)
                DataRecordUtil.TakeScreenshot("Screenshot", excelFileName, trial);
                Debug.Log("Screenshot of the display is taken successfully!");
            }
        }
        */
    }

    public void StartRecording()
    {
        if (!isRecording)
        {
            Debug.Log("Recording On...");
            // UI
            startIcon.SetActive(true);
            stopIcon.SetActive(false);
            // Flag
            isRecording = true;
        }  
    }

    public void StopRecording()
    {
        if (isRecording)
        { 
            Debug.Log("Recording Off...");
            exlpackage.Save();
            Debug.Log("Data has been saved successfully!");
            // UI
            startIcon.SetActive(false);
            stopIcon.SetActive(true);
            // Flag
            isRecording = false;
        }
    }

    public void InitializeExcelSheet(string excelFileName, string sheetName)
    {
        // Define Excel FilePath
        string path = Application.dataPath + "/Data/" + excelFileName + ".xlsx";

        // If directory name doesn't exist, create one
        string dirPath = System.IO.Path.GetDirectoryName(path);
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
        FileInfo newFile = new FileInfo(path);
        exlpackage = new ExcelPackage(newFile);

        // Open Excel File
        DataRecordUtil.SheetRespawn(exlpackage, sheetName, out worksheet);

        // Titles for each column
        col = 1;
        worksheet.Cells[1, col++].Value = "TimeStamp(s)";
        worksheet.Cells[1, col++].Value = "Gaze(X)";
        worksheet.Cells[1, col++].Value = "Gaze(Y)";
        worksheet.Cells[1, col++].Value = "Gaze Duration(s)";
        for(int i = 0; i < TrackerList.Length; i++)
        {
            var trackerName = TrackerList[i].name;
            worksheet.Cells[1, col++].Value = trackerName + "(X)";
            worksheet.Cells[1, col++].Value = trackerName + "(Y)";
            worksheet.Cells[1, col++].Value = trackerName + "(Z)";
            worksheet.Cells[1, col++].Value = trackerName + "(Raw)";
            worksheet.Cells[1, col++].Value = trackerName + "(Pitch)";
            worksheet.Cells[1, col++].Value = trackerName + "(Yaw)";
        }
        
        raw = 2;
        col = 1;
        exlpackage.Save();
        Debug.Log("File: " + excelFileName + " - Sheet: " + sheetName + " is initialied.");
    }
}
