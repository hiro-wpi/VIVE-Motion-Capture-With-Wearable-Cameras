using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackerReplay : MonoBehaviour
{
    // File names
    public string folderName;
    public string fileName;

    private int indexHead = 4;
    private int indexWaist = 10;
    private int indexLeftShoulder = 16;
    private int indexRightShoulder = 22;
    private int indexLeftHand = 28;
    private int indexRightHand = 34;

    // Trackers
    public GameObject head;
    public GameObject waist;
    public GameObject leftShoulder;
    public GameObject rightShoulder;
    public GameObject leftHand;
    public GameObject rightHand;

    // Replay
    StreamReader csvReader;
    private float[] values;
    private float timeOffset;
    private bool done;
    private bool playing;
    private float timer;


    void Start()
    {
        ReloadReplay();
    }

    public void ReloadReplay()
    {
        timer = 0;
        playing = true;
        done = false;
        LoadFile();
    }
    public void PauseReplay()
    {
        playing = !playing;
    }

    public void LoadFile()
    {
        // Load file
        string parentFolder = Application.dataPath + "/Data/" + folderName;
        string name = parentFolder + "/" + fileName + ".csv";
        if (File.Exists(name))
        {
            // CSV reader
            csvReader = new StreamReader(name);
            values = ReadLine(csvReader);
            // Reading
            timeOffset = values[0];
            done = false;
        }
        else
        {
            Debug.Log("File not found!");
            done = true;
        }
    }

    // Read one line from csv reader and convert it to float array
    private float[] ReadLine(StreamReader csvReader)
    {
        string line = csvReader.ReadLine();
        // End of file
        if(line == null)
        {
            done = true;
            Debug.Log("Replay done");
            return null;
        }
        // Get new line
        else
        {
            line = line.Substring(0, line.Length-1); // remove one extra ',' in the end
            string[] valuesString = line.Split(',');
            values = valuesString.Select(x => float.Parse(x)).ToArray();
            return values;
        }
    }


    // Read data and build the model
    void FixedUpdate()
    {
        // Finish replaying
        if (done)
            return;

        if (playing)
        {
            timer = timer + Time.fixedDeltaTime;
            // Update pose
            if (timer > values[0] - timeOffset)
            {
                // Update transform
                UpdateAllTrackers();
                // Get the new line
                values = ReadLine(csvReader);
            }
        }
    }


    private void UpdateAllTrackers()
    {
        UpdateTransform(head, values, indexHead);
        UpdateTransform(waist, values, indexWaist);
        UpdateTransform(leftShoulder, values, indexLeftShoulder);
        UpdateTransform(rightShoulder, values, indexRightShoulder);
        UpdateTransform(leftHand, values, indexLeftHand);
        UpdateTransform(rightHand, values, indexRightHand);
    }

    private void UpdateTransform(GameObject gameObject, float[] values, int i)
    {
        // Get transform from data file
        float x = values[i];
        float y = values[i+1];
        float z = values[i+2];
        float roll = values[i+3];
        float pitch = values[i+4];
        float yaw = values[i+5];

        // Set transform
        SetTransform(gameObject, new Vector3(x, y, z), new Vector3(roll, pitch, yaw));
    }
    private void SetTransform(GameObject gameObject, Vector3 position, Vector3 rotation)
    {
        SetTransform(gameObject, position, Quaternion.Euler(rotation));
    }
    private void SetTransform(GameObject gameObject, Vector3 position, Quaternion rotation)
    {
        gameObject.transform.position = position;
        gameObject.transform.rotation = rotation;
    }
}
