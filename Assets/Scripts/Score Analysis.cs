using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class ScoreAnalysis : MonoBehaviour
{
    [Header("setting")]
    [SerializeField] GameObject temp;

    [Header("data")]
    [SerializeField] List<int> scoreRawList = new();
    [SerializeField] List<float> scorePercentageList = new();

    [Header("bar chart")]
    [SerializeField] RectTransform barContainer;
    [SerializeField] List<BarchartBarObj> scoreBars;

    void OnEnable()
    {
        processScoreNote(DataManager.instance.scoreNoteList);
        updateBarchart();
        updateGrouping();
    }

    // calculate statistics from score notes
    void processScoreNote(List<ScoreNote> scoreNoteList)
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

    // update bar chart display
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

    // update grouping display
    void updateGrouping() {
        // todo
        Debug.Log("update grouping");
        Debug.Log("not yet implemented");
    }
}