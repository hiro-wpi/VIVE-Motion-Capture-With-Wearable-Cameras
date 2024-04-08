using UnityEngine;
using OfficeOpenXml;
using System.IO;
using System.Threading;
using System.Globalization;
//using RootMotion.FinalIK;

public class ShadowReplay : MonoBehaviour
{
    public GameObject shadowBody;

    private ExcelPackage exl;
    private ExcelWorksheet worksheet;
    public string ExcelFilePath;
    private int row_max;

    public Transform head;
    public Transform left_hand;
    public Transform right_hand;
    public Transform left_elbow;
    public Transform right_elbow;
    public Transform clavicle;
    public Transform left_foot;
    public Transform right_foot;


    private int row = 2;
    private int col_head = 5;
    private int col_clavicle = 65;
    private int col_left_hand = 11;
    private int col_right_hand = 17;
    private int col_left_elbow = 23;
    private int col_right_elbow = 29;
    private int col_left_foot = 35;
    private int col_right_foot = 41;


    // Start is called before the first frame update
    void Start()
    {
        string filepath = Application.dataPath + "/" + ExcelFilePath;
        exl = new ExcelPackage(new FileStream(filepath, FileMode.Open));
        worksheet = exl.Workbook.Worksheets[1];
        row_max = worksheet.Dimension.Rows;

        //for (int i = 2; i <= exl.Workbook.Worksheets.Count; i++)
        //{
        //    worksheet = exl.Workbook.Worksheets[i];
        //    Debug.Log("asdasd:" + i);
        //    Debug.Log(worksheet.Dimension.Start);
        //    Debug.Log(worksheet.Dimension.End);

        //    Debug.Log("col start:" + worksheet.Dimension.Start.Column);
        //    Debug.Log("col end:" + worksheet.Dimension.End.Column);
        //    Debug.Log("row start:" + worksheet.Dimension.Start.Row);
        //    Debug.Log("row end:" + worksheet.Dimension.End.Row);
        //    Debug.Log("wrnm");

        //    /*for (int j = worksheet.Dimension.Start.Column, k = worksheet.Dimension.End.Column; j <= k; j++)
        //    {
        //        for (int m = worksheet.Dimension.Start.Row, n = worksheet.Dimension.End.Row; m <= n; m++)
        //        {
        //            string str = worksheet.GetValue(m, j).ToString();
        //            if (str != null)
        //            {
        //                // do something
        //                Debug.Log(str);
        //            }
        //        }
        //    }*/
        //}
    }

    public Transform PoseTransUpdate(Transform tf, ExcelWorksheet wk, int row, int col)
    {
        float x = float.Parse(wk.GetValue(row, col).ToString());
        float y = float.Parse(wk.GetValue(row, col + 1).ToString());
        float z = float.Parse(wk.GetValue(row, col + 2).ToString());
        float roll = float.Parse(wk.GetValue(row, col + 3).ToString());
        float pitch = float.Parse(wk.GetValue(row, col + 4).ToString());
        float yall = float.Parse(wk.GetValue(row, col + 5).ToString());

        tf.position = new Vector3(x, y, z);
        tf.rotation = Quaternion.Euler(new Vector3(roll, pitch, yall));
        return tf;
    }

    //public void FBBIKSolverSet(ref GameObject shadowBody, string effector)
    //{
    //    if (effector == "body")
    //    {
    //        ik.solver.bodyEffector.position = PoseTransUpdate(clavicle, worksheet, row, col_clavicle).position;
    //        ik.solver.bodyEffector.rotation = PoseTransUpdate(clavicle, worksheet, row, col_clavicle).rotation;
    //        ik.solver.bodyEffector.positionWeight = posWeight;
    //        ik.solver.bodyEffector.rotationWeight = rotWeight;
    //    }
    //    else if (effector == "left_hand")
    //    {
    //        ik.solver.leftHandEffector.position = PoseTransUpdate(left_hand, worksheet, row, col_left_hand).position;
    //        ik.solver.leftHandEffector.rotation = PoseTransUpdate(left_hand, worksheet, row, col_left_hand).rotation;
    //        ik.solver.leftHandEffector.positionWeight = posWeight;
    //        ik.solver.leftHandEffector.rotationWeight = rotWeight;
    //    }
    //    else if (effector == "right_hand")
    //    {
    //        ik.solver.rightHandEffector.position = PoseTransUpdate(right_hand, worksheet, row, col_right_hand).position;
    //        ik.solver.rightHandEffector.rotation = PoseTransUpdate(right_hand, worksheet, row, col_right_hand).rotation;
    //        ik.solver.rightHandEffector.positionWeight = posWeight;
    //        ik.solver.rightHandEffector.rotationWeight = rotWeight;
    //    }
    //    else if (effector == "left_elbow")
    //    {
    //        ik.solver.leftShoulderEffector.position = PoseTransUpdate(left_elbow, worksheet, row, col_left_elbow).position;
    //        ik.solver.leftShoulderEffector.rotation = PoseTransUpdate(left_elbow, worksheet, row, col_left_elbow).rotation;
    //        ik.solver.leftShoulderEffector.positionWeight = posWeight;
    //        ik.solver.leftShoulderEffector.rotationWeight = rotWeight;
    //    }
    //    else if (effector == "right_elbow")
    //    {
    //        ik.solver.rightShoulderEffector.position = PoseTransUpdate(right_elbow, worksheet, row, col_right_elbow).position;
    //        ik.solver.rightShoulderEffector.rotation = PoseTransUpdate(right_elbow, worksheet, row, col_right_elbow).rotation;
    //        ik.solver.rightShoulderEffector.positionWeight = posWeight;
    //        ik.solver.rightShoulderEffector.rotationWeight = rotWeight;
    //    }
    //    else if (effector == "left_foot")
    //    {
    //        ik.solver.leftFootEffector.position = PoseTransUpdate(left_foot, worksheet, row, col_left_foot).position;
    //        ik.solver.leftFootEffector.rotation = PoseTransUpdate(left_foot, worksheet, row, col_left_foot).rotation;
    //        ik.solver.leftFootEffector.positionWeight = posWeight;
    //        ik.solver.leftFootEffector.rotationWeight = rotWeight;
    //    }
    //    else if (effector == "right_foot")
    //    {
    //        ik.solver.rightFootEffector.position = PoseTransUpdate(right_foot, worksheet, row, col_right_foot).position;
    //        ik.solver.rightFootEffector.rotation = PoseTransUpdate(right_foot, worksheet, row, col_right_foot).rotation;
    //        ik.solver.rightFootEffector.positionWeight = posWeight;
    //        ik.solver.rightFootEffector.rotationWeight = rotWeight;
    //    }
    //}

    // Update is called once per frame
    void Update()
    {
        if (row < row_max)
        {

            //ik.solver.leftHandEffector.positionOffset += something;

            //ik.solver.leftHandEffector.maintainRelativePositionWeight = 1f;
            //ik.solver.rightHandEffector.maintainRelativePositionWeight = 1f;
            //FBBIKSolverSet(ref ik, "body");
            //FBBIKSolverSet(ref ik, "left_hand");
            //FBBIKSolverSet(ref ik, "right_hand");
            //FBBIKSolverSet(ref ik, "left_elbow");
            //FBBIKSolverSet(ref ik, "right_elbow");
            //FBBIKSolverSet(ref ik, "left_foot");
            //FBBIKSolverSet(ref ik, "right_foot");

            head = PoseTransUpdate(head, worksheet, row, col_head);
            clavicle = PoseTransUpdate(clavicle, worksheet, row, col_clavicle);
            left_hand = PoseTransUpdate(left_hand, worksheet, row, col_left_hand);
            right_hand = PoseTransUpdate(right_hand, worksheet, row, col_right_hand);
            left_elbow = PoseTransUpdate(left_elbow, worksheet, row, col_left_elbow);
            right_elbow = PoseTransUpdate(right_elbow, worksheet, row, col_right_elbow);
            left_foot = PoseTransUpdate(left_foot, worksheet, row, col_left_foot);
            right_foot = PoseTransUpdate(right_foot, worksheet, row, col_right_foot);

            row++;
            Thread.Sleep(100);
        }


    }
}
