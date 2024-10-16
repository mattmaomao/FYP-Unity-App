using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RecordType { Competition, Practice, Other }

public struct ArrowRecord
{
    public int score;
    public Vector2 landPos;
}

// one record of Round
[System.Serializable]
public class ScoreNote
{
    public System.DateTime timestamp;
    public string title;
    public RecordType recordType;
    public int distance;    // 18, 30, 50, 70, 90
    public int targetType;    // 6, 10
    public int numOfRound;    // 1, 2
    public int numOfEnd;    // 6, 12
    public int arrowPerEnd;    // 3, 6

    // used equipment
    // todo

    public List<List<ArrowRecord>> records;

    public ScoreNote(System.DateTime timestamp, string title, RecordType recordType, int distance, int targetType, int numOfRound, int numOfEnd, int arrowPerEnd, bool init = true)
    {
        this.timestamp = timestamp;
        this.title = title;
        this.recordType = recordType;
        this.distance = distance;
        this.targetType = targetType;
        this.numOfRound = numOfRound;
        this.numOfEnd = numOfEnd;
        this.arrowPerEnd = arrowPerEnd;

        if (init)
            initScores();
    }

    public void initScores()
    {
        // clear all scores
        if (records != null)
        {
            foreach (List<ArrowRecord> temp in records)
                temp.Clear();
            records.Clear();
        }

        // create list of scores (-1, default)
        records = new();
        for (int i = 0; i < numOfEnd; i++)
        {
            List<ArrowRecord> temp = new();
            for (int j = 0; j < arrowPerEnd; j++)
                temp.Add(new ArrowRecord { score = -1, landPos = default });
            records.Add(temp);
        }
    }

    public void updateScore(int end, int arrow, int score, Vector2 landPos)
    {
        records[end][arrow] = new ArrowRecord { score = score, landPos = landPos };
    }
    public void addScore(int score, Vector2 landPos)
    {
        for (int i = 0; i < records.Count; i++)
        {
            for (int j = 0; j < records[i].Count; j++)
            {
                if (records[i][j].score == -1)
                {
                    records[i][j] = new ArrowRecord { score = score, landPos = landPos };
                    return;
                }
            }
        }
    }

    #region get Data
    public int currentEnd()
    {
        for (int i = 0; i < records.Count; i++)
            for (int j = 0; j < records[i].Count; j++)
                if (records[i][j].score == -1)
                    return i;

        return records.Count;
    }
    public int currentArrowIdx()
    {
        int end = currentEnd();
        if (end == numOfEnd)
            return 0;

        for (int i = 0; i < records[end].Count; i++)
            if (records[end][i].score == -1)
                return i;

        return 0;
    }
    public int getScore()
    {
        int score = 0;
        foreach (List<ArrowRecord> end in records)
            foreach (ArrowRecord arrow in end)
                score += Mathf.Clamp(arrow.score, 0, 10);
        return score;
    }
    #endregion
}