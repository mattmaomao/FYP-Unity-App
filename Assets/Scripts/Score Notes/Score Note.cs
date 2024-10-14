using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum RecordType { Competition, Practice, Other }

// one record of Round
[System.Serializable]
public class ScoreNote
{
    public float timestamp;
    public RecordType recordType;
    public List<List<int>> scores;
    public List<List<Vector2>> landPos;
    public int endArrow;    // 3, 6
    public int endNum;    // 6, 12
    public int roundNum = 2;

    // preset used? (competition type)
    // todo
    public int distance;

    // used equipment
    // todo

    public ScoreNote(float timestamp, RecordType recordType, int endArrow, int endNum, int roundNum, int distance, bool init = true)
    {
        this.timestamp = timestamp;
        this.recordType = recordType;
        this.endArrow = endArrow;
        this.endNum = endNum;
        this.roundNum = roundNum;
        this.distance = distance;

        if (init)
            initScores();
    }

    public void initScores()
    {
        // clear all scores
        if (scores != null) {
            foreach (List<int> temp in scores)
                temp.Clear();
            scores.Clear();
        }

        // create list of scores (-1)
        scores = new();
        for (int i = 0; i < endArrow / 3 * endNum; i++)
        {
            List<int> temp = new();
            for (int j = 0; j < 3; j++)
                temp.Add(-1);
            scores.Add(temp);
        }
    }

    public void addScore(int end, int arrow, int score)
    {
        scores[end][arrow] = score;
    }
    public void addScore(int score)
    {
        for (int i = 0; i < scores.Count; i++)
        {
            for (int j = 0; j < scores[i].Count; j++)
            {
                if (scores[i][j] == -1)
                {
                    scores[i][j] = score;
                    return;
                }
            }
        }
    }

}