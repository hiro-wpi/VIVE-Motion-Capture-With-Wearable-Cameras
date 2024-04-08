using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanStickModel : MonoBehaviour
{
    // Body data
    // offset for calibration
    public float waistRotationX;
    public float waistRotationZ;
    public float bodyCenterOffset;
    public float leftUpperTrackerRotationZ;
    public float leftLowerTrackerRotationZ;
    public float rightUpperTrackerRotationZ;
    public float rightLowerTrackerRotationZ;
    public float leftUpperTrackerOffset;
    public float leftLowerTrackerOffset;
    public float rightUpperTrackerOffset;
    public float rightLowerTrackerOffset;
    // body dimension
    public float upperBodyLength;
    public float shoulderWidth;
    public float upperArmLength;
    public float lowerArmLength;
    

    // Trackers
    public GameObject head;
    public GameObject waist;
    public GameObject leftShoulder;
    public GameObject rightShoulder;
    public GameObject leftHand;
    public GameObject rightHand;

    // Stick model
    public GameObject bodyJointPrefab;
    public GameObject bodyStickPrefab;
    
    private GameObject waistJoint;
    private GameObject chestJoint;
    private GameObject neckJoint;
    private GameObject headJoint;
    private GameObject leftShoulderJoint;
    private GameObject rightShoulderJoint;
    private GameObject leftElbowJoint;
    private GameObject rightElbowJoint;
    private GameObject leftWristJoint;
    private GameObject rightWristJoint;

    private GameObject lowerBodyStick;
    private GameObject upperBodyUpperStick;
    private GameObject upperBodyLowerStick;
    private GameObject neckStick;
    private GameObject leftShoulderStick;
    private GameObject rightShoulderStick;
    private GameObject leftUpperArmStick;
    private GameObject rightUpperArmStick;
    private GameObject leftLowerArmStick;
    private GameObject rightLowerArmStick;
    private GameObject leftHandStick;
    private GameObject rightHandStick;


    void Start()
    {
        waistJoint = AddNewJoint(gameObject);
        waistJoint.name = "Waist Joint";
        // chestJoint = AddNewJoint(gameObject);
        // chestJoint.name = "Chest Joint";
        neckJoint = AddNewJoint(waistJoint);
        neckJoint.name = "Neck Joint";
        headJoint = AddNewJoint(neckJoint);
        headJoint.name = "Head Joint";
        leftShoulderJoint = AddNewJoint(neckJoint);
        leftShoulderJoint.name = "Left Shoulder Joint";
        rightShoulderJoint = AddNewJoint(neckJoint);
        rightShoulderJoint.name = "Right Shoulder Joint";
        leftElbowJoint = AddNewJoint(leftShoulderJoint);
        leftElbowJoint.name = "Left Elbow Joint";
        rightElbowJoint = AddNewJoint(rightShoulderJoint);
        rightElbowJoint.name = "Right Elbow Joint";
        leftWristJoint = AddNewJoint(leftElbowJoint);
        leftWristJoint.name = "Left Hand Joint";
        rightWristJoint = AddNewJoint(rightElbowJoint);
        rightWristJoint.name = "Right Hand Joint";
    }


    // Add human model
    private GameObject AddNewJoint(GameObject parent=null)
    {
        return AddNewGameObject(bodyJointPrefab, parent);
    }
    private GameObject AddNewStick(GameObject parent=null)
    {
        return AddNewGameObject(bodyStickPrefab, parent);
    }
    private GameObject AddNewGameObject(GameObject gameObject, GameObject parent=null)
    {
        GameObject instance = Instantiate(gameObject, new Vector3(), new Quaternion());
        if (parent != null)
            instance.transform.parent = parent.transform;
        return instance;
    }


    // Read data and build the model
    void FixedUpdate()
    {
        UpdateStickBody();
    }

    private void UpdateStickBody()
    {
        // Joints

        // waist - coordinate center
        SetTransform(waistJoint, waist.transform);
        waistJoint.transform.rotation = Quaternion.Euler(waist.transform.rotation.eulerAngles 
                                      - new Vector3(waistRotationX, 0, waistRotationZ));
        waistJoint.transform.Translate(new Vector3(0, 0, -bodyCenterOffset), Space.Self);

        // neck
        neckJoint.transform.localPosition = new Vector3(0, upperBodyLength, 0);

        // head - head tracker
        SetTransform(headJoint, head.transform);
        headJoint.transform.Translate(new Vector3(0, 0, -0.15f), Space.Self);

        // shoulder 
        // Cannot just assume that the shoulder is staic
        // leftShoulderJoint.transform.localPosition = new Vector3(-shoulderWidth, 0, 0);
        // rightShoulderJoint.transform.localPosition = new Vector3(+shoulderWidth, 0, 0);
        SetTransform(leftShoulderJoint, leftShoulder.transform);
        leftShoulderJoint.transform.Translate(new Vector3(0, -leftUpperTrackerOffset, 0), Space.Self);
        SetTransform(rightShoulderJoint, rightShoulder.transform);
        rightShoulderJoint.transform.Translate(new Vector3(0, -rightUpperTrackerOffset, 0), Space.Self);


        // TODO
        // The way to calculate elbow position could be a combination result
        // from both trackers instead of just one tracker
        // elbow
        SetTransform(leftElbowJoint, leftShoulderJoint.transform);
        MoveTowards(leftElbowJoint, 
                    leftShoulderJoint.transform.position, 
                    leftShoulder.transform.position,
                    upperArmLength);
        SetTransform(rightElbowJoint, rightShoulderJoint.transform);
        MoveTowards(rightElbowJoint, 
                    rightShoulderJoint.transform.position, 
                    rightShoulder.transform.position, 
                    upperArmLength);

        // hand
        SetTransform(leftWristJoint, leftElbowJoint.transform);
        MoveTowards(leftWristJoint, 
                    leftElbowJoint.transform.position, 
                    leftHand.transform.position, 
                    lowerArmLength);
        SetTransform(rightWristJoint, rightElbowJoint.transform);
        MoveTowards(rightWristJoint, 
                    rightElbowJoint.transform.position, 
                    rightHand.transform.position, 
                    lowerArmLength);

        // Sticks
        DrawLineConnect(waistJoint, neckJoint);
        DrawLineConnect(neckJoint, headJoint);
        DrawLineConnect(neckJoint, leftShoulderJoint);
        DrawLineConnect(neckJoint, rightShoulderJoint);
        DrawLineConnect(leftShoulderJoint, leftElbowJoint);
        DrawLineConnect(rightShoulderJoint, rightElbowJoint);
        DrawLineConnect(leftElbowJoint, leftWristJoint);
        DrawLineConnect(rightElbowJoint, rightWristJoint);
    }


    // Utils
    private void MoveTowards(GameObject gameObject, 
                             Vector3 from, Vector3 to, float distance)
    {
        Vector3 direction = to - from;
        gameObject.transform.position += Vector3.Normalize(direction) * distance;
    }

    private void SetTransform(GameObject gameObject, Transform transform)
    {
        SetTransform(gameObject, transform.position, transform.rotation);
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

    private void DrawLineConnect(GameObject obj1, GameObject obj2)
    {
        Debug.DrawLine(obj1.transform.position, obj2.transform.position, 
                       Color.blue, Time.fixedDeltaTime);
    }
}
