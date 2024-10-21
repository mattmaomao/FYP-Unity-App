using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreAnalysis : MonoBehaviour
{
    // read list from save file
    public List<ScoreNote> scoreNoteList = new List<ScoreNote>();

    [Header("setting")]
    [SerializeField] GameObject temp;

    [Header("data")]
    [SerializeField] List<int> scoreRawList = new();
    [SerializeField] List<float> scorePercentageList = new();

    [Header("bar chart")]
    [SerializeField] RectTransform barContainer;
    [SerializeField] List<BarchartBarObj> scoreBars;

    void Start()
    {
        // debug temp
        scoreNoteList.Add(new ScoreNote(
            timestamp: System.DateTime.Now,
            title: "record 1",
            recordType: RecordType.Practice,
            distance: 18,
            targetType: TargetType.Ring10,
            numOfRound: 2,
            numOfEnd: 12,
            arrowPerEnd: 6
        ));
        scoreNoteList[0].initScores();
        for (int i = 0; i < scoreNoteList[0].numOfEnd * 0.75f; i++)
            for (int j = 0; j < scoreNoteList[0].arrowPerEnd; j++)
                scoreNoteList[0].updateScore(i, j, Random.Range(0, 12), default);

        processScoreNote();
        updateBarchart();
    }

    void processScoreNote()
    {
        // collect scores
        scoreRawList = new() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        int total = 0;
        foreach (ScoreNote scoreNote in scoreNoteList)
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

    void updateBarchart()
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
    }
}