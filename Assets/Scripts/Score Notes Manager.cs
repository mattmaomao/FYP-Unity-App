using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScoreNotesManager : MonoBehaviour
{
    List<ScoreRow> scoreRows = new List<ScoreRow>();
    [SerializeField] GameObject rowParent;
    [SerializeField] GameObject rowPrefab;
    ScoreNote scoreNote;
    [SerializeField] List<GameObject> arrowKnobs;
    [SerializeField] bool canAdd = true;

    void Start()
    {
        // debug
        scoreNote = new ScoreNote(0, RecordType.Practice, 6, 12, 2, 70);

        scoreRows = new();
        for (int i = 0; i < scoreNote.endArrow / 3 * scoreNote.endNum; i++)
        {
            GameObject row = Instantiate(rowPrefab, rowParent.transform);
            scoreRows.Add(row.GetComponent<ScoreRow>());
        }

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
        for (int i = 0; i < scoreNote.records.Count; i++)
        {
            int emptyRecord = 0;

            // sort scores
            List<int> scores = new();
            for (int j = 0; j < scoreNote.records[i].Count; j++)
            {
                if (scoreNote.records[i][j].score == -1)
                    emptyRecord++;
                scores.Add(scoreNote.records[i][j].score);
            }
            if (emptyRecord == scoreNote.endArrow)
            {
                scoreRows[i * (scoreNote.endArrow / 3)].updateRow(new() { -1, -1, -1 }, -1, -1);
                scoreRows[i * (scoreNote.endArrow / 3) + 1].updateRow(new() { -1, -1, -1 }, -1, -1);
                break;
            }

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
        clearMark();
        
        int end = canAdd ? scoreNote.currentEnd() : scoreNote.currentEnd() - 1;
        for (int i = 0; i < scoreNote.records[end].Count; i++)
        {
            if (scoreNote.records[end][i].score != -1
                && scoreNote.records[end][i].landPos != default)
            {
                foreach (GameObject obj in arrowKnobs)
                    if (!obj.activeSelf)
                    {
                        obj.transform.localPosition = scoreNote.records[end][i].landPos;
                        obj.SetActive(true);
                        break;
                    }
            }
        }
    }
    void clearMark()
    {
        foreach (GameObject obj in arrowKnobs)
            obj.SetActive(false);
    }

    // add score, land position to the current end      => for target click
    public void addScore(int score, Vector2 landPos)
    {
        if (!canAdd) return;

        scoreNote.addScore(score, landPos);
        if (scoreNote.currentArrowIdx() == 0)
            canAdd = false;

        updateMark();
        updateGrid();
    }
    // add score to the current end      => for number input
    public void addScore(int score)
    {
        addScore(score, default);
    }

    // remove previous input
    public void removelastInput()
    {
        if (scoreNote.currentEnd() == 0 && scoreNote.currentArrowIdx() == 0) return;

        if (scoreNote.currentArrowIdx() == 0)
            scoreNote.records[scoreNote.currentEnd() - 1][scoreNote.endArrow - 1] = new ArrowRecord { score = -1, landPos = default };
        else
            scoreNote.records[scoreNote.currentEnd()][scoreNote.currentArrowIdx() - 1] = new ArrowRecord { score = -1, landPos = default };
        updateGrid();
        // remove last mark
        for (int i = arrowKnobs.Count - 1; i >= 0; i--)
            if (arrowKnobs[i].activeSelf)
            {
                arrowKnobs[i].SetActive(false);
                break;
            }
        updateMark();

        canAdd = true;
    }
    // move to the next end
    public void nextEnd()
    {
        if (!canAdd && scoreNote.currentArrowIdx() == 0)
        {
            clearMark();
            canAdd = true;
        }
    }
}
