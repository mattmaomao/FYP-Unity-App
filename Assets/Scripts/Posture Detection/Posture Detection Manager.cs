using System.Collections;
using System.Collections.Generic;
using Mediapipe.Unity;
using Mediapipe.Unity.Sample.PoseLandmarkDetection;
using UnityEngine;
using UnityEngine.UI;

public enum PosePtIdx
{
    // face
    Nose = 0,
    LeftEyeInner = 1, LeftEye = 2, LeftEyeOuter = 3,
    RightEyeInner = 4, RightEye = 5, RightEyeOuter = 6,
    LeftEar = 7, RightEar = 8,
    MouthLeft = 9, MouthRight = 10,
    // upper body
    LeftShoulder = 11, RightShoulder = 12,
    LeftElbow = 13, RightElbow = 14,
    LeftWrist = 15, RightWrist = 16,
    LeftPinky = 17, RightPinky = 18,
    LeftIndex = 19, RightIndex = 20,
    LeftThumb = 21, RightThumb = 22,
    // lower body
    LeftHip = 23, RightHip = 24,
    LeftKnee = 25, RightKnee = 26,
    LeftAnkle = 27, RightAnkle = 28,
    LeftHeel = 29, RightHeel = 30,
    LeftFootIndex = 31, RightFootIndex = 32
}

public enum PoseConnectionIdx
{
    // face
    Nose_LeftEyeInner = 0, LeftEyeInner_LeftEye = 1, LeftEye_LeftEyeOuter = 2, LeftEyeOuter_LeftEar = 3,
    Nose_RightEyeInner = 4, RightEyeInner_RightEye = 5, RightEye_RightEyeOuter = 6, RightEyeOuter_RightEar = 7,
    MouthLeft_MouthRight = 8,
    // upper body
    LeftShoulder_LeftElbow = 9, LeftElbow_LeftWrist = 10,
    LeftWrist_LeftPinky = 11, LeftWrist_LeftIndex = 12, LeftWrist_LeftThumb = 13, LeftPinky_LeftIndex = 14,

    RightShoulder_RightElbow = 15, RightElbow_RightWrist = 16,
    RightWrist_RightPinky = 17, RightWrist_RightIndex = 18, RightWrist_RightThumb = 19, RightPinky_RightIndex = 20,

    LeftShoulder_RightShoulder = 21,
    LeftHip_RightHip = 23,
    RightShoulder_RightHip = 22, LeftShoulder_LeftHip = 24,

    // lower body
    LeftHip_LeftKnee = 25, LeftKnee_LeftAnkle = 26,
    LeftAnkle_LeftHeel = 27, LeftAnkle_LeftFootIndex = 28, LeftHeel_LeftFootIndex = 29,

    RightHip_RightKnee = 30, RightKnee_RightAnkle = 31,
    RightAnkle_RightHeel = 32, RightAnkle_RightFootIndex = 33, RightHeel_RightFootIndex = 34
}

public class PostureDetectionManager : MonoBehaviour
{
    [SerializeField] MyLandmarkerRunner myLandmarkerRunner;
    [SerializeField] MultiPoseLandmarkListWithMaskAnnotation multiPoseLandmarkListWithMaskAnnotation;
    [SerializeField] List<PointAnnotation> pointAnnotations = new();
    [SerializeField] List<ConnectionAnnotation> connectionAnnotations = new();
    bool initedPointList = false;

    [Header("Setting")]
    [SerializeField] Dropdown _sourceInput;

    [Header("Debug")]
    [SerializeField] GameObject anslystWindow;
    [SerializeField] GameObject table;
    [SerializeField] List<GameObject> rowList = new();
    [SerializeField] GameObject rowPrefab;

    void Start()
    {
        generateTable();
    }

    // Update is called once per frame
    void Update()
    {
        if (!initedPointList)
        {
            if (multiPoseLandmarkListWithMaskAnnotation == null)
                return;

            if (multiPoseLandmarkListWithMaskAnnotation.count <= 0)
                return;

            getAnnoIntoList();
        }
        else
        {
            updateTable();
        }
    }

    // get annotations into list
    void getAnnoIntoList()
    {
        // point
        PointListAnnotation pointListAnnotation = multiPoseLandmarkListWithMaskAnnotation[0].getPointListAnnotation();

        pointAnnotations?.Clear();
        pointAnnotations = new();
        for (int i = 0; i < pointListAnnotation.count; i++)
        {
            PointAnnotation point = pointListAnnotation[i];
            pointAnnotations.Add(point);
        }

        // connection
        ConnectionListAnnotation connectionListAnnotation = multiPoseLandmarkListWithMaskAnnotation[0].getConnectionListAnnotation();

        connectionAnnotations?.Clear();
        connectionAnnotations = new();
        for (int i = 0; i < connectionListAnnotation.count; i++)
        {
            ConnectionAnnotation point = connectionListAnnotation[i];
            connectionAnnotations.Add(point);
        }

        disableUselessAnno();

        initedPointList = true;
    }

    // hide useless annotations (face, hands, feet)
    void disableUselessAnno()
    {
        // disable face nodes
        List<PosePtIdx> disablePts = new() {
            PosePtIdx.Nose,
            PosePtIdx.LeftEyeInner,
            PosePtIdx.LeftEye,
            PosePtIdx.LeftEyeOuter,
            PosePtIdx.RightEyeInner,
            PosePtIdx.RightEye,
            PosePtIdx.RightEyeOuter,
            PosePtIdx.LeftEar,
            PosePtIdx.RightEar,
            PosePtIdx.MouthLeft,
            PosePtIdx.MouthRight,

            PosePtIdx.LeftPinky,
            PosePtIdx.RightPinky,
            PosePtIdx.LeftIndex,
            PosePtIdx.RightIndex,
            PosePtIdx.LeftThumb,
            PosePtIdx.RightThumb,

            PosePtIdx.LeftHeel,
            PosePtIdx.RightHeel,
            PosePtIdx.LeftFootIndex,
            PosePtIdx.RightFootIndex
        };
        for (int i = 0; i <= pointAnnotations.Count; i++)
        {
            if (disablePts.Contains((PosePtIdx)i))
                pointAnnotations[i].SetRadius(0.0f);
        }

        // disable face nodes
        List<PoseConnectionIdx> disableCons = new() {
            PoseConnectionIdx.Nose_LeftEyeInner,
            PoseConnectionIdx.LeftEyeInner_LeftEye,
            PoseConnectionIdx.LeftEye_LeftEyeOuter,
            PoseConnectionIdx.LeftEyeOuter_LeftEar,
            PoseConnectionIdx.Nose_RightEyeInner,
            PoseConnectionIdx.RightEyeInner_RightEye,
            PoseConnectionIdx.RightEye_RightEyeOuter,
            PoseConnectionIdx.RightEyeOuter_RightEar,
            PoseConnectionIdx.MouthLeft_MouthRight,

            PoseConnectionIdx.LeftWrist_LeftPinky,
            PoseConnectionIdx.LeftWrist_LeftIndex,
            PoseConnectionIdx.LeftWrist_LeftThumb,
            PoseConnectionIdx.LeftPinky_LeftIndex,
            PoseConnectionIdx.RightWrist_RightPinky,
            PoseConnectionIdx.RightWrist_RightIndex,
            PoseConnectionIdx.RightWrist_RightThumb,
            PoseConnectionIdx.RightPinky_RightIndex,

            PoseConnectionIdx.LeftAnkle_LeftHeel,
            PoseConnectionIdx.LeftAnkle_LeftFootIndex,
            PoseConnectionIdx.LeftHeel_LeftFootIndex,
            PoseConnectionIdx.RightAnkle_RightHeel,
            PoseConnectionIdx.RightAnkle_RightFootIndex,
            PoseConnectionIdx.RightHeel_RightFootIndex,
        };
        for (int i = 0; i <= connectionAnnotations.Count; i++)
        {
            if (disableCons.Contains((PoseConnectionIdx)i))
                connectionAnnotations[i].SetLineWidth(0.0f);
        }
    }

    string[] detectValue = new string[] {
        "L shoulder Pos X", "L shoulder Pos Y", "L shoulder Pos Z",
        "R shoulder Pos X", "R shoulder Pos Y", "R shoulder Pos Z"
    };

    void updateTable()
    {
        // calculate detect value
        rowList[0].GetComponent<TableData>().readData(pointAnnotations[(int)PosePtIdx.LeftShoulder].transform.localPosition.x);
        rowList[1].GetComponent<TableData>().readData(pointAnnotations[(int)PosePtIdx.LeftShoulder].transform.localPosition.y);
        rowList[2].GetComponent<TableData>().readData(pointAnnotations[(int)PosePtIdx.LeftShoulder].transform.localPosition.z);
        rowList[3].GetComponent<TableData>().readData(pointAnnotations[(int)PosePtIdx.RightShoulder].transform.localPosition.x);
        rowList[4].GetComponent<TableData>().readData(pointAnnotations[(int)PosePtIdx.RightShoulder].transform.localPosition.y);
        rowList[5].GetComponent<TableData>().readData(pointAnnotations[(int)PosePtIdx.RightShoulder].transform.localPosition.z);
    }

    void generateTable()
    {
        // clear table
        foreach (GameObject row in rowList)
            Destroy(row);
        rowList?.Clear();

        // generate table
        for (int i = 0; i < detectValue.Length; i++)
        {
            GameObject row = Instantiate(rowPrefab, table.transform);
            rowList.Add(row);
            rowList[i].GetComponent<TableData>().nameText.text = detectValue[i];
        }
    }

    // debug
    public void toggleAnalyst()
    {
        anslystWindow.SetActive(!anslystWindow.activeSelf);
    }

    // change input mode
    // later
    // [SerializeField] int setting = 3;
    // [ContextMenu("Change Mode")]
    // // public void changeMode(int idx) {
    // public void changeMode() {
    //     StartCoroutine(changeModeCoroutine());
        
    // }

    // IEnumerator changeModeCoroutine()
    // {
    //     myLandmarkerRunner.Stop();
    //     yield return new WaitForSeconds(0.1f);
    //     // myLandmarkerRunner.changeOptions(idx);
    //     myLandmarkerRunner.changeOptions(setting);
    //     yield return new WaitForSeconds(0.1f);
    //     myLandmarkerRunner.Play();
    // }
}
