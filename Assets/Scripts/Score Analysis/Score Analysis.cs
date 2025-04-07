using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreAnalysis : MonoBehaviour
{
    [SerializeField] ScoreAnalysisFilterManager filterManager;

    [Header("setting")]
    [SerializeField] List<ScoreNote> targetScoreNotes = new();
    [SerializeField] List<PostureData> postureDataList = new();

    [Header("data")]
    [SerializeField] List<int> scoreRawList = new();
    [SerializeField] List<float> scorePercentageList = new();

    [Header("UI")]
    [SerializeField] GameObject popUpPanel;
    [SerializeField] TextMeshProUGUI popUpText;

    [Header("Score bar chart")]
    [SerializeField] RectTransform barContainer;
    [SerializeField] List<BarchartBarObj> scoreBars;

    [Header("Score Line")]
    [SerializeField] RectTransform scoreLineContainer;
    [SerializeField] GameObject scoreLineScrollContainer;
    [SerializeField] UILineRenderer scoreUILine;
    [SerializeField] TextMeshProUGUI vertiMaxText;
    [SerializeField] TextMeshProUGUI vertiMidText;
    [SerializeField] TextMeshProUGUI vertiMinText;
    [SerializeField] List<GameObject> score_DateObjList = new();

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
    [SerializeField] List<GameObject> posture_dateObjList = new();
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

    void OnEnable()
    {
        targetScoreNotes = DataManager.instance.scoreNoteList;
        makeScoreAnalysis();

        postureDataList = DataManager.instance.postureDataList;
        makePostureAnalysis();

        closePopUp();
    }

    #region score analysis
    void makeScoreAnalysis()
    {
        processScoreNote();
        StartCoroutine(updateBarchart_ScoreDistribution());
        updateGrouping();
        StartCoroutine(updateLineChart_Score());
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
    IEnumerator updateBarchart_ScoreDistribution()
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
            scoreBars[i].changeValue(scorePercentageList[i].ToString("F1") + "%");

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

    // update line chart display
    IEnumerator updateLineChart_Score()
    {
        yield return new WaitForEndOfFrame();

        // overall line chart
        float horizontalSpacing = 16;
        float bottomOffset = 48;
        float maxHeight = scoreLineContainer.rect.height - bottomOffset;
        float maxWidth = scoreLineContainer.rect.width;

        // draw main line
        float prevX = 0;
        DateTime prevTime = targetScoreNotes.Count == 0 ? default : targetScoreNotes[0].timestamp;
        DateTime day = default;
        List<Vector2> points = new();
        score_DateObjList.ForEach(obj => Destroy(obj));
        score_DateObjList.Clear();

        // offset for less data in the same day
        float offset = 0;
        int dataCount = 0;
        int minimumDataCount = 5;

        // find range
        float minScore = 10000;
        float maxScore = 0;
        for (int i = 0; i < targetScoreNotes.Count; i++)
        {
            float score = targetScoreNotes[i].getScore();
            if (score < minScore)
                minScore = score;
            if (score > maxScore)
                maxScore = score;
        }

        vertiMaxText.text = maxScore.ToString("F0");
        vertiMidText.text = ((maxScore + minScore) / 2).ToString("F0");
        vertiMinText.text = minScore.ToString("F0");

        for (int i = 0; i < targetScoreNotes.Count; i++)
        {
            if (targetScoreNotes[i].timestamp.Date != day)
            {
                if (i != 0 && dataCount < minimumDataCount)
                    offset += minimumDataCount - dataCount;
                dataCount = 0;
            }
            dataCount++;

            prevX = (i + offset) * maxWidth / LINE_MAX_POINT + (i == 0 ? horizontalSpacing : 0);
            points.Add(new Vector2(
                prevX,
                (targetScoreNotes[i].getScore() - minScore) /
                    (maxScore - minScore) * maxHeight + bottomOffset
            ));
            // draw date text
            if (dataCount == 1)
            {
                day = targetScoreNotes[i].timestamp.Date;
                GameObject dateObj = Instantiate(dateTextPrefab, scoreLineScrollContainer.transform);
                dateObj.GetComponent<TextMeshProUGUI>().text = day.ToString("dd/MM");
                dateObj.GetComponent<RectTransform>().localPosition = new Vector3(prevX, scoreLineScrollContainer.GetComponent<RectTransform>().rect.min.y, -1);
                score_DateObjList.Add(dateObj);
            }
        }

        scoreUILine.gameObject.SetActive(false);
        scoreUILine.points = points.ToArray();
        yield return new WaitForEndOfFrame();
        scoreUILine.gameObject.SetActive(true);
        scoreLineScrollContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(prevX + 4 * maxWidth / LINE_MAX_POINT,
                                                                            scoreLineScrollContainer.GetComponent<RectTransform>().sizeDelta.y);

        yield return new WaitForEndOfFrame();
    }

    #endregion

    #region posture analysis
    void makePostureAnalysis()
    {
        clearPostureData();
        processPostureData();
        StartCoroutine(updateLineChart_Posture());
    }

    // get useful data from posture data
    void processPostureData()
    {
        // turn raw data into percentage
        foreach (PostureData d in postureDataList)
        {
            postureDataDict["Overall"].Add(new() { time = d.dateTime, score = 100 - d.scores[0] });
            postureDataDict["FrontWrist"].Add(new() { time = d.dateTime, score = 100 - d.scores[1] });
            postureDataDict["BackWrist"].Add(new() { time = d.dateTime, score = 100 - d.scores[2] });
            postureDataDict["FrontElbow"].Add(new() { time = d.dateTime, score = 100 - d.scores[3] });
            postureDataDict["BackElbow"].Add(new() { time = d.dateTime, score = 100 - d.scores[4] });
            postureDataDict["FrontShoulder"].Add(new() { time = d.dateTime, score = 100 - d.scores[5] });
            postureDataDict["BackShoulder"].Add(new() { time = d.dateTime, score = 100 - d.scores[6] });
        }

        // // sort data by time
        // foreach (string key in postureDataDict.Keys)
        //     postureDataDict[key].Sort((a, b) => a.time.CompareTo(b.time));

        // // find min, max score of each list
        // int i = 0;
        // foreach (string key in postureDataDict.Keys)
        // {
        //     foreach (lineChart_PostureData d in postureDataDict[key])
        //     {
        //         // min
        //         if (d.score < postureScoreRange[i][0])
        //             postureScoreRange[i][0] = d.score;
        //         // max
        //         if (d.score > postureScoreRange[i][1])
        //             postureScoreRange[i][1] = d.score;
        //     }
        //     i++;
        // }

        // only extract overall score
        // sort data by time
        postureDataDict["Overall"].Sort((a, b) => a.time.CompareTo(b.time));

        // find min, max score of each list
        foreach (lineChart_PostureData d in postureDataDict["Overall"])
        {
            // min
            if (d.score < postureScoreRange[0][0])
                postureScoreRange[0][0] = d.score;
            // max
            if (d.score > postureScoreRange[0][1])
                postureScoreRange[0][1] = d.score;
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
    IEnumerator updateLineChart_Posture()
    {
        // overall line chart
        float horizontalSpacing = 16;
        float bottomOffset = 48;
        float maxHeight = postureContainer_Overall.rect.height - bottomOffset;
        float maxWidth = postureContainer_Overall.rect.width;

        // draw main line
        float prevX = 0;
        DateTime prevTime = postureDataDict["Overall"].Count == 0 ? default : postureDataDict["Overall"][0].time;
        DateTime day = default;
        List<Vector2> points = new();
        posture_dateObjList.ForEach(obj => Destroy(obj));
        posture_dateObjList.Clear();

        // offset for less data in the same day
        float offset = 0;
        int dataCount = 0;
        int minimumDataCount = 5;

        for (int i = 0; i < postureDataDict["Overall"].Count; i++)
        {
            if (postureDataDict["Overall"][i].time.Date != day)
            {
                if (i != 0 && dataCount < minimumDataCount)
                    offset += minimumDataCount - dataCount;
                dataCount = 0;
            }
            dataCount++;

            prevX = (i + offset) * maxWidth / LINE_MAX_POINT + (i == 0 ? horizontalSpacing : 0);
            points.Add(new Vector2(
                prevX,
                (postureDataDict["Overall"][i].score - postureScoreRange[0][0]) /
                    (postureScoreRange[0][1] - postureScoreRange[0][0]) * maxHeight +
                    bottomOffset
            ));
            // draw date text
            if (dataCount == 1)
            {
                day = postureDataDict["Overall"][i].time.Date;
                GameObject dateObj = Instantiate(dateTextPrefab, LineScrollContainer.transform);
                dateObj.GetComponent<TextMeshProUGUI>().text = day.ToString("dd/MM");
                dateObj.GetComponent<RectTransform>().localPosition = new Vector3(prevX, LineScrollContainer.GetComponent<RectTransform>().rect.min.y, -1);
                posture_dateObjList.Add(dateObj);
            }
        }

        overallUILine.gameObject.SetActive(false);
        overallUILine.points = points.ToArray();
        yield return new WaitForEndOfFrame();
        overallUILine.gameObject.SetActive(true);
        LineScrollContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(prevX + 4 * maxWidth / LINE_MAX_POINT,
                                                                            LineScrollContainer.GetComponent<RectTransform>().sizeDelta.y);

        // draw lvl indicator lines
        foreach (GameObject obj in lvlIndicatorLines_Overall)
            obj.SetActive(false);

        if (postureDataDict["Overall"].Count == 0)
            yield break;

        ArcherLvl minLvl = ArcherLvl.Advanced;
        ArcherLvl maxLvl = ArcherLvl.Beginner;
        // find min lvl
        if (postureScoreRange[0][0] < PostureScoreUtils.instance.absoulteScore_Beginner)
            minLvl = ArcherLvl.Beginner;
        else if (postureScoreRange[0][0] < PostureScoreUtils.instance.absoulteScore_Elementary)
            minLvl = ArcherLvl.Elementary;
        else if (postureScoreRange[0][0] < PostureScoreUtils.instance.absoulteScore_Intermidate)
            minLvl = ArcherLvl.Intermidate;

        // find max lvl
        if (postureScoreRange[0][1] >= PostureScoreUtils.instance.absoulteScore_Advanced)
            maxLvl = ArcherLvl.Advanced;
        else if (postureScoreRange[0][1] >= PostureScoreUtils.instance.absoulteScore_Intermidate)
            maxLvl = ArcherLvl.Intermidate;
        else if (postureScoreRange[0][1] >= PostureScoreUtils.instance.absoulteScore_Elementary)
            maxLvl = ArcherLvl.Elementary;

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
        float tempY = (targetScore - postureScoreRange[0][0]) / (postureScoreRange[0][1] - postureScoreRange[0][0]) * maxHeight + maxHeight * 0.05f - maxHeight / 2;
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
        tempY = (targetScore - postureScoreRange[0][0]) / (postureScoreRange[0][1] - postureScoreRange[0][0]) * maxHeight + maxHeight * 0.05f - maxHeight / 2;
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

    #region filter
    public void loadDataFilter()
    {
        FilterData filterData = filterManager.loadDataFilter();
        DateTime dateFrom = filterData.dateFrom;
        DateTime dateTo = filterData.dateTo;
        int recordType = filterData.recordType;
        int distance = filterData.distance;

        int[] distanceChoice = { 18, 30, 50, 70, 90 };

        // filter data
        targetScoreNotes = DataManager.instance.scoreNoteList.FindAll(d => d.timestamp >= dateFrom && d.timestamp <= dateTo);
        if (recordType != -1 && targetScoreNotes.Count > 0)
            targetScoreNotes = targetScoreNotes.FindAll(d => d.recordType == (RecordType)recordType);
        if (distance != -1 && targetScoreNotes.Count > 0)
            targetScoreNotes = targetScoreNotes.FindAll(d => d.distance == distanceChoice[distance]);
        if (targetScoreNotes.Count > 0)
            makeScoreAnalysis();
        else
        {
            popUpText.text += "No score note found!\n";
        }

        postureDataList = DataManager.instance.postureDataList.FindAll(d => d.dateTime >= dateFrom && d.dateTime <= dateTo);
        if (postureDataList.Count > 0)
            makePostureAnalysis();
        else
        {
            popUpText.text += "No posture record found!\n";
        }

        // show pop up
        if (popUpText.text != "")
        {
            popUpText.text += "Please adjust the filters!";
            popUpPanel.SetActive(true);
        }
    }

    public void closePopUp()
    {
        popUpPanel.SetActive(false);
        popUpText.text = "";
    }

    #endregion
}

struct lineChart_PostureData
{
    public DateTime time;
    public float score;
}