using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class ScoreNotesManager : MonoBehaviour
{
    #region Singleton
    public static ScoreNotesManager instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        // DontDestroyOnLoad(gameObject);
        else
            Destroy(gameObject);
    }
    #endregion

    ScoreNote scoreNote;
    List<ScoreRow> scoreRows = new List<ScoreRow>();
    [SerializeField] GameObject rowParent;
    [SerializeField] GameObject rowPrefab;
    [SerializeField] List<GameObject> arrowKnobs;
    bool canAdd = true;
    [SerializeField] GameObject numPad;
    [SerializeField] GameObject numPad_hideBtn;
    [SerializeField] Vector2 selectedCell;

    void Start()
    {
        // debug
        scoreNote = new(
            timestamp: Time.time,
            title: "test",
            recordType: RecordType.Practice,
            distance: 18,
            targetType: 1,
            numOfRound: 2,
            numOfEnd: 12,
            arrowPerEnd: 6
        );

        initScoreNote(scoreNote);
    }

    public void initScoreNote(ScoreNote note)
    {
        scoreNote = note;

        scoreRows = new();
        for (int i = 0; i < scoreNote.arrowPerEnd / 3 * scoreNote.numOfEnd; i++)
        {
            GameObject row = Instantiate(rowPrefab, rowParent.transform);
            scoreRows.Add(row.GetComponent<ScoreRow>());
        }

        setGrid();
        hideNumPad();
    }


    #region display
    // init grid for score display
    void setGrid()
    {
        foreach (ScoreRow row in scoreRows)
            row.gameObject.SetActive(false);

        int totalEnds = scoreNote.arrowPerEnd / 3 * scoreNote.numOfEnd;
        if (totalEnds > scoreRows.Count)
            Debug.LogError("not enough rows");

        for (int i = 0; i < totalEnds; i++)
        {
            scoreRows[i].gameObject.SetActive(true);
            scoreRows[i].initRow(i, scoreNote.arrowPerEnd == 6);
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
            if (emptyRecord == scoreNote.arrowPerEnd)
            {
                scoreRows[i * (scoreNote.arrowPerEnd / 3)].updateRow(new() { -1, -1, -1 }, -1, -1);
                scoreRows[i * (scoreNote.arrowPerEnd / 3) + 1].updateRow(new() { -1, -1, -1 }, -1, -1);
                break;
            }

            scores.Sort();
            scores.Reverse();

            // update rows
            int subTotal = 0;
            for (int j = 0; j < 3; j++)
                subTotal += Mathf.Clamp(scores[j], 0, 10);
            cumTotal += subTotal;
            scoreRows[i * (scoreNote.arrowPerEnd / 3)].updateRow(scores.GetRange(0, 3), subTotal, scoreNote.arrowPerEnd == 3 ? cumTotal : -1);

            if (scoreNote.arrowPerEnd == 6)
            {
                subTotal = 0;
                for (int j = 3; j < 6; j++)
                    subTotal += Mathf.Clamp(scores[j], 0, 10);
                cumTotal += subTotal;
                scoreRows[i * (scoreNote.arrowPerEnd / 3) + 1].updateRow(scores.GetRange(3, 3), subTotal, cumTotal);
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
    #endregion

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

    #region UI btns
    // remove previous input
    public void removelastInput()
    {
        if (scoreNote.currentEnd() == 0 && scoreNote.currentArrowIdx() == 0) return;

        if (scoreNote.currentArrowIdx() == 0)
            scoreNote.records[scoreNote.currentEnd() - 1][scoreNote.arrowPerEnd - 1] = new ArrowRecord { score = -1, landPos = default };
        else
            scoreNote.records[scoreNote.currentEnd()][scoreNote.currentArrowIdx() - 1] = new ArrowRecord { score = -1, landPos = default };
        // remove last mark
        for (int i = arrowKnobs.Count - 1; i >= 0; i--)
        {
            if (arrowKnobs[i].activeSelf)
            {
                arrowKnobs[i].SetActive(false);
                break;
            }
        }

        canAdd = true;

        selectedCell = new(scoreNote.currentEnd(), scoreNote.currentArrowIdx());

        updateMark();
        updateGrid();
    }
    // move to the next end
    public void nextEnd()
    {
        if (!canAdd && scoreNote.currentArrowIdx() == 0)
        {
            clearMark();
            canAdd = true;

            selectedCell = new(scoreNote.currentEnd(), scoreNote.currentArrowIdx());
        }
    }
    #endregion

    #region NumPad
    public void showNumPad()
    {
        numPad.SetActive(true);
        numPad_hideBtn.SetActive(true);
    }
    public void hideNumPad()
    {
        numPad.SetActive(false);
        numPad_hideBtn.SetActive(false);
    }
    public void selectCell(int end, int arrow)
    {
        showNumPad();
        selectedCell = new(end, arrow);
    }
    public void inputNum(int num)
    {
        if (!canAdd) return;

        // calculate end, arrow idx from selected cell
        scoreNote.updateScore((int)selectedCell.x, (int)selectedCell.y, num, default);

        selectedCell = new(scoreNote.currentEnd(), scoreNote.currentArrowIdx());

        if (scoreNote.currentArrowIdx() == 0)
            canAdd = false;

        updateGrid();
    }
    #endregion

}