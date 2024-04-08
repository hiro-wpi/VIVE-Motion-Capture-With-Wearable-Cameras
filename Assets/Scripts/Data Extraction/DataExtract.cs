using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // CSV writter
    private int fileNum;
    private string folderName;
    private float[] data;
    private TextWriter dataWriter;

    // Record Icon
    public GameObject startIcon;
    public GameObject stopIcon;

    void Start()
    {
        // Initialize file name
        fileNum = 1;
        folderName = System.DateTime.Now.ToString("MM-dd HH-mm-ss");

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
            // n * 
            // (tracker.x, tracker.y, tracker.z
            //  tracker.euler.x, tracker.euler.y, tracker.euler.z)

            int length = 1 + 3 + 6 * TrackerList.Length;
            data = new float[length];

            data[0] = Time.time;
            data[1] = gazeInfo.pixelX;
            data[2] = gazeInfo.pixelY;
            data[3] = gazeInfo.gazeStayTime;
            for (int i = 4; i < length; i += 6)
            {
                int i_tracker = (i-4) / 6;
                Transform tf = TrackerList[i_tracker].transform;
                data[i] = tf.position.x;
                data[i+1] = tf.position.y;
                data[i+2] = tf.position.z;
                data[i+3] = tf.rotation.eulerAngles.x;
                data[i+4] = tf.rotation.eulerAngles.y;
                data[i+5] = tf.rotation.eulerAngles.z;
            }

            dataWriter.WriteLine(ArrayToCSVLine<float>(data));
        }
    }

    void Update()
    {
        // Start or stop recording
        if (Input.GetKeyDown(KeyCode.S))
        {
            if(isRecording)
                StopRecording();
            else
                StartRecording();
        }
    }

    public void StartRecording()
    {
        if (!isRecording)
        {
            // Create writer to write csv files
            string parentFolder = Application.dataPath + "/Data/" + folderName + "/";
            if (!Directory.Exists(parentFolder))
                Directory.CreateDirectory(parentFolder); 
            string name = parentFolder + "/" + fileNum.ToString();
            dataWriter = new StreamWriter(name + ".csv", false);
            
            Debug.Log("Recording On");
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
            dataWriter.Close();
            fileNum ++;

            Debug.Log("Recording Off");
            // UI
            startIcon.SetActive(false);
            stopIcon.SetActive(true);
            // Flag
            isRecording = false;
        }
    }

    private string ArrayToCSVLine<T>(T[] array)
    {
        string line = "";
        // Add value to line
        foreach (T value in array)
        {
            if (value is float || value is int)
                line += string.Format("{0:0.000}", value) + ",";
            else if (value is string)
                line += value + ",";
        }
        // Remove "," in the end
        line.Remove(line.Length - 1);
        return line;
    }
}
