using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreAnalysis : MonoBehaviour
{
    [Header("setting")]
    [SerializeField] List<ScoreNote> targetScoreNotes = new();
    [SerializeField] List<PostureData> postureDataList = new();

    [Header("data")]
    [SerializeField] List<int> scoreRawList = new();
    [SerializeField] List<float> scorePercentageList = new();

    [Header("bar chart")]
    [SerializeField] RectTransform barContainer;
    [SerializeField] List<BarchartBarObj> scoreBars;

    [Header("Grouping")]
    [SerializeField] Image groupingTargetImg;
    [SerializeField] List<GameObject> groupingIndicators;

    [Header("Posture Line")]
    [SerializeField] RectTransform postureContainer_Overall;
    // [SerializeField] RectTransform postureContainer_Front;
    // [SerializeField] RectTransform postureContainer_Back;
    [SerializeField] GameObject LineScrollContainer;
    [SerializeField] UILineRenderer overallUILine;
    [SerializeField] GameObject dateTextPrefab;
    [SerializeField] List<GameObject> dateObjList = new();
    [SerializeField] List<GameObject> lvlIndicatorLines_Overall;

    const int LINE_MAX_POINT = 36;
    Dictionary<string, List<lineChart_PostureData>> postureDataDict = new() {
        { "Overall", new() },
        { "FrontWrist", new() },
        { "FrontElbow", new() },
        { "FrontShoulder", new() },
        { "BackWrist", new() },
        { "BackElbow", new() },
        { "BackShoulder", new() }
    };
    List<List<float>> postureScoreRange = new() {
        new() { 100, 0 },
        new() { 100, 0 },
        new() { 100, 0 },
        new() { 100, 0 },
        new() { 100, 0 },
        new() { 100, 0 },
        new() { 100, 0 }
    };


    // todo add filter

    void OnEnable()
    {
        targetScoreNotes = DataManager.instance.scoreNoteList;
        makeScoreAnalysis();

        postureDataList = DataManager.instance.postureDataList;
        makePostureAnalysis();
    }

    #region score analysis
    void makeScoreAnalysis()
    {
        processScoreNote();
        StartCoroutine(updateBarchart());
        updateGrouping();
    }

    // calculate statistics from score notes
    void processScoreNote()
    {
        // collect scores
        scoreRawList = new() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        int total = 0;
        foreach (ScoreNote scoreNote in targetScoreNotes)
            foreach (List<ArrowRecord> tempList in scoreNote.records)
                foreach (ArrowRecord arrowRecord in tempList)
                    if (arrowRecord.score != -1)
                    {
                        scoreRawList[arrowRecord.score]++;
                        total++;
                    }

        // calculate the percentage
        scorePercentageList = new() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        for (int i = 0; i < 12; i++)
            scorePercentageList[i] = (float)scoreRawList[i] / total * 100;
    }

    // update bar chart display
    IEnumerator updateBarchart()
    {
        float maxHeight = barContainer.rect.height
            - scoreBars[0].gameObject.GetComponent<VerticalLayoutGroup>().spacing
            - scoreBars[0].labelText.gameObject.GetComponent<RectTransform>().sizeDelta.y
            - scoreBars[0].valueText.gameObject.GetComponent<RectTransform>().sizeDelta.y;

        float maxPercentage = 0;
        foreach (float percentage in scorePercentageList)
            if (percentage > maxPercentage)
                maxPercentage = percentage;

        for (int i = 0; i < scoreRawList.Count; i++)
        {
            // change value
            scoreBars[i].changeValue(scorePercentageList[i].ToString("F2") + "%");

            // calculate bar height
            scoreBars[i].changeBarHeight(maxHeight * scorePercentageList[i] / maxPercentage);
        }

        yield return new WaitForEndOfFrame();
        LayoutRebuilder.ForceRebuildLayoutImmediate(barContainer);
    }

    // update grouping display (100 latest arrows)
    void updateGrouping()
    {
        // hide all dots
        foreach (GameObject obj in groupingIndicators)
            obj.SetActive(false);

        int total = 0;
        for (int i = targetScoreNotes.Count - 1; i >= 0; i--)
        {
            ScoreNote scoreNote = targetScoreNotes[i];
            for (int j = scoreNote.records.Count - 1; j >= 0; j--)
            {
                List<ArrowRecord> tempList = scoreNote.records[j];
                for (int k = tempList.Count - 1; k >= 0; k--)
                {
                    ArrowRecord arrowRecord = tempList[k];
                    if (arrowRecord.landPos != null && arrowRecord.landPos != default)
                    {
                        foreach (GameObject obj in groupingIndicators)
                            if (!obj.activeSelf)
                            {
                                obj.transform.localPosition = new Vector2(
                                    arrowRecord.landPos[0] * groupingTargetImg.rectTransform.rect.height / 2,
                                    arrowRecord.landPos[1] * groupingTargetImg.rectTransform.rect.height / 2
                                    );
                                obj.SetActive(true);
                                break;
                            }
                        total++;
                        if (total >= 100)
                            return;
                    }
                }
            }
        }
    }

    #endregion

    #region posture analysis
    void makePostureAnalysis()
    {
        clearPostureData();
        processPostureData();
        StartCoroutine(updateLineChart());
    }

    // get useful data from posture data
    void processPostureData()
    {
        Debug.Log("postureDataList.Count: " + postureDataList.Count);
        // turn raw data into percentage
        foreach (PostureData d in postureDataList)
        {
            List<float> scorePercent = PostureScoreUtils.instance.adjustedScore(
                new List<float> {
                    d.frontWristFluctuate,
                    d.backWristFluctuate,
                    d.frontElbowAngleFluctuate,
                    d.backElbowAngleFluctuate,
                    d.frontShoulderAngleFluctuate,
                    d.backShoulderAngleFluctuate,
                    0});

            postureDataDict["FrontWrist"].Add(new() { time = d.dateTime, score = 100 - scorePercent[0] });
            postureDataDict["BackWrist"].Add(new() { time = d.dateTime, score = 100 - scorePercent[1] });
            postureDataDict["FrontElbow"].Add(new() { time = d.dateTime, score = 100 - scorePercent[2] });
            postureDataDict["BackElbow"].Add(new() { time = d.dateTime, score = 100 - scorePercent[3] });
            postureDataDict["FrontShoulder"].Add(new() { time = d.dateTime, score = 100 - scorePercent[4] });
            postureDataDict["BackShoulder"].Add(new() { time = d.dateTime, score = 100 - scorePercent[5] });
            postureDataDict["Overall"].Add(new() { time = d.dateTime, score = 100 - scorePercent[6] });
        }

        // sort data by time
        foreach (string key in postureDataDict.Keys)
            postureDataDict[key].Sort((a, b) => a.time.CompareTo(b.time));

        // find min, max score of each list
        int i = 0;
        foreach (string key in postureDataDict.Keys)
        {
            foreach (lineChart_PostureData d in postureDataDict[key])
            {
                // min
                if (d.score < postureScoreRange[i][0])
                    postureScoreRange[i][0] = d.score;
                // max
                if (d.score > postureScoreRange[i][1])
                    postureScoreRange[i][1] = d.score;
            }
            i++;
        }
    }

    // reset list data
    void clearPostureData()
    {
        foreach (string key in postureDataDict.Keys)
            postureDataDict[key].Clear();
        postureDataDict.Clear();

        postureDataDict = new() {
            { "Overall", new() },
            { "FrontWrist", new() },
            { "FrontElbow", new() },
            { "FrontShoulder", new() },
            { "BackWrist", new() },
            { "BackElbow", new() },
            { "BackShoulder", new() }
        };

        foreach (var k in postureScoreRange)
            k.Clear();
        postureScoreRange.Clear();

        postureScoreRange = new() {
            new() { 100, 0 },
            new() { 100, 0 },
            new() { 100, 0 },
            new() { 100, 0 },
            new() { 100, 0 },
            new() { 100, 0 },
            new() { 100, 0 }
        };
    }

    // update line chart display
    IEnumerator updateLineChart()
    {
        // overall line chart
        float horizontalSpacing = 16;
        float bottomOffset = 48;
        float maxHeight = postureContainer_Overall.rect.height - bottomOffset;
        float maxWidth = postureContainer_Overall.rect.width;

        // draw main line
        float prevX = 0;
        DateTime prevTime = postureDataDict["Overall"][0].time;
        DateTime day = default;
        List<Vector2> points = new();
        dateObjList.ForEach(obj => Destroy(obj));
        dateObjList.Clear();
        for (int i = 0; i < postureDataDict["Overall"].Count; i++)
        {
            if (postureDataDict["Overall"][i].time.Date != day)
            {
                day = postureDataDict["Overall"][i].time.Date;
                GameObject dateObj = Instantiate(dateTextPrefab, LineScrollContainer.transform);
                dateObj.GetComponent<TextMeshProUGUI>().text = day.ToString("dd/MM");
                dateObj.GetComponent<RectTransform>().localPosition = new Vector3(prevX, LineScrollContainer.GetComponent<RectTransform>().rect.min.y, -1);
                dateObjList.Add(dateObj);
            }

            prevX = i * maxWidth / LINE_MAX_POINT + (i == 0 ? horizontalSpacing : 0);
            points.Add(new Vector2(
                prevX,
                (postureDataDict["Overall"][i].score - postureScoreRange[0][0]) /
                    (postureScoreRange[0][1] - postureScoreRange[0][0]) * maxHeight +
                    bottomOffset
            ));
        }
        overallUILine.gameObject.SetActive(false);
        overallUILine.points = points.ToArray();
        yield return new WaitForEndOfFrame();
        overallUILine.gameObject.SetActive(true);
        LineScrollContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(prevX + horizontalSpacing, 
                                                                            LineScrollContainer.GetComponent<RectTransform>().sizeDelta.y);


        // draw lvl indicator lines
        foreach (GameObject obj in lvlIndicatorLines_Overall)
            obj.SetActive(false);

        ArcherLvl minLvl = ArcherLvl.Beginner;
        ArcherLvl maxLvl = ArcherLvl.Beginner;
        // find min lvl
        if (postureScoreRange[0][0] < PostureScoreUtils.instance.absoulteScore_Beginner)
            minLvl = ArcherLvl.Beginner;
        else if (postureScoreRange[0][0] < PostureScoreUtils.instance.absoulteScore_Elementary)
            minLvl = ArcherLvl.Elementary;
        else if (postureScoreRange[0][0] < PostureScoreUtils.instance.absoulteScore_Intermidate)
            minLvl = ArcherLvl.Intermidate;
        else if (postureScoreRange[0][0] < PostureScoreUtils.instance.absoulteScore_Advanced)
            minLvl = ArcherLvl.Advanced;
        // find max lvl
        if (postureScoreRange[0][1] >= PostureScoreUtils.instance.absoulteScore_Advanced)
            maxLvl = ArcherLvl.Advanced;
        else if (postureScoreRange[0][1] >= PostureScoreUtils.instance.absoulteScore_Intermidate)
            maxLvl = ArcherLvl.Intermidate;
        else if (postureScoreRange[0][1] >= PostureScoreUtils.instance.absoulteScore_Elementary)
            maxLvl = ArcherLvl.Elementary;
        else if (postureScoreRange[0][1] >= PostureScoreUtils.instance.absoulteScore_Beginner)
            maxLvl = ArcherLvl.Beginner;
        Debug.Log("Overall: " + minLvl + " - " + maxLvl);

        float targetScore = 0;
        switch (minLvl)
        {
            case ArcherLvl.Beginner:
                targetScore = PostureScoreUtils.instance.absoulteScore_Beginner;
                break;
            case ArcherLvl.Elementary:
                targetScore = PostureScoreUtils.instance.absoulteScore_Elementary;
                break;
            case ArcherLvl.Intermidate:
                targetScore = PostureScoreUtils.instance.absoulteScore_Intermidate;
                break;
            case ArcherLvl.Advanced:
                targetScore = PostureScoreUtils.instance.absoulteScore_Advanced;
                break;
        }
        float tempY = (targetScore - postureScoreRange[0][0]) / (postureScoreRange[0][1] - postureScoreRange[0][0]) * maxHeight + maxHeight * 0.05f - maxHeight/2;
        lvlIndicatorLines_Overall[(int)minLvl].GetComponent<RectTransform>().localPosition = new Vector3(0, tempY, -2);
        switch (maxLvl)
        {
            case ArcherLvl.Beginner:
                targetScore = PostureScoreUtils.instance.absoulteScore_Beginner;
                break;
            case ArcherLvl.Elementary:
                targetScore = PostureScoreUtils.instance.absoulteScore_Elementary;
                break;
            case ArcherLvl.Intermidate:
                targetScore = PostureScoreUtils.instance.absoulteScore_Intermidate;
                break;
            case ArcherLvl.Advanced:
                targetScore = PostureScoreUtils.instance.absoulteScore_Advanced;
                break;
        }
        tempY = (targetScore - postureScoreRange[0][0]) / (postureScoreRange[0][1] - postureScoreRange[0][0]) * maxHeight + maxHeight * 0.05f - maxHeight/2;
        lvlIndicatorLines_Overall[(int)maxLvl].GetComponent<RectTransform>().localPosition = new Vector3(0, tempY, -2);

        lvlIndicatorLines_Overall[(int)minLvl].SetActive(true);
        lvlIndicatorLines_Overall[(int)maxLvl].SetActive(true);


        // front wrist line chart
        // todo

        // front elbow line chart
        // todo

        yield return new WaitForEndOfFrame();
    }

    #endregion

    public void loadDateRange(DateTime dateFrom, DateTime dateTo) {

        // filter data
        targetScoreNotes = DataManager.instance.scoreNoteList.FindAll(d => d.timestamp >= dateFrom && d.timestamp <= dateTo);
        if (targetScoreNotes.Count > 0)
            makeScoreAnalysis();

        postureDataList = DataManager.instance.postureDataList.FindAll(d => d.dateTime >= dateFrom && d.dateTime <= dateTo);
        if (postureDataList.Count > 0)
            makePostureAnalysis();
    }
}

struct lineChart_PostureData
{
    public DateTime time;
    public float score;
}