using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PosetureScoring : MonoBehaviour
{
    struct TimedPos { public float time; public Vector3 pos; }
    struct TimedAngle { public float time; public float angle; }

    [SerializeField] PostureDetectionManager PDM;
    [SerializeField] PoseIdentifier poseIdentifier;
    bool isScoring = false;

    //parts
    // wrist, elbow, shoulder
    float frontElbowAngle => Vector2.Angle(
        (PDM.pointAnnotations[(int)PosePtIdx.LeftElbow] - PDM.pointAnnotations[(int)PosePtIdx.LeftWrist]),
        (PDM.pointAnnotations[(int)PosePtIdx.LeftShoulder] - PDM.pointAnnotations[(int)PosePtIdx.LeftElbow]));
    float backElbowAngle => Vector2.Angle(
        (PDM.pointAnnotations[(int)PosePtIdx.RightElbow] - PDM.pointAnnotations[(int)PosePtIdx.RightWrist]),
        (PDM.pointAnnotations[(int)PosePtIdx.RightShoulder] - PDM.pointAnnotations[(int)PosePtIdx.RightElbow]));
    // elbow, shoulder, shoulder
    float frontShoulderAngle => Vector2.Angle(
        (PDM.pointAnnotations[(int)PosePtIdx.LeftShoulder] - PDM.pointAnnotations[(int)PosePtIdx.LeftElbow]),
        (PDM.pointAnnotations[(int)PosePtIdx.RightShoulder] - PDM.pointAnnotations[(int)PosePtIdx.LeftShoulder]));
    float backShoulderAngle => Vector2.Angle(
        (PDM.pointAnnotations[(int)PosePtIdx.RightShoulder] - PDM.pointAnnotations[(int)PosePtIdx.RightElbow]),
        (PDM.pointAnnotations[(int)PosePtIdx.LeftShoulder] - PDM.pointAnnotations[(int)PosePtIdx.RightShoulder]));

    // small part for scoring
    // fluctuate within 1 sec of releasing
    float timer = 0;
    float fluctureTime = 1;
    float frontWristFluctuate;
    float backWristFluctuate;
    float frontElbowAngleFluctuate;
    float backElbowAngleFluctuate;
    float frontShoulderAngleFluctuate;
    float backShoulderAngleFluctuate;
    List<TimedPos> frontWristPts = new();
    List<TimedPos> backWristPts = new();
    List<TimedAngle> frontElbowAngleChanges = new();
    List<TimedAngle> backElbowAngleChanges = new();
    List<TimedAngle> frontShoulderAngleChanges = new();
    List<TimedAngle> backShoulderAngleChanges = new();

    bool released = false;
    float releaseTime = 0;

    [Header("UI")]
    [SerializeField] GameObject startBtn;
    [SerializeField] GameObject startText;
    [SerializeField] GameObject recordingIndicator;
    [SerializeField] GameObject scoreDisplayPanel;
    [SerializeField] TextMeshProUGUI scoreDisplayText;

    [Header("Debug")]
    [SerializeField] GameObject debugPanel;
    [SerializeField] TextMeshProUGUI debugPanelText;

    void Start()
    {
        // reset
        stopScoring();
    }

    void Update()
    {
        // check if scoring
        if (!isScoring) return;

        if (!released && poseIdentifier.currentPose == Pose.Release)
        {
            released = true;
            releaseTime = timer;
        }
        if (poseIdentifier.currentPose != Pose.Idle && poseIdentifier.currentPose != Pose.Draw_Ready)
            recordJointsPos();
        else
        {
            clearRecord();
        }

        DebugTable();
    }

    // start btn
    public void StartScoring()
    {
        isScoring = true;
        startBtn.SetActive(false);
        startText.SetActive(false);
        recordingIndicator.SetActive(true);
        clearRecord();
        hideDisplayScore();
    }

    void stopScoring()
    {
        isScoring = false;
        startBtn.SetActive(true);
        startText.SetActive(true);
        recordingIndicator.SetActive(false);
        clearRecord();
        hideDisplayScore();
    }

    // clear list data
    void clearRecord()
    {
        released = false;
        timer = 0;
        releaseTime = -1;

        frontWristPts.Clear();
        backWristPts.Clear();
        frontElbowAngleChanges.Clear();
        backElbowAngleChanges.Clear();
        frontShoulderAngleChanges.Clear();
        backShoulderAngleChanges.Clear();
    }

    // save joints position, angle into list
    void recordJointsPos()
    {
        timer += Time.deltaTime;
        if (released && timer - releaseTime > 0.5f)
        {
            // poseIdentifier.currentPose = Pose.Idle;
            released = false;
            timer = 0;
            isScoring = false;

            // calculate score
            calculateScore();
            return;
        }

        // // remove old data
        // if (timer > fluctureTime && frontWristPts.Count > 0 && backWristPts.Count > 0)
        // {
        //     frontWristPts.RemoveAt(0);
        //     backWristPts.RemoveAt(0);
        // }

        // record new data
        frontWristPts.Add(new TimedPos { time = timer, pos = PDM.pointAnnotations[(int)PosePtIdx.LeftWrist] });
        backWristPts.Add(new TimedPos { time = timer, pos = PDM.pointAnnotations[(int)PosePtIdx.RightWrist] });
        frontElbowAngleChanges.Add(new TimedAngle { time = timer, angle = frontElbowAngle });
        backElbowAngleChanges.Add(new TimedAngle { time = timer, angle = backElbowAngle });
        frontShoulderAngleChanges.Add(new TimedAngle { time = timer, angle = frontShoulderAngle });
        backShoulderAngleChanges.Add(new TimedAngle { time = timer, angle = backShoulderAngle });
    }

    // calculate score
    void calculateScore()
    {
        // cal
        calFrontWristFluct();
        calBackWristFluct();

        calFrontElbowAngleFluct();
        calBackElbowAngleFluct();
        calFrontShoulderAngleFluct();
        calBackShoulderAngleFluct();

        // display
        displayScore();
    }

    #region calculation
    // calculate front wrist fluctuate
    void calFrontWristFluct()
    {
        Vector3 avgPt = Vector3.zero;
        int count = 0;

        for (int i = 0; i < frontWristPts.Count; i++)
        {
            if (Mathf.Abs(frontWristPts[i].time - releaseTime) < fluctureTime)
            {
                avgPt += frontWristPts[i].pos;
                count++;
            }
        }
        avgPt /= count;
        frontWristFluctuate = 0;
        for (int i = 0; i < frontWristPts.Count; i++)
        {
            if (Mathf.Abs(frontWristPts[i].time - releaseTime) < fluctureTime)
                frontWristFluctuate += Vector2.Distance(avgPt, frontWristPts[i].pos);
        }
        frontWristFluctuate /= count;
    }
    // calculate back wrist fluctuate
    void calBackWristFluct()
    {
        Vector3 avgPt = Vector3.zero;
        int count = 0;

        for (int i = 0; i < backWristPts.Count; i++)
        {
            if (Mathf.Abs(backWristPts[i].time - releaseTime) < fluctureTime)
            {
                avgPt += backWristPts[i].pos;
                count++;
            }
        }
        avgPt /= count;
        backWristFluctuate = 0;
        for (int i = 0; i < backWristPts.Count; i++)
        {
            if (Mathf.Abs(backWristPts[i].time - releaseTime) < fluctureTime)
                backWristFluctuate += Vector2.Distance(avgPt, backWristPts[i].pos);
        }
        backWristFluctuate /= count;
    }

    // calulate front elbow angle fluctuate
    void calFrontElbowAngleFluct()
    {
        float avgAngle = 0;
        int count = 0;

        for (int i = 0; i < frontElbowAngleChanges.Count; i++)
        {
            if (Mathf.Abs(frontElbowAngleChanges[i].time - releaseTime) < fluctureTime)
            {
                avgAngle += frontElbowAngleChanges[i].angle;
                count++;
            }
        }
        avgAngle /= count;
        frontElbowAngleFluctuate = 0;
        for (int i = 0; i < frontElbowAngleChanges.Count; i++)
        {
            if (Mathf.Abs(frontElbowAngleChanges[i].time - releaseTime) < fluctureTime)
                frontElbowAngleFluctuate += Mathf.Abs(avgAngle - frontElbowAngleChanges[i].angle);
        }
        frontElbowAngleFluctuate /= count;
    }
    // calulate back elbow angle fluctuate
    void calBackElbowAngleFluct()
    {
        float avgAngle = 0;
        int count = 0;

        for (int i = 0; i < backElbowAngleChanges.Count; i++)
        {
            if (Mathf.Abs(backElbowAngleChanges[i].time - releaseTime) < fluctureTime)
            {
                avgAngle += backElbowAngleChanges[i].angle;
                count++;
            }
        }
        avgAngle /= count;
        backElbowAngleFluctuate = 0;
        for (int i = 0; i < backElbowAngleChanges.Count; i++)
        {
            if (Mathf.Abs(backElbowAngleChanges[i].time - releaseTime) < fluctureTime)
                backElbowAngleFluctuate += Mathf.Abs(avgAngle - backElbowAngleChanges[i].angle);
        }
        backElbowAngleFluctuate /= count;
    }
    // calulate front shoulder angle fluctuate
    void calFrontShoulderAngleFluct()
    {
        float avgAngle = 0;
        int count = 0;

        for (int i = 0; i < frontShoulderAngleChanges.Count; i++)
        {
            if (Mathf.Abs(frontShoulderAngleChanges[i].time - releaseTime) < fluctureTime)
            {
                avgAngle += frontShoulderAngleChanges[i].angle;
                count++;
            }
        }
        avgAngle /= count;
        frontShoulderAngleFluctuate = 0;
        for (int i = 0; i < frontShoulderAngleChanges.Count; i++)
        {
            if (Mathf.Abs(frontShoulderAngleChanges[i].time - releaseTime) < fluctureTime)
                frontShoulderAngleFluctuate += Mathf.Abs(avgAngle - frontShoulderAngleChanges[i].angle);
        }
        frontShoulderAngleFluctuate /= count;
    }
    // calulate back shoulder angle fluctuate
    void calBackShoulderAngleFluct()
    {
        float avgAngle = 0;
        int count = 0;

        for (int i = 0; i < backShoulderAngleChanges.Count; i++)
        {
            if (Mathf.Abs(backShoulderAngleChanges[i].time - releaseTime) < fluctureTime)
            {
                avgAngle += backShoulderAngleChanges[i].angle;
                count++;
            }
        }
        avgAngle /= count;
        backShoulderAngleFluctuate = 0;
        for (int i = 0; i < backShoulderAngleChanges.Count; i++)
        {
            if (Mathf.Abs(backShoulderAngleChanges[i].time - releaseTime) < fluctureTime)
                backShoulderAngleFluctuate += Mathf.Abs(avgAngle - backShoulderAngleChanges[i].angle);
        }
        backShoulderAngleFluctuate /= count;
    }

    #endregion

    #region display
    void displayScore()
    {
        PDM.hideAnnotations();
        // display score
        scoreDisplayPanel.SetActive(true);
        
        scoreDisplayText.text = "front Elbow Angle Fluctuate\n";
        scoreDisplayText.text += frontElbowAngleFluctuate.ToString("F2");
        scoreDisplayText.text += "\n\nback Elbow Angle Fluctuate\n";
        scoreDisplayText.text += backElbowAngleFluctuate.ToString("F2");
        scoreDisplayText.text += "\n\nfront Shoulder Angle Fluctuate\n";
        scoreDisplayText.text += frontShoulderAngleFluctuate.ToString("F2");
        scoreDisplayText.text += "\n\nback Shoulder Angle Fluctuate\n";
        scoreDisplayText.text += backShoulderAngleFluctuate.ToString("F2");
        scoreDisplayText.text += "\n\nfront Wrist Fluctuate\n";
        scoreDisplayText.text += frontWristFluctuate.ToString("F2");
        scoreDisplayText.text += "\n\nback Wrist Fluctuate\n";
        scoreDisplayText.text += backWristFluctuate.ToString("F2");
    }
    void hideDisplayScore()
    {
        // hide score
        PDM.showAnnotations();
        scoreDisplayPanel.SetActive(false);
    }

    // back btn (cancel)
    public void cancelScore()
    {
        stopScoring();
    }
    // save btn
    public void saveScore()
    {
        // save score
        // todo
        Debug.Log("save score");

        stopScoring();
    }

    #endregion

    #region debug
    public void toggleDebug()
    {
        debugPanel.SetActive(!debugPanel.activeSelf);
    }

    // debug
    void DebugTable()
    {
        debugPanelText.text = "front Elbow Angle\n";
        debugPanelText.text += frontElbowAngle.ToString("F2");
        debugPanelText.text += "\n\nback Elbow Angle\n";
        debugPanelText.text += backElbowAngle.ToString("F2");
        debugPanelText.text += "\n\nfront Shoulder Angle\n";
        debugPanelText.text += frontShoulderAngle.ToString("F2");
        debugPanelText.text += "\n\nback Shoulder Angle\n";
        debugPanelText.text += backShoulderAngle.ToString("F2");
        debugPanelText.text += "\n\nfront Wrist Fluctuate\n";
        debugPanelText.text += frontWristFluctuate.ToString("F2");
        debugPanelText.text += "\n\nback Wrist Fluctuate\n";
        debugPanelText.text += backWristFluctuate.ToString("F2");
    }

    #endregion
}