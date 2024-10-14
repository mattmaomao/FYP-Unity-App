using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ScoreNotesManager : MonoBehaviour
{
    [SerializeField] GameObject scoreGrid;
    List<ScoreRow> scoreRows = new List<ScoreRow>();
    ScoreNote scoreNote;
    [SerializeField] GameObject arrowPrefab;
    [SerializeField] Transform arrowParent;

    void Start()
    {
        // debug
        scoreNote = new ScoreNote(0, RecordType.Practice, 6, 12, 2, 70);

        scoreRows = new();
        for (int i = 0; i < scoreGrid.transform.childCount; i++)
            scoreRows.Add(scoreGrid.transform.GetChild(i).GetComponent<ScoreRow>());

        setGrid();
    }

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
            int endTotal = 0;
            int notCount = 0;
            foreach (int score in scoreNote.scores[i])
            {
                if (score == -1)
                {
                    notCount++;
                    continue;
                }
                endTotal += Mathf.Clamp(score, 0, 10);
            }
            if (notCount == 3)
                break;
            cumTotal += endTotal;

            scoreRows[i].updateRow(scoreNote.scores[i], endTotal, cumTotal);
        }
    }

    public void nextEnd()
    {
        clearMark();
    }

    public void addMark(Vector2 pos)
    {
        // spawn a mark on the target, calculate the score then add
        GameObject arrow = Instantiate(arrowPrefab, arrowParent);
        arrow.transform.localPosition = pos;
    }
    void clearMark()
    {
        foreach (Transform child in arrowParent)
            Destroy(child.gameObject);

    }

    // add score to the current end
    public void addScore(int score)
    {
        scoreNote.addScore(score);
        updateGrid();
    }
    // remove previous input
    public void removeScore()
    {
        // todo
        Debug.Log("remove input");
        Debug.Log("not yet implemented");
    }
}
