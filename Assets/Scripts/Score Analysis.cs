using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreAnalysis : MonoBehaviour
{
    [Header("setting")]
    [SerializeField] List<ScoreNote> targetScoreNotes;

    [Header("data")]
    [SerializeField] List<int> scoreRawList = new();
    [SerializeField] List<float> scorePercentageList = new();

    [Header("bar chart")]
    [SerializeField] RectTransform barContainer;
    [SerializeField] List<BarchartBarObj> scoreBars;

    [Header("Grouping")]
    [SerializeField] Image groupingTargetImg;
    [SerializeField] List<GameObject> groupingIndicators;


    void OnEnable()
    {
        targetScoreNotes = DataManager.instance.scoreNoteList;
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

}