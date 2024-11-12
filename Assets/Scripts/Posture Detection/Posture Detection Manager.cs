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
    RightEyeInner = 1, RightEye = 2, RightEyeOuter = 3,
    LeftEyeInner = 4, LeftEye = 5, LeftEyeOuter = 6,
    RightEar = 7, LeftEar = 8,
    MouthRight = 9, MouthLeft = 10,
    // upper body
    RightShoulder = 11, LeftShoulder = 12,
    RightElbow = 13, LeftElbow = 14,
    RightWrist = 15, LeftWrist = 16,
    RightPinky = 17, LeftPinky = 18,
    RightIndex = 19, LeftIndex = 20,
    RightThumb = 21, LeftThumb = 22,
    // lower body
    RightHip = 23, LeftHip = 24,
    RightKnee = 25, LeftKnee = 26,
    RightAnkle = 27, LeftAnkle = 28,
    RightHeel = 29, LeftHeel = 30,
    RightFootIndex = 31, LeftFootIndex = 32
}

public enum PoseConnectionIdx
{
    // face
    Nose_RightEyeInner = 0, RightEyeInner_RightEye = 1, RightEye_RightEyeOuter = 2, RightEyeOuter_RightEar = 3,
    Nose_LeftEyeInner = 4, LeftEyeInner_LeftEye = 5, LeftEye_LeftEyeOuter = 6, LeftEyeOuter_LeftEar = 7,
    MouthRight_MouthLeft = 8,
    // upper body
    RightShoulder_RightElbow = 9, RightElbow_RightWrist = 10,
    RightWrist_RightPinky = 11, RightWrist_RightIndex = 12, RightWrist_RightThumb = 13, RightPinky_RightIndex = 14,

    LeftShoulder_LeftElbow = 15, LeftElbow_LeftWrist = 16,
    LeftWrist_LeftPinky = 17, LeftWrist_LeftIndex = 18, LeftWrist_LeftThumb = 19, LeftPinky_LeftIndex = 20,

    RightShoulder_LeftShoulder = 21,
    RightHip_LeftHip = 23,
    LeftShoulder_LeftHip = 22, RightShoulder_RightHip = 24,

    // lower body
    RightHip_RightKnee = 25, RightKnee_RightAnkle = 26,
    RightAnkle_RightHeel = 27, RightAnkle_RightFootIndex = 28, RightHeel_RightFootIndex = 29,

    LeftHip_LeftKnee = 30, LeftKnee_LeftAnkle = 31,
    LeftAnkle_LeftHeel = 32, LeftAnkle_LeftFootIndex = 33, LeftHeel_LeftFootIndex = 34
}

public class PostureDetectionManager : MonoBehaviour
{
    [SerializeField] MyLandmarkerRunner myLandmarkerRunner;
    [SerializeField] MultiPoseLandmarkListWithMaskAnnotation multiPoseLandmarkListWithMaskAnnotation;
    public List<PointAnnotation> pointAnnotations = new();
    [SerializeField] List<ConnectionAnnotation> connectionAnnotations = new();
    bool initedPointList = false;

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
            PoseConnectionIdx.MouthRight_MouthLeft,

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


    #region debug
    string[] detectValue = new string[] {
        "L shoulder", "L Elbow", "L Wrist",
        "R shoulder", "R Elbow", "R Wrist",
        "L Hip", "L Knee", "L Ankle",
        "R Hip", "R Knee", "R Ankle",
    };
    int[] detectIdx = new int[] {
        (int)PosePtIdx.LeftShoulder,
        (int)PosePtIdx.LeftElbow,
        (int)PosePtIdx.LeftWrist,
        (int)PosePtIdx.RightShoulder,
        (int)PosePtIdx.RightElbow,
        (int)PosePtIdx.RightWrist,
        (int)PosePtIdx.LeftHip,
        (int)PosePtIdx.LeftKnee,
        (int)PosePtIdx.LeftAnkle,
        (int)PosePtIdx.RightHip,
        (int)PosePtIdx.RightKnee,
        (int)PosePtIdx.RightAnkle
    };

    void updateTable()
    {
        // calculate detect value
        for (int i = 0; i < detectIdx.Length; i++)
            rowList[i].GetComponent<TableData>().readData(pointAnnotations[detectIdx[i]].transform.localPosition);
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

    public void toggleAnalyst()
    {
        anslystWindow.SetActive(!anslystWindow.activeSelf);
    }
    #endregion

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
