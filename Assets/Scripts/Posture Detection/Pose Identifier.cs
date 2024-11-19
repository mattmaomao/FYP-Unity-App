using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum Pose { Draw_Ready, Draw, Aim, Release, Idle };

public class PoseIdentifier : MonoBehaviour
{
    [SerializeField] PostureDetectionManager PDM;
    public Pose currentPose = Pose.Idle;

    // aim judge
    float aimfluctuate = 0;
    float aimTolerance => shoulderWidth / 10;
    struct PositionData
    {
        public Vector2 position;
        public float timestamp;
    }
    List<PositionData> positionHistory = new();

    [Header("Debug")]
    [SerializeField] TextMeshProUGUI subPoseText;
    [SerializeField] TextMeshProUGUI poseText;

    // sub pose
    [SerializeField] float straightMargin = 50f;
    [SerializeField] float armUpTolerance => shoulderWidth / 2;
    float shoulderWidth;
    bool pushArm_Straight;
    bool pushArm_Up;
    bool pullArm_Up;
    bool pullArm_Bend;
    bool pullArm_BetweenShoulder;

    void Update()
    {
        checkSubPose();
        checkPose();

        // record wrist position
        positionHistory.Add(new PositionData { position = PDM.pointAnnotations[(int)PosePtIdx.RightWrist].transform.localPosition, timestamp = Time.time });
        positionHistory.RemoveAll(pos => Time.time - pos.timestamp > 1.0f);
        // calculate average position
        Vector2 avg = Vector2.zero;
        foreach (PositionData data in positionHistory)
            avg += data.position;
        avg /= positionHistory.Count;
        aimfluctuate = Vector2.Distance(PDM.pointAnnotations[(int)PosePtIdx.RightWrist].transform.localPosition, avg);

        // debug texts
        poseText.text = currentPose.ToString();
        subPoseText.text = "sub pose:" +
            $"shoulder width: {shoulderWidth.ToString("F2")}\n" +
            $"pushArm_Straight: {pushArm_Straight}\n" +
            $"pushArm_Up: {pushArm_Up}\n" +
            $"pullArm_Up: {pullArm_Up}\n" +
            $"{PDM.pointAnnotations[(int)PosePtIdx.RightElbow].transform.localPosition.y.ToString("F2")}, {(PDM.pointAnnotations[(int)PosePtIdx.RightShoulder].transform.localPosition.y - armUpTolerance).ToString("F2")}\n" +
            $"{PDM.pointAnnotations[(int)PosePtIdx.RightWrist].transform.localPosition.y.ToString("F2")}, {(PDM.pointAnnotations[(int)PosePtIdx.RightShoulder].transform.localPosition.y - armUpTolerance).ToString("F2")}\n" +
            $"{Mathf.Abs(PDM.pointAnnotations[(int)PosePtIdx.RightShoulder].transform.localPosition.y - PDM.pointAnnotations[(int)PosePtIdx.RightElbow].transform.localPosition.y).ToString("F2")}\n" +
            $"{Mathf.Abs(PDM.pointAnnotations[(int)PosePtIdx.RightElbow].transform.localPosition.y - PDM.pointAnnotations[(int)PosePtIdx.RightWrist].transform.localPosition.y).ToString("F2")}\n" +
            $"pullArm_Bend: {pullArm_Bend}\n" +
            $"pullArm_BetweenShoulder: {pullArm_BetweenShoulder}\n";
        subPoseText.text += aimfluctuate.ToString("F2");
    }

    void checkSubPose()
    {
        if (SystemInfo.deviceType != DeviceType.Handheld) {
            shoulderWidth = (
                Mathf.Abs(PDM.pointAnnotations[(int)PosePtIdx.RightShoulder].transform.localPosition.x -
                    PDM.pointAnnotations[(int)PosePtIdx.LeftShoulder].transform.localPosition.x)
            );
            pushArm_Straight = (
                Mathf.Abs(
                    // angle between shoulder and elbow
                    (float)(Math.Atan2(
                        (PDM.pointAnnotations[(int)PosePtIdx.LeftShoulder].transform.localPosition.y - PDM.pointAnnotations[(int)PosePtIdx.LeftElbow].transform.localPosition.y) /
                        (PDM.pointAnnotations[(int)PosePtIdx.LeftShoulder].transform.localPosition.x - PDM.pointAnnotations[(int)PosePtIdx.LeftElbow].transform.localPosition.x),
                        1.0f) * 180 / Math.PI) -
                    // angle between elbow and wrist
                    (float)(Math.Atan2(
                        (PDM.pointAnnotations[(int)PosePtIdx.LeftElbow].transform.localPosition.y - PDM.pointAnnotations[(int)PosePtIdx.LeftWrist].transform.localPosition.y) /
                        (PDM.pointAnnotations[(int)PosePtIdx.LeftElbow].transform.localPosition.x - PDM.pointAnnotations[(int)PosePtIdx.LeftWrist].transform.localPosition.x),
                        1.0f) * 180 / Math.PI)
                ) < straightMargin
            );
            pushArm_Up = (
                // elbow and wrist are above shoulder
                PDM.pointAnnotations[(int)PosePtIdx.LeftElbow].transform.localPosition.y > PDM.pointAnnotations[(int)PosePtIdx.LeftShoulder].transform.localPosition.y - armUpTolerance &&
                PDM.pointAnnotations[(int)PosePtIdx.LeftWrist].transform.localPosition.y > PDM.pointAnnotations[(int)PosePtIdx.LeftShoulder].transform.localPosition.y - armUpTolerance &&
                // elbow and wrist are near shoulder
                Mathf.Abs(PDM.pointAnnotations[(int)PosePtIdx.LeftShoulder].transform.localPosition.y -
                PDM.pointAnnotations[(int)PosePtIdx.LeftElbow].transform.localPosition.y) < armUpTolerance &&
                Mathf.Abs(PDM.pointAnnotations[(int)PosePtIdx.LeftElbow].transform.localPosition.y -
                PDM.pointAnnotations[(int)PosePtIdx.LeftWrist].transform.localPosition.y) < armUpTolerance
            );
            pullArm_Up = (
                // elbow and wrist are above shoulder
                PDM.pointAnnotations[(int)PosePtIdx.RightElbow].transform.localPosition.y > PDM.pointAnnotations[(int)PosePtIdx.RightShoulder].transform.localPosition.y - armUpTolerance &&
                PDM.pointAnnotations[(int)PosePtIdx.RightWrist].transform.localPosition.y > PDM.pointAnnotations[(int)PosePtIdx.RightShoulder].transform.localPosition.y - armUpTolerance &&
                // elbow and wrist are near shoulder
                Mathf.Abs(PDM.pointAnnotations[(int)PosePtIdx.RightShoulder].transform.localPosition.y -
                PDM.pointAnnotations[(int)PosePtIdx.RightElbow].transform.localPosition.y) < armUpTolerance &&
                Mathf.Abs(PDM.pointAnnotations[(int)PosePtIdx.RightElbow].transform.localPosition.y -
                PDM.pointAnnotations[(int)PosePtIdx.RightWrist].transform.localPosition.y) < armUpTolerance
            );
            pullArm_Bend = (
                // elbow is behind wrist
                PDM.pointAnnotations[(int)PosePtIdx.RightElbow].transform.localPosition.x >
                PDM.pointAnnotations[(int)PosePtIdx.RightWrist].transform.localPosition.x
            );
            pullArm_BetweenShoulder = (
                // wrist is between shoulders
                PDM.pointAnnotations[(int)PosePtIdx.RightShoulder].transform.localPosition.x >
                PDM.pointAnnotations[(int)PosePtIdx.RightWrist].transform.localPosition.x &&
                PDM.pointAnnotations[(int)PosePtIdx.RightWrist].transform.localPosition.x >
                PDM.pointAnnotations[(int)PosePtIdx.LeftShoulder].transform.localPosition.x
            );
        }
        else {
            shoulderWidth = (
                Mathf.Abs(PDM.pointAnnotations[(int)PosePtIdx.RightShoulder].transform.localPosition.y -
                    PDM.pointAnnotations[(int)PosePtIdx.LeftShoulder].transform.localPosition.y)
            );
            pushArm_Straight = (
                Mathf.Abs(
                    // angle between shoulder and elbow
                    (float)(Math.Atan2(
                        (PDM.pointAnnotations[(int)PosePtIdx.LeftShoulder].transform.localPosition.x - PDM.pointAnnotations[(int)PosePtIdx.LeftElbow].transform.localPosition.x) /
                        (PDM.pointAnnotations[(int)PosePtIdx.LeftShoulder].transform.localPosition.y - PDM.pointAnnotations[(int)PosePtIdx.LeftElbow].transform.localPosition.y),
                        1.0f) * 180 / Math.PI) -
                    // angle between elbow and wrist
                    (float)(Math.Atan2(
                        (PDM.pointAnnotations[(int)PosePtIdx.LeftElbow].transform.localPosition.x - PDM.pointAnnotations[(int)PosePtIdx.LeftWrist].transform.localPosition.x) /
                        (PDM.pointAnnotations[(int)PosePtIdx.LeftElbow].transform.localPosition.y - PDM.pointAnnotations[(int)PosePtIdx.LeftWrist].transform.localPosition.y),
                        1.0f) * 180 / Math.PI)
                ) < straightMargin
            );
            pushArm_Up = (
                // elbow and wrist are above shoulder
                PDM.pointAnnotations[(int)PosePtIdx.LeftElbow].transform.localPosition.x > PDM.pointAnnotations[(int)PosePtIdx.LeftShoulder].transform.localPosition.x - armUpTolerance &&
                PDM.pointAnnotations[(int)PosePtIdx.LeftWrist].transform.localPosition.x > PDM.pointAnnotations[(int)PosePtIdx.LeftShoulder].transform.localPosition.x - armUpTolerance &&
                // elbow and wrist are near shoulder
                Mathf.Abs(PDM.pointAnnotations[(int)PosePtIdx.LeftShoulder].transform.localPosition.x -
                PDM.pointAnnotations[(int)PosePtIdx.LeftElbow].transform.localPosition.x) < armUpTolerance &&
                Mathf.Abs(PDM.pointAnnotations[(int)PosePtIdx.LeftElbow].transform.localPosition.x -
                PDM.pointAnnotations[(int)PosePtIdx.LeftWrist].transform.localPosition.x) < armUpTolerance
            );
            pullArm_Up = (
                // elbow and wrist are above shoulder
                PDM.pointAnnotations[(int)PosePtIdx.RightElbow].transform.localPosition.x > PDM.pointAnnotations[(int)PosePtIdx.RightShoulder].transform.localPosition.x - armUpTolerance &&
                PDM.pointAnnotations[(int)PosePtIdx.RightWrist].transform.localPosition.x > PDM.pointAnnotations[(int)PosePtIdx.RightShoulder].transform.localPosition.x - armUpTolerance &&
                // elbow and wrist are near shoulder
                Mathf.Abs(PDM.pointAnnotations[(int)PosePtIdx.RightShoulder].transform.localPosition.x -
                PDM.pointAnnotations[(int)PosePtIdx.RightElbow].transform.localPosition.x) < armUpTolerance &&
                Mathf.Abs(PDM.pointAnnotations[(int)PosePtIdx.RightElbow].transform.localPosition.x -
                PDM.pointAnnotations[(int)PosePtIdx.RightWrist].transform.localPosition.x) < armUpTolerance
            );
            pullArm_Bend = (
                // elbow is behind wrist
                PDM.pointAnnotations[(int)PosePtIdx.RightElbow].transform.localPosition.y >
                PDM.pointAnnotations[(int)PosePtIdx.RightWrist].transform.localPosition.y
            );
            pullArm_BetweenShoulder = (
                // wrist is between shoulders
                PDM.pointAnnotations[(int)PosePtIdx.RightShoulder].transform.localPosition.y >
                PDM.pointAnnotations[(int)PosePtIdx.RightWrist].transform.localPosition.y &&
                PDM.pointAnnotations[(int)PosePtIdx.RightWrist].transform.localPosition.y >
                PDM.pointAnnotations[(int)PosePtIdx.LeftShoulder].transform.localPosition.y
            );

        }
    }

    void checkPose()
    {
        switch (currentPose)
        {
            case Pose.Idle:
                if (PDM.pointAnnotations[(int)PosePtIdx.RightElbow].transform.localPosition.y < PDM.pointAnnotations[(int)PosePtIdx.RightShoulder].transform.localPosition.y - armUpTolerance &&
                    PDM.pointAnnotations[(int)PosePtIdx.RightWrist].transform.localPosition.y < PDM.pointAnnotations[(int)PosePtIdx.RightShoulder].transform.localPosition.y - armUpTolerance &&
                    PDM.pointAnnotations[(int)PosePtIdx.LeftElbow].transform.localPosition.y < PDM.pointAnnotations[(int)PosePtIdx.LeftShoulder].transform.localPosition.y - armUpTolerance &&
                    PDM.pointAnnotations[(int)PosePtIdx.LeftWrist].transform.localPosition.y < PDM.pointAnnotations[(int)PosePtIdx.LeftShoulder].transform.localPosition.y - armUpTolerance)
                    currentPose = Pose.Draw_Ready;
                break;
            case Pose.Draw_Ready:
                if (pushArm_Up && pushArm_Straight && pullArm_Up)
                    currentPose = Pose.Draw;
                break;
            case Pose.Draw:
                if (pushArm_Up && pushArm_Straight && pullArm_Up && pullArm_BetweenShoulder && pullArm_Bend)
                {
                    if (aimfluctuate < aimTolerance)
                    {
                        currentPose = Pose.Aim;
                        positionHistory?.Clear();
                    }
                }
                else if (!(pushArm_Up && pushArm_Straight && pullArm_Up))
                {
                    Debug.Log("break from aim: " + !pushArm_Up + ", " + !pushArm_Straight + ", " + !pullArm_Up);
                    currentPose = Pose.Idle;
                }
                break;
            case Pose.Aim:
                if (pushArm_Up && pushArm_Straight && pullArm_Up && pullArm_BetweenShoulder && pullArm_Bend)
                {
                    if (aimfluctuate > aimTolerance)
                        currentPose = Pose.Release;
                }
                else
                {
                    Debug.Log("break from aim: " + pushArm_Up + ", " + pushArm_Straight + ", " + pullArm_Up + ", " + pullArm_BetweenShoulder + ", " + pullArm_Bend);
                    currentPose = Pose.Idle;
                }
                break;
            case Pose.Release:
                if (!(pushArm_Up && pushArm_Straight && pullArm_Up && pullArm_Bend))
                {
                    Debug.Log("break from release: " + !pushArm_Up + ", " + !pushArm_Straight + ", " + !pullArm_Up + ", " + !pullArm_Bend);
                    currentPose = Pose.Idle;
                }
                break;
            default:
                break;
        }
    }
}
