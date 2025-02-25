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

    [Header("Sub Pose")]
    [SerializeField] float straightMargin = 50f;
    [SerializeField] float armUpTolerance => shoulderWidth / 2;
    public float shoulderWidth;
    bool pushArm_Straight;
    bool pushArm_Up;
    bool pullArm_Up;
    bool pullArm_Bend;
    bool pullArm_BetweenShoulder;


    [Header("Debug")]
    [SerializeField] TextMeshProUGUI subPoseText;
    [SerializeField] TextMeshProUGUI poseText;


    void Update()
    {
        checkSubPose();
        checkPose();

        // record wrist position
        if (PDM.pointAnnotations.Count > 0) {
            positionHistory.Add(new PositionData { position = PDM.pointAnnotations[(int)PosePtIdx.RightWrist], timestamp = Time.time });
            positionHistory.RemoveAll(pos => Time.time - pos.timestamp > 1.0f);
            // calculate average position
            Vector2 avg = Vector2.zero;
            foreach (PositionData data in positionHistory)
                avg += data.position;
            avg /= positionHistory.Count;
            aimfluctuate = Vector2.Distance(PDM.pointAnnotations[(int)PosePtIdx.RightWrist], avg);
        }
        
        // debug texts
        poseText.text = currentPose.ToString();
    }

    void checkSubPose()
    {
        if (PDM.pointAnnotations.Count <= 0) return;

        if (SystemInfo.deviceType != DeviceType.Handheld)
        {
            shoulderWidth = (
                Mathf.Abs(PDM.pointAnnotations[(int)PosePtIdx.RightShoulder].x -
                    PDM.pointAnnotations[(int)PosePtIdx.LeftShoulder].x)
            );
            pushArm_Straight = (
                Mathf.Abs(
                    // angle between shoulder and elbow
                    (float)(Math.Atan2(
                        (PDM.pointAnnotations[(int)PosePtIdx.LeftShoulder].y - PDM.pointAnnotations[(int)PosePtIdx.LeftElbow].y) /
                        (PDM.pointAnnotations[(int)PosePtIdx.LeftShoulder].x - PDM.pointAnnotations[(int)PosePtIdx.LeftElbow].x),
                        1.0f) * 180 / Math.PI) -
                    // angle between elbow and wrist
                    (float)(Math.Atan2(
                        (PDM.pointAnnotations[(int)PosePtIdx.LeftElbow].y - PDM.pointAnnotations[(int)PosePtIdx.LeftWrist].y) /
                        (PDM.pointAnnotations[(int)PosePtIdx.LeftElbow].x - PDM.pointAnnotations[(int)PosePtIdx.LeftWrist].x),
                        1.0f) * 180 / Math.PI)
                ) < straightMargin
            );
            pushArm_Up = (
                // elbow and wrist are above shoulder
                PDM.pointAnnotations[(int)PosePtIdx.LeftElbow].y > PDM.pointAnnotations[(int)PosePtIdx.LeftShoulder].y - armUpTolerance &&
                PDM.pointAnnotations[(int)PosePtIdx.LeftWrist].y > PDM.pointAnnotations[(int)PosePtIdx.LeftShoulder].y - armUpTolerance
            // elbow and wrist are near shoulder
            // Mathf.Abs(PDM.pointAnnotations[(int)PosePtIdx.LeftShoulder].y -
            // PDM.pointAnnotations[(int)PosePtIdx.LeftElbow].y) < armUpTolerance &&
            // Mathf.Abs(PDM.pointAnnotations[(int)PosePtIdx.LeftElbow].y -
            // PDM.pointAnnotations[(int)PosePtIdx.LeftWrist].y) < armUpTolerance
            );
            pullArm_Up = (
                // elbow and wrist are above shoulder
                PDM.pointAnnotations[(int)PosePtIdx.RightElbow].y > PDM.pointAnnotations[(int)PosePtIdx.RightShoulder].y - armUpTolerance &&
                PDM.pointAnnotations[(int)PosePtIdx.RightWrist].y > PDM.pointAnnotations[(int)PosePtIdx.RightShoulder].y - armUpTolerance
            // elbow and wrist are near shoulder
            // Mathf.Abs(PDM.pointAnnotations[(int)PosePtIdx.RightShoulder].y -
            // PDM.pointAnnotations[(int)PosePtIdx.RightElbow].y) < armUpTolerance &&
            // Mathf.Abs(PDM.pointAnnotations[(int)PosePtIdx.RightElbow].y -
            // PDM.pointAnnotations[(int)PosePtIdx.RightWrist].y) < armUpTolerance
            );
            pullArm_Bend = (
                // elbow is behind wrist
                PDM.pointAnnotations[(int)PosePtIdx.RightElbow].x >
                PDM.pointAnnotations[(int)PosePtIdx.RightWrist].x
            );
            pullArm_BetweenShoulder = (
                // wrist is between shoulders
                PDM.pointAnnotations[(int)PosePtIdx.RightShoulder].x >
                PDM.pointAnnotations[(int)PosePtIdx.RightWrist].x &&
                PDM.pointAnnotations[(int)PosePtIdx.RightWrist].x >
                PDM.pointAnnotations[(int)PosePtIdx.LeftShoulder].x
            );
        }
        else
        {
            shoulderWidth = (
                Mathf.Abs(-PDM.pointAnnotations[(int)PosePtIdx.RightShoulder].y -
                    -PDM.pointAnnotations[(int)PosePtIdx.LeftShoulder].y)
            );
            pushArm_Straight = (
                Mathf.Abs(
                    // angle between shoulder and elbow
                    (float)(Math.Atan2(
                        (PDM.pointAnnotations[(int)PosePtIdx.LeftShoulder].x - PDM.pointAnnotations[(int)PosePtIdx.LeftElbow].x) /
                        (-PDM.pointAnnotations[(int)PosePtIdx.LeftShoulder].y - -PDM.pointAnnotations[(int)PosePtIdx.LeftElbow].y),
                        1.0f) * 180 / Math.PI) -
                    // angle between elbow and wrist
                    (float)(Math.Atan2(
                        (PDM.pointAnnotations[(int)PosePtIdx.LeftElbow].x - PDM.pointAnnotations[(int)PosePtIdx.LeftWrist].x) /
                        (-PDM.pointAnnotations[(int)PosePtIdx.LeftElbow].y - -PDM.pointAnnotations[(int)PosePtIdx.LeftWrist].y),
                        1.0f) * 180 / Math.PI)
                ) < straightMargin
            );
            pushArm_Up = (
                // elbow and wrist are above shoulder
                PDM.pointAnnotations[(int)PosePtIdx.LeftElbow].x > PDM.pointAnnotations[(int)PosePtIdx.LeftShoulder].x - armUpTolerance &&
                PDM.pointAnnotations[(int)PosePtIdx.LeftWrist].x > PDM.pointAnnotations[(int)PosePtIdx.LeftShoulder].x - armUpTolerance
            // elbow and wrist are near shoulder
            // Mathf.Abs(PDM.pointAnnotations[(int)PosePtIdx.LeftShoulder].x -
            // PDM.pointAnnotations[(int)PosePtIdx.LeftElbow].x) < armUpTolerance &&
            // Mathf.Abs(PDM.pointAnnotations[(int)PosePtIdx.LeftElbow].x -
            // PDM.pointAnnotations[(int)PosePtIdx.LeftWrist].x) < armUpTolerance
            );
            pullArm_Up = (
                // elbow and wrist are above shoulder
                PDM.pointAnnotations[(int)PosePtIdx.RightElbow].x > PDM.pointAnnotations[(int)PosePtIdx.RightShoulder].x - armUpTolerance &&
                PDM.pointAnnotations[(int)PosePtIdx.RightWrist].x > PDM.pointAnnotations[(int)PosePtIdx.RightShoulder].x - armUpTolerance
            // elbow and wrist are near shoulder
            // Mathf.Abs(PDM.pointAnnotations[(int)PosePtIdx.RightShoulder].x -
            // PDM.pointAnnotations[(int)PosePtIdx.RightElbow].x) < armUpTolerance &&
            // Mathf.Abs(PDM.pointAnnotations[(int)PosePtIdx.RightElbow].x -
            // PDM.pointAnnotations[(int)PosePtIdx.RightWrist].x) < armUpTolerance
            );
            pullArm_Bend = (
                // elbow is behind wrist
                -PDM.pointAnnotations[(int)PosePtIdx.RightElbow].y >
                -PDM.pointAnnotations[(int)PosePtIdx.RightWrist].y
            );
            pullArm_BetweenShoulder = (
                // wrist is between shoulders
                -PDM.pointAnnotations[(int)PosePtIdx.RightShoulder].y >
                -PDM.pointAnnotations[(int)PosePtIdx.RightWrist].y &&
                -PDM.pointAnnotations[(int)PosePtIdx.RightWrist].y >
                -PDM.pointAnnotations[(int)PosePtIdx.LeftShoulder].y
            );

        }
    }

    void checkPose()
    {
        switch (currentPose)
        {
            case Pose.Idle:
                if (SystemInfo.deviceType != DeviceType.Handheld)
                {
                    if (PDM.pointAnnotations[(int)PosePtIdx.RightElbow].y < PDM.pointAnnotations[(int)PosePtIdx.RightShoulder].y - armUpTolerance &&
                        PDM.pointAnnotations[(int)PosePtIdx.RightWrist].y < PDM.pointAnnotations[(int)PosePtIdx.RightShoulder].y - armUpTolerance &&
                        PDM.pointAnnotations[(int)PosePtIdx.LeftElbow].y < PDM.pointAnnotations[(int)PosePtIdx.LeftShoulder].y - armUpTolerance &&
                        PDM.pointAnnotations[(int)PosePtIdx.LeftWrist].y < PDM.pointAnnotations[(int)PosePtIdx.LeftShoulder].y - armUpTolerance)
                        currentPose = Pose.Draw_Ready;
                }
                else
                {
                    if (PDM.pointAnnotations[(int)PosePtIdx.RightElbow].x < PDM.pointAnnotations[(int)PosePtIdx.RightShoulder].x - armUpTolerance &&
                        PDM.pointAnnotations[(int)PosePtIdx.RightWrist].x < PDM.pointAnnotations[(int)PosePtIdx.RightShoulder].x - armUpTolerance &&
                        PDM.pointAnnotations[(int)PosePtIdx.LeftElbow].x < PDM.pointAnnotations[(int)PosePtIdx.LeftShoulder].x - armUpTolerance &&
                        PDM.pointAnnotations[(int)PosePtIdx.LeftWrist].x < PDM.pointAnnotations[(int)PosePtIdx.LeftShoulder].x - armUpTolerance)
                        currentPose = Pose.Draw_Ready;
                }
                break;
            case Pose.Draw_Ready:
                if (pushArm_Up && pushArm_Straight && pullArm_Up)
                    currentPose = Pose.Draw;
                break;
            case Pose.Draw:
                if (pushArm_Up && pushArm_Straight && pullArm_Up && pullArm_BetweenShoulder && pullArm_Bend)
                {
                    // check if wrist is stable
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
                if (pushArm_Up && pushArm_Straight && pullArm_Up && pullArm_Bend)
                {
                    // check if wrist is stable
                    if (aimfluctuate > aimTolerance)
                        currentPose = Pose.Release;
                }
                else
                {
                    Debug.Log("break from aim: " + pushArm_Up + ", " + pushArm_Straight + ", " + pullArm_Up + ", " + pullArm_Bend);
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

    // force reset pose to idle
    public void resetPose()
    {
        currentPose = Pose.Idle;
    }
}
