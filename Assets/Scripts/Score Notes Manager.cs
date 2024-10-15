using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScoreNotesManager : MonoBehaviour
{
    [SerializeField] GameObject scoreGrid;
    List<ScoreRow> scoreRows = new List<ScoreRow>();
    ScoreNote scoreNote;
    [SerializeField] GameObject arrowPrefab;
    [SerializeField] Transform arrowParent;
    [SerializeField] bool canAdd = true;

    void Start()
    {
        // debug
        scoreNote = new ScoreNote(0, RecordType.Practice, 6, 12, 2, 70);

        scoreRows = new();
        for (int i = 0; i < scoreGrid.transform.childCount; i++)
            scoreRows.Add(scoreGrid.transform.GetChild(i).GetComponent<ScoreRow>());

        setGrid();
    }

    // init grid for score display
    void setGrid()
    {
        foreach (ScoreRow row in scoreRows)
            row.gameObject.SetActive(false);

        int totalEnds = scoreNote.endArrow / 3 * scoreNote.endNum;
        if (totalEnds > scoreRows.Count)
            Debug.LogError("not enough rows");

        for (int i = 0; i < totalEnds; i++)
        {
            scoreRows[i].gameObject.SetActive(true);
            scoreRows[i].initRow((i + 1) * 3);
        }
    }
    void updateGrid()
    {
        int cumTotal = 0;
        for (int i = 0; i < scoreRows.Count; i++)
        {
            // check is whole end is empty
            if (scoreNote.records[i][0].score == -1) break;

            // sort scores
            List<int> scores = new();
            for (int j = 0; j < scoreNote.records[i].Count; j++)
                scores.Add(scoreNote.records[i][j].score);
            scores.Sort();
            scores.Reverse();

            // update rows
            int subTotal = 0;
            for (int j = 0; j < 3; j++)
                subTotal += Mathf.Clamp(scores[j], 0, 10);
            cumTotal += subTotal;
            scoreRows[i * (scoreNote.endArrow / 3)].updateRow(scores.GetRange(0, 3), subTotal, scoreNote.endArrow == 3 ? cumTotal : -1);

            if (scoreNote.endArrow == 6)
            {
                subTotal = 0;
                for (int j = 3; j < 6; j++)
                    subTotal += Mathf.Clamp(scores[j], 0, 10);
                cumTotal += subTotal;
                scoreRows[i * (scoreNote.endArrow / 3) + 1].updateRow(scores.GetRange(3, 3), subTotal, cumTotal);
            }
        }
    }

    // spawn a mark on the target
    void updateMark()
    {
        for (int i = 0; i < scoreNote.records[scoreNote.currentEnd()].Count; i++)
        {
            if (scoreNote.records[scoreNote.currentEnd()][i].score != -1
                && scoreNote.records[scoreNote.currentEnd()][i].landPos != default)
            {
                GameObject arrow = Instantiate(arrowPrefab, arrowParent);
                arrow.transform.localPosition = scoreNote.records[scoreNote.currentEnd()][i].landPos;
            }
        }
    }
    void clearMark()
    {
        foreach (Transform child in arrowParent)
            Destroy(child.gameObject);
    }

    // add score, land position to the current end      => for target click
    public void addScore(int score, Vector2 landPos)
    {
        if (!canAdd) return;

        scoreNote.addScore(score, landPos);
        updateMark();
        updateGrid();

        if (scoreNote.currentArrowIdx() == 0)
            canAdd = false;
    }
    // add score to the current end      => for number input
    public void addScore(int score)
    {
        addScore(score, default);
    }

    // remove previous input
    [ContextMenu("undo")]
    public void removelastInput()
    {
        if (scoreNote.currentArrowIdx() == 0)
            scoreNote.records[scoreNote.currentEnd()-1][scoreNote.endArrow-1] = new ArrowRecord { score = -1, landPos = default };
        else
            scoreNote.records[scoreNote.currentEnd()][scoreNote.currentArrowIdx()-1] = new ArrowRecord { score = -1, landPos = default };
        updateGrid();
        // remove last mark
        Destroy(arrowParent.GetChild(arrowParent.childCount - 1).gameObject);
        updateMark();

        canAdd = true;
    }
    // move to the next end
    [ContextMenu("next end")]
    public void nextEnd()
    {
        clearMark();
        canAdd = true;
    }
}
