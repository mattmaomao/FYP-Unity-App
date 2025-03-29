using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PosetureScoring : MonoBehaviour
{
    struct TimedAngle { public float time; public float angle; }

    [SerializeField] PostureDetectionManager PDM;
    [SerializeField] PoseIdentifier poseIdentifier;
    bool isScoring = false;

    #region data collection
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

    #endregion

    #region rank calculation
    // rank
    float overallRank;
    float frontWristRank;
    float backWristRank;
    float frontElbowAngleRank;
    float backElbowAngleRank;
    float frontShoulderAngleRank;
    float backShoulderAngleRank;
    ArcherLvl archerLvl;

    #endregion

    [Header("UI")]
    [SerializeField] GameObject startBtn;
    [SerializeField] GameObject stopBtn;
    [SerializeField] GameObject startText;
    [SerializeField] GameObject recordingIndicator;
    [SerializeField] GameObject backBtn;
    [SerializeField] GameObject nextBtn;
    [SerializeField] GameObject lvlPanel;
    [SerializeField] GameObject resultDisplayPanel;
    // auto save
    [SerializeField] Toggle autoSaveToggle;
    bool autoSaving => autoSaveToggle.isOn;

    [Header("UI_Score")]
    [SerializeField] TextMeshProUGUI scoreDisplayText;
    [SerializeField] GameObject showDetailBtn;
    [SerializeField] GameObject detailedScorePanel;
    [SerializeField] TextMeshProUGUI showDetailBtnText;
    bool showingDetail = false;
    int scorePage = 0;
    [SerializeField] List<TextMeshProUGUI> detailedScoreTexts;
    string currentGrade = "";

    [Header("UI_Line")]
    // line
    [SerializeField] GameObject showLinePanel;
    [SerializeField] Transform lineContainer;
    Camera cameraToCapture;
    [SerializeField] Image lineBackground;
    [SerializeField] LineRenderer frontLine;
    [SerializeField] LineRenderer backLine;
    [SerializeField] GameObject cameraSettingBtn;

    [Header("UI_Suggestion")]
    [SerializeField] GameObject suggestionPanel;
    [SerializeField] TextMeshProUGUI suggestionText;

    [Header("Debug")]
    [SerializeField] GameObject debugPanel;
    [SerializeField] TextMeshProUGUI debugPanelText;
    [SerializeField] GameObject lineDotsPrefab;

    void Start()
    {
        // reset
        stopScoring();

        cameraToCapture = Camera.main;
    }

    void Update()
    {
        if (archerLvl == ArcherLvl.Null) PDM.hideAnnotations();

        // check if scoring
        if (!isScoring) return;

        if (!released && poseIdentifier.currentPose == Pose.Release)
        {
            onRelease();
        }
        if (poseIdentifier.currentPose != Pose.Idle && poseIdentifier.currentPose != Pose.Draw_Ready)
            recordJointsPos();
        else
        {
            clearRecord();
        }

        DebugTable();
    }

    void OnEnable()
    {
        stopScoring();
        archerLvl = ArcherLvl.Null;
        lvlPanel.SetActive(true);
    }
    void OnDisable()
    {
        stopScoring();
    }

    void onRelease()
    {
        released = true;
        releaseTime = timer;

        takeReleaseScreenShot();
    }


    // start btn
    public void startScoring()
    {
        clearRecord();
        hideDisplayScore();

        startBtn.SetActive(false);
        stopBtn.SetActive(true);
        startText.SetActive(false);
        recordingIndicator.SetActive(true);
        cameraSettingBtn.SetActive(false);

        poseIdentifier.resetPose();

        isScoring = true;
    }

    // stop btn
    public void stopScoring()
    {
        isScoring = false;
        startBtn.SetActive(true);
        stopBtn.SetActive(false);
        startText.SetActive(true);
        recordingIndicator.SetActive(false);
        cameraSettingBtn.SetActive(true);

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

        // draw dotted line
        drawLine_dot();

        // draw line
        drawLine();

        // make suggesiton 
        makeSuggestion();

        // auto save action
        if (autoSaving)
            StartCoroutine(autoSave());

        // rotate for mobile device
        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            lineContainer.transform.localRotation = Quaternion.Euler(0, 0, 90);
            frontLine.transform.localRotation = Quaternion.Euler(0, 0, 90);
            backLine.transform.localRotation = Quaternion.Euler(0, 0, 90);
        }
        else
        {
            lineContainer.transform.localRotation = Quaternion.Euler(0, 0, 0);
            frontLine.transform.localRotation = Quaternion.Euler(0, 0, 0);
            backLine.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
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
    // make suggestion
    void makeSuggestion()
    {
        // float tempTolerance = (backWristEnd.x - backWristStart.x) / 3;
        float tempTolerance = poseIdentifier.shoulderWidth / 4;
        bool frontWristUp = frontWristEnd.y - frontWristStart.y > tempTolerance;
        bool frontWristDown = frontWristEnd.y - frontWristStart.y < -tempTolerance;
        bool backWristUp = backWristEnd.y - backWristStart.y > tempTolerance;
        bool backWristDown = backWristEnd.y - backWristStart.y < -tempTolerance;

        suggestionText.text = "";
        // wrist swing
        if (frontWristUp && backWristUp)
            suggestionText.text += "Both of you wrists were swung upwards! \nThis may cause your arrow to go up.\n\n";
        else if (frontWristDown && backWristDown)
            suggestionText.text += "Both of you wrists were swung downwards! \nThis may cause your arrow to go down.\n\n";
        else if (frontWristUp && backWristDown)
            suggestionText.text += "Your front wrist was swung upwards and your back wrist was swung downwards! \nThis may cause your arrow to go up.\n\n";
        else if (frontWristDown && backWristUp)
            suggestionText.text += "Your front wrist was swung downwards and your back wrist was swung upwards! \nThis may cause your arrow to go down.\n\n";
        else if (frontWristUp)
            suggestionText.text += "Your front wrist was swung upwards! \nThis may cause your arrow to go up.\n\n";
        else if (frontWristDown)
            suggestionText.text += "Your front wrist was swung downwards! \nThis may cause your arrow to go down.\n\n";
        else if (backWristUp)
            suggestionText.text += "Your back wrist was swung upwards! \nThis may cause your arrow to go down.\n\n";
        else if (backWristDown)
            suggestionText.text += "Your back wrist was swung downwards! \nThis may cause your arrow to go up.\n\n";

        // fluctuate
        string temp = "";
        int c = 0;
        if (frontWristRank > 70)
        {
            temp += "front wrist, ";
            c++;
        }
        if (backWristRank > 70)
        {
            temp += "back wrist, ";
            c++;
        }
        if (frontElbowAngleRank > 70)
        {
            temp += "front elbow, ";
            c++;
        }
        if (backElbowAngleRank > 70)
        {
            temp += "back elbow, ";
            c++;
        }
        if (frontShoulderAngleRank > 70)
        {
            temp += "front shoulder, ";
            c++;
        }
        if (backShoulderAngleRank > 70)
        {
            temp += "back shoulder, ";
            c++;
        }

        if (temp != "")
        {
            temp = temp.Substring(0, temp.Length - 2);
            suggestionText.text += $"Your {temp} {(c > 1 ? "fluctuate" : "fluctuates")} too much! \nTry to be steady.\n\n";
        }
    }
    #endregion

    #region display
    void displayScore()
    {
        PDM.hideAnnotations();

        // display score
        changePage(0);

        // calculate rank
        List<float> newScore = PostureScoreUtils.instance.adjustedScore(
                new List<float> { 
                    frontWristFluctuate, 
                    backWristFluctuate, 
                    frontElbowAngleFluctuate, 
                    backElbowAngleFluctuate, 
                    frontShoulderAngleFluctuate, 
                    backShoulderAngleFluctuate, 
                    0 }, 
                (int) archerLvl);

        frontWristRank = newScore[0];
        backWristRank = newScore[1];
        frontElbowAngleRank = newScore[2];
        backElbowAngleRank = newScore[3];
        frontShoulderAngleRank = newScore[4];
        backShoulderAngleRank = newScore[5];
        overallRank = newScore[6];

        showSimpleScore();
        // play se
        currentGrade = scoreToRank(overallRank);
        switch (currentGrade) {
            case "<b><color=yellow>Perfect</color></b>":
                AudioManager.instance.PlaySE(AudioManager.instance.Perfect_voice);
                break;
            case "<b><color=red>Excellent</color></b>":
                AudioManager.instance.PlaySE(AudioManager.instance.Excellent_voice);
                break;
            case "<b><color=blue>Very Good</color></b>":
                AudioManager.instance.PlaySE(AudioManager.instance.VeryGood_voice);
                break;
            case "<b><color=black>Good</color></b>":
                AudioManager.instance.PlaySE(AudioManager.instance.Good_voice);
                break;
            case "<b><color=white>Fair</color></b>":
                AudioManager.instance.PlaySE(AudioManager.instance.Fair_voice);
                break;
            case "<b><color=green>Poor</color></b>":
                AudioManager.instance.PlaySE(AudioManager.instance.Poor_voice);
                break;
            default:
                break;
        }
    }
    void drawLine()
    {
        // draw line of last 1 sec
        if (SystemInfo.deviceType == DeviceType.Handheld) {
            frontLine.SetPosition(0, new Vector2(frontWristStart.x + lineContainer.localPosition.y/2, frontWristStart.y));
            frontLine.SetPosition(1, new Vector2(frontWristEnd.x + lineContainer.localPosition.y/2, frontWristEnd.y));
            backLine.SetPosition(0, new Vector2(backWristStart.x + lineContainer.localPosition.y/2, backWristStart.y));
            backLine.SetPosition(1, new Vector2(backWristEnd.x + lineContainer.localPosition.y/2, backWristEnd.y));
        }
        else {
            frontLine.SetPosition(0, new Vector2(frontWristStart.x, frontWristStart.y + lineContainer.localPosition.y/2));
            frontLine.SetPosition(1, new Vector2(frontWristEnd.x, frontWristEnd.y + lineContainer.localPosition.y/2));
            backLine.SetPosition(0, new Vector2(backWristStart.x, backWristStart.y + lineContainer.localPosition.y/2));
            backLine.SetPosition(1, new Vector2(backWristEnd.x, backWristEnd.y + lineContainer.localPosition.y/2));
        }
    }
    void drawLine_dot()
    {
        // clear old line
        foreach (Transform child in lineContainer)
            Destroy(child.gameObject);
        // draw line
        foreach (TimedPos tp in frontWristPts)
        {
            GameObject lineDots = Instantiate(lineDotsPrefab, lineContainer);
            if (SystemInfo.deviceType == DeviceType.Handheld)
                lineDots.transform.localPosition = new Vector3(tp.pos.x + lineContainer.localPosition.y/2, tp.pos.y, 0);
            else
                lineDots.transform.localPosition = new Vector3(tp.pos.x, tp.pos.y + lineContainer.localPosition.y/2, 0);
        }
        foreach (TimedPos tp in backWristPts)
        {
            GameObject lineDots = Instantiate(lineDotsPrefab, lineContainer);
            lineDots.GetComponent<Image>().color = Color.red;
            if (SystemInfo.deviceType == DeviceType.Handheld)
                lineDots.transform.localPosition = new Vector3(tp.pos.x + lineContainer.localPosition.y/2, tp.pos.y, 0);
            else
                lineDots.transform.localPosition = new Vector3(tp.pos.x, tp.pos.y + lineContainer.localPosition.y/2, 0);
        }
    }
    void showSimpleScore()
    {
        showingDetail = false;

        // display text
        scoreDisplayText.text = scoreToRank(overallRank);
    }
    void showDetailScore()
    {
        showingDetail = true;

        // display text
        detailedScoreTexts[0].text = scoreToRank(overallRank);

        detailedScoreTexts[1].text = scoreToRank(frontWristRank);
        detailedScoreTexts[2].text = scoreToRank(frontElbowAngleRank);
        detailedScoreTexts[3].text = scoreToRank(frontShoulderAngleRank);

        detailedScoreTexts[4].text = scoreToRank(backWristRank);
        detailedScoreTexts[5].text = scoreToRank(backElbowAngleRank);
        detailedScoreTexts[6].text = scoreToRank(backShoulderAngleRank);
    }
    string scoreToRank(float score)
    {
        if (score < 10) return "<b><color=yellow>Perfect</color></b>";
        if (score < 30) return "<b><color=red>Excellent</color></b>";
        if (score < 50) return "<b><color=blue>Very Good</color></b>";
        if (score < 70) return "<b><color=black>Good</color></b>";
        if (score < 90) return "<b><color=white>Fair</color></b>";
        return "<b><color=green>Poor</color></b>";
    }

    // take screenshot of the whole screen 
    void takeReleaseScreenShot()
    {
        // Create a RenderTexture to capture the camera's view
        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        cameraToCapture.targetTexture = renderTexture;
        cameraToCapture.Render();

        // Create a new Texture2D and read the RenderTexture data into it
        Texture2D screenTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        RenderTexture.active = renderTexture;
        screenTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenTexture.Apply();

        // Reset the camera's target texture
        cameraToCapture.targetTexture = null;
        RenderTexture.active = null;

        // Create a sprite from the captured texture
        Sprite sprite = Sprite.Create(screenTexture, new Rect(0, 0, screenTexture.width, screenTexture.height), new Vector2(0.5f, 0.5f));
        lineBackground.sprite = sprite;

        // set size
        RectTransform canvasTransform = GameObject.Find("Canvas").GetComponent<RectTransform>();

        lineBackground.GetComponent<RectTransform>().sizeDelta = canvasTransform.sizeDelta;
        lineBackground.GetComponent<RectTransform>().position = Vector3.zero;
        lineContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(canvasTransform.sizeDelta.x, canvasTransform.sizeDelta.y);
        lineContainer.GetComponent<RectTransform>().position = Vector3.zero;
        frontLine.GetComponent<RectTransform>().sizeDelta = new Vector2(canvasTransform.sizeDelta.x, canvasTransform.sizeDelta.y);
        frontLine.GetComponent<RectTransform>().position = new Vector3(0, 0, -1);
        backLine.GetComponent<RectTransform>().sizeDelta = new Vector2(canvasTransform.sizeDelta.x, canvasTransform.sizeDelta.y);
        backLine.GetComponent<RectTransform>().position = new Vector3(0, 0, -1);
    }

    void hideDisplayScore()
    {
        // hide score
        PDM.showAnnotations();
        resultDisplayPanel.SetActive(false);
    }

    #endregion

    #region UI
    public void chooseLvl(int lvl)
    {
        archerLvl = (ArcherLvl)lvl;
        lvlPanel.SetActive(false);
        PDM.showAnnotations();
    }
    // score detail btn
    public void toggleScoreDetail()
    {
        if (showingDetail)
            showSimpleScore();
        else
            showDetailScore();

        showDetailBtnText.text = showingDetail ? "Hide Detail" : "Show Detail";
        detailedScorePanel.SetActive(showingDetail);
    }
    // change page
    void changePage(int idx)
    {
        resultDisplayPanel.SetActive(true);
        scorePage = idx;
        switch (scorePage)
        {
            case 0:
                showLinePanel.SetActive(false);
                suggestionPanel.SetActive(false);
                showDetailBtn.SetActive(true);
                backBtn.SetActive(false);
                nextBtn.SetActive(true);
                frontLine.gameObject.SetActive(false);
                backLine.gameObject.SetActive(false);
                break;
            case 1:
                showLinePanel.SetActive(true);
                suggestionPanel.SetActive(false);
                showDetailBtn.SetActive(false);
                backBtn.SetActive(true);
                nextBtn.SetActive(true);
                frontLine.gameObject.SetActive(true);
                backLine.gameObject.SetActive(true);
                break;
            case 2:
                suggestionPanel.SetActive(true);
                showDetailBtn.SetActive(false);
                backBtn.SetActive(true);
                nextBtn.SetActive(false);
                frontLine.gameObject.SetActive(false);
                backLine.gameObject.SetActive(false);
                break;
        }
    }
    // back, next btn
    public void onBackBtn()
    {
        changePage(scorePage - 1);
    }
    public void onNextBtn()
    {
        changePage(scorePage + 1);
    }
    // cancel btn
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
            archerLvl = (int)archerLvl,

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

            // stop record continuous data
            // frontWristPts = this.frontWristPts,
            // backWristPts = this.backWristPts,
        };
        DataManager.instance.postureDataList.Add(postureData);
        DataManager.instance.SavePostureDataToFile();

        stopScoring();
    }

    #endregion

    #region debug
    IEnumerator autoSave()
    {
        yield return new WaitForSeconds(1f);
        saveScore();
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
