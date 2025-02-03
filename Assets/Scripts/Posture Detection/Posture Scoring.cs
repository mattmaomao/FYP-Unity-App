using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PosetureScoring : MonoBehaviour
{
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
    float fluctuateTime = 1;

    // front wrist
    float frontWristFluctuate;
    Vector3 frontWristStart;
    Vector3 frontWristEnd;
    // back wrist
    float backWristFluctuate;
    Vector3 backWristStart;
    Vector3 backWristEnd;
    // front elbow
    float frontElbowAngleFluctuate;
    float frontElbowAngleStart;
    float frontElbowAngleEnd;
    // back elbow
    float backElbowAngleFluctuate;
    float backElbowAngleStart;
    float backElbowAngleEnd;
    // front shoulder
    float frontShoulderAngleFluctuate;
    float frontShoulderAngleStart;
    float frontShoulderAngleEnd;
    // back shoulder
    float backShoulderAngleFluctuate;
    float backShoulderAngleStart;
    float backShoulderAngleEnd;

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
    public void startScoring()
    {
        clearRecord();
        hideDisplayScore();

        startBtn.SetActive(false);
        startText.SetActive(false);
        recordingIndicator.SetActive(true);

        poseIdentifier.resetPose();

        isScoring = true;
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

        frontWristStart = default;
        frontWristEnd = default;
        backWristStart = default;
        backWristEnd = default;
        frontElbowAngleStart = default;
        frontElbowAngleEnd = default;
        backElbowAngleStart = default;
        backElbowAngleEnd = default;
        frontShoulderAngleStart = default;
        frontShoulderAngleEnd = default;
        backShoulderAngleStart = default;
        backShoulderAngleEnd = default;
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

        // debug
        // StartCoroutine(autoSave());
    }

    #region calculation
    // calculate front wrist fluctuate
    void calFrontWristFluct()
    {
        Vector3 avgPt = Vector3.zero;
        int count = 0;
        int startIdx, endIdx;

        startIdx = frontWristPts.Count - 1;
        endIdx = 0;

        // filter points within fluctuate time
        for (int i = 0; i < frontWristPts.Count; i++)
        {
            if (Mathf.Abs(frontWristPts[i].time - releaseTime) < fluctuateTime)
            {
                // find start and end index of point within the fluctuate time
                if (frontWristPts[startIdx].time > frontWristPts[i].time)
                    startIdx = i;
                if (frontWristPts[endIdx].time < frontWristPts[i].time)
                    endIdx = i;

                avgPt += frontWristPts[i].pos;
                count++;
            }
        }

        // calculate average
        avgPt /= count;
        frontWristFluctuate = 0;
        for (int i = 0; i < frontWristPts.Count; i++)
        {
            if (Mathf.Abs(frontWristPts[i].time - releaseTime) < fluctuateTime)
                frontWristFluctuate += Vector2.Distance(avgPt, frontWristPts[i].pos);
        }
        frontWristFluctuate /= count;

        // get start, end points
        frontWristStart = frontWristPts[startIdx].pos;
        frontWristEnd = frontWristPts[endIdx].pos;
    }
    // calculate back wrist fluctuate
    void calBackWristFluct()
    {
        Vector3 avgPt = Vector3.zero;
        int count = 0;
        int startIdx, endIdx;

        startIdx = backWristPts.Count - 1;
        endIdx = 0;

        // filter points within fluctuate time
        for (int i = 0; i < backWristPts.Count; i++)
        {
            if (Mathf.Abs(backWristPts[i].time - releaseTime) < fluctuateTime)
            {
                // find start and end index of point within the fluctuate time
                if (backWristPts[startIdx].time > backWristPts[i].time)
                    startIdx = i;
                if (backWristPts[endIdx].time < backWristPts[i].time)
                    endIdx = i;

                avgPt += backWristPts[i].pos;
                count++;
            }
        }

        // calculate average
        avgPt /= count;
        backWristFluctuate = 0;
        for (int i = 0; i < backWristPts.Count; i++)
        {
            if (Mathf.Abs(backWristPts[i].time - releaseTime) < fluctuateTime)
                backWristFluctuate += Vector2.Distance(avgPt, backWristPts[i].pos);
        }
        backWristFluctuate /= count;

        // get start, end points
        backWristStart = backWristPts[startIdx].pos;
        backWristEnd = backWristPts[endIdx].pos;
    }

    // calulate front elbow angle fluctuate
    void calFrontElbowAngleFluct()
    {
        float avgAngle = 0;
        int count = 0;
        int startIdx, endIdx;

        startIdx = frontElbowAngleChanges.Count - 1;
        endIdx = 0;

        // filter points within fluctuate time
        for (int i = 0; i < frontElbowAngleChanges.Count; i++)
        {
            if (Mathf.Abs(frontElbowAngleChanges[i].time - releaseTime) < fluctuateTime)
            {
                // find start and end index of point within the fluctuate time
                if (frontElbowAngleChanges[startIdx].time > frontElbowAngleChanges[i].time)
                    startIdx = i;
                if (frontElbowAngleChanges[endIdx].time < frontElbowAngleChanges[i].time)
                    endIdx = i;

                avgAngle += frontElbowAngleChanges[i].angle;
                count++;
            }
        }

        // calculate average
        avgAngle /= count;
        frontElbowAngleFluctuate = 0;
        for (int i = 0; i < frontElbowAngleChanges.Count; i++)
        {
            if (Mathf.Abs(frontElbowAngleChanges[i].time - releaseTime) < fluctuateTime)
                frontElbowAngleFluctuate += Mathf.Abs(avgAngle - frontElbowAngleChanges[i].angle);
        }
        frontElbowAngleFluctuate /= count;

        // get start, end points
        frontElbowAngleStart = frontElbowAngleChanges[startIdx].angle;
        frontElbowAngleEnd = frontElbowAngleChanges[endIdx].angle;
    }
    // calulate back elbow angle fluctuate
    void calBackElbowAngleFluct()
    {
        float avgAngle = 0;
        int count = 0;
        int startIdx, endIdx;

        startIdx = backElbowAngleChanges.Count - 1;
        endIdx = 0;

        // filter points within fluctuate time
        for (int i = 0; i < backElbowAngleChanges.Count; i++)
        {
            if (Mathf.Abs(backElbowAngleChanges[i].time - releaseTime) < fluctuateTime)
            {
                // find start and end index of point within the fluctuate time
                if (backElbowAngleChanges[startIdx].time > backElbowAngleChanges[i].time)
                    startIdx = i;
                if (backElbowAngleChanges[endIdx].time < backElbowAngleChanges[i].time)
                    endIdx = i;

                avgAngle += backElbowAngleChanges[i].angle;
                count++;
            }
        }

        // calculate average
        avgAngle /= count;
        backElbowAngleFluctuate = 0;
        for (int i = 0; i < backElbowAngleChanges.Count; i++)
        {
            if (Mathf.Abs(backElbowAngleChanges[i].time - releaseTime) < fluctuateTime)
                backElbowAngleFluctuate += Mathf.Abs(avgAngle - backElbowAngleChanges[i].angle);
        }
        backElbowAngleFluctuate /= count;

        // get start, end points
        backElbowAngleStart = backElbowAngleChanges[startIdx].angle;
        backElbowAngleEnd = backElbowAngleChanges[endIdx].angle;
    }
    // calulate front shoulder angle fluctuate
    void calFrontShoulderAngleFluct()
    {
        float avgAngle = 0;
        int count = 0;
        int startIdx, endIdx;

        startIdx = frontShoulderAngleChanges.Count - 1;
        endIdx = 0;

        // filter points within fluctuate time
        for (int i = 0; i < frontShoulderAngleChanges.Count; i++)
        {
            if (Mathf.Abs(frontShoulderAngleChanges[i].time - releaseTime) < fluctuateTime)
            {
                // find start and end index of point within the fluctuate time
                if (frontShoulderAngleChanges[startIdx].time > frontShoulderAngleChanges[i].time)
                    startIdx = i;
                if (frontShoulderAngleChanges[endIdx].time < frontShoulderAngleChanges[i].time)
                    endIdx = i;

                avgAngle += frontShoulderAngleChanges[i].angle;
                count++;
            }
        }

        // calculate average
        avgAngle /= count;
        frontShoulderAngleFluctuate = 0;
        for (int i = 0; i < frontShoulderAngleChanges.Count; i++)
        {
            if (Mathf.Abs(frontShoulderAngleChanges[i].time - releaseTime) < fluctuateTime)
                frontShoulderAngleFluctuate += Mathf.Abs(avgAngle - frontShoulderAngleChanges[i].angle);
        }
        frontShoulderAngleFluctuate /= count;

        // get start, end points
        frontShoulderAngleStart = frontShoulderAngleChanges[startIdx].angle;
        frontShoulderAngleEnd = frontShoulderAngleChanges[endIdx].angle;
    }
    // calulate back shoulder angle fluctuate
    void calBackShoulderAngleFluct()
    {
        float avgAngle = 0;
        int count = 0;
        int startIdx, endIdx;

        startIdx = backShoulderAngleChanges.Count - 1;
        endIdx = 0;

        // filter points within fluctuate time
        for (int i = 0; i < backShoulderAngleChanges.Count; i++)
        {
            if (Mathf.Abs(backShoulderAngleChanges[i].time - releaseTime) < fluctuateTime)
            {
                // find start and end index of point within the fluctuate time
                if (backShoulderAngleChanges[startIdx].time > backShoulderAngleChanges[i].time)
                    startIdx = i;
                if (backShoulderAngleChanges[endIdx].time < backShoulderAngleChanges[i].time)
                    endIdx = i;

                avgAngle += backShoulderAngleChanges[i].angle;
                count++;
            }
        }

        // calculate average
        avgAngle /= count;
        backShoulderAngleFluctuate = 0;
        for (int i = 0; i < backShoulderAngleChanges.Count; i++)
        {
            if (Mathf.Abs(backShoulderAngleChanges[i].time - releaseTime) < fluctuateTime)
                backShoulderAngleFluctuate += Mathf.Abs(avgAngle - backShoulderAngleChanges[i].angle);
        }
        backShoulderAngleFluctuate /= count;

        // get start, end points
        backShoulderAngleStart = backShoulderAngleChanges[startIdx].angle;
        backShoulderAngleEnd = backShoulderAngleChanges[endIdx].angle;
    }

    #endregion

    #region display
    void displayScore()
    {
        PDM.hideAnnotations();
        // display score
        scoreDisplayPanel.SetActive(true);

        // hardcode score range
        float minFrontWristFluctuate = 8f;
        float maxFrontWristFluctuate = 200f;

        float minBackWristFluctuate = 60f;
        float maxBackWristFluctuate = 150f;

        float minFrontElbowAngleFluctuate = 0f;
        float maxFrontElbowAngleFluctuate = 10f;

        float minBackElbowAngleFluctuate = 0f;
        float maxBackElbowAngleFluctuate = 20f;

        float minFrontShoulderAngleFluctuate = 0f;
        float maxFrontShoulderAngleFluctuate = 10f;

        float minBackShoulderAngleFluctuate = 0f;
        float maxBackShoulderAngleFluctuate = 10f;

        float frontWristRank = (frontWristFluctuate - minFrontWristFluctuate) / (maxFrontWristFluctuate - minFrontWristFluctuate) * 100;
        float backWristRank = (backWristFluctuate - minBackWristFluctuate) / (maxBackWristFluctuate - minBackWristFluctuate) * 100;
        float frontElbowAngleRank = (frontElbowAngleFluctuate - minFrontElbowAngleFluctuate) / (maxFrontElbowAngleFluctuate - minFrontElbowAngleFluctuate) * 100;
        float backElbowAngleRank = (backElbowAngleFluctuate - minBackElbowAngleFluctuate) / (maxBackElbowAngleFluctuate - minBackElbowAngleFluctuate) * 100;
        float frontShoulderAngleRank = (frontShoulderAngleFluctuate - minFrontShoulderAngleFluctuate) / (maxFrontShoulderAngleFluctuate - minFrontShoulderAngleFluctuate) * 100;
        float backShoulderAngleRank = (backShoulderAngleFluctuate - minBackShoulderAngleFluctuate) / (maxBackShoulderAngleFluctuate - minBackShoulderAngleFluctuate) * 100;

        Debug.Log($"front Wrist: {frontWristFluctuate}, ({minFrontWristFluctuate}, {maxFrontWristFluctuate}),  {frontWristRank}");
        Debug.Log($"back Wrist: {backWristFluctuate}, ({minBackWristFluctuate}, {maxBackWristFluctuate}),  {backWristRank}");
        Debug.Log($"front Elbow: {frontElbowAngleFluctuate}, ({minFrontElbowAngleFluctuate}, {maxFrontElbowAngleFluctuate}),  {frontElbowAngleRank}");
        Debug.Log($"back Elbow: {backElbowAngleFluctuate}, ({minBackElbowAngleFluctuate}, {maxBackElbowAngleFluctuate}),  {backElbowAngleRank}");
        Debug.Log($"front Shoudler: {frontShoulderAngleFluctuate}, ({minFrontShoulderAngleFluctuate}, {maxFrontShoulderAngleFluctuate}),  {frontShoulderAngleRank}");
        Debug.Log($"back Shoulder: {backShoulderAngleFluctuate}, ({minBackShoulderAngleFluctuate}, {maxBackShoulderAngleFluctuate}),  {backShoulderAngleRank}");

        scoreDisplayText.text = "";
        scoreDisplayText.text += "\n\nfront Wrist Fluctuate\n";
        scoreDisplayText.text += scoreToRank(frontWristRank);
        scoreDisplayText.text += "\n\nback Wrist Fluctuate\n";

        scoreDisplayText.text += scoreToRank(backWristRank);
        scoreDisplayText.text += "\n\nfront Elbow Angle Fluctuate\n";
        scoreDisplayText.text += scoreToRank(frontElbowAngleRank);
        scoreDisplayText.text += "\n\nback Elbow Angle Fluctuate\n";
        scoreDisplayText.text += scoreToRank(backElbowAngleRank);

        scoreDisplayText.text += "\n\nfront Shoulder Angle Fluctuate\n";
        scoreDisplayText.text += scoreToRank(frontShoulderAngleRank);
        scoreDisplayText.text += "\n\nback Shoulder Angle Fluctuate\n";
        scoreDisplayText.text += scoreToRank(backShoulderAngleRank);

    }
    string scoreToRank(float score)
    {
        if (score < 10) return "Perfect";
        if (score < 30) return "Excellent";
        if (score < 50) return "Very Good";
        if (score < 70) return "Good";
        if (score < 90) return "Fair";
        return "Poor";
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
        PostureData postureData = new PostureData
        {
            dateTime = System.DateTime.Now,
            postureName = "Posture_" + (DataManager.instance.postureDataList != null ? DataManager.instance.postureDataList.Count : 0),

            // front wrist
            frontWristFluctuate = this.frontWristFluctuate,
            frontWristStart = this.frontWristStart,
            frontWristEnd = this.frontWristEnd,
            // back wrist
            backWristFluctuate = this.backWristFluctuate,
            backWristStart = this.backWristStart,
            backWristEnd = this.backWristEnd,
            // front elbow
            frontElbowAngleFluctuate = this.frontElbowAngleFluctuate,
            frontElbowAngleStart = this.frontElbowAngleStart,
            frontElbowAngleEnd = this.frontElbowAngleEnd,
            // back elbow
            backElbowAngleFluctuate = this.backElbowAngleFluctuate,
            backElbowAngleStart = this.backElbowAngleStart,
            backElbowAngleEnd = this.backElbowAngleEnd,
            // front shoulder
            frontShoulderAngleFluctuate = this.frontShoulderAngleFluctuate,
            frontShoulderAngleStart = this.frontShoulderAngleStart,
            frontShoulderAngleEnd = this.frontShoulderAngleEnd,
            // back shoulder
            backShoulderAngleFluctuate = this.backShoulderAngleFluctuate,
            backShoulderAngleStart = this.backShoulderAngleStart,
            backShoulderAngleEnd = this.backShoulderAngleEnd,

            frontWristPts = this.frontWristPts,
            backWristPts = this.backWristPts,
        };
        DataManager.instance.postureDataList.Add(postureData);
        DataManager.instance.SavePostureDataToFile();

        stopScoring();
    }

    #endregion

    #region debug
    IEnumerator autoSave()
    {
        yield return new WaitForSeconds(0.1f);
        saveScore();
        yield return new WaitForSeconds(1f);
        startScoring();
    }

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

public struct TimedPos { public float time; public Vector3 pos; }
