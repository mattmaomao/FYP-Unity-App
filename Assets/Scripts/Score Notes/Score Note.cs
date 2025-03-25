using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RecordType { Practice, Competition, Other }
public enum TargetType { Ring6, Ring10 }

public struct ArrowRecord
{
    public int score;
    public float[] landPos;
}

// one record of Round
[System.Serializable]
public class ScoreNote
{
    public System.DateTime timestamp;
    public string title;
    public RecordType recordType;
    public int distance;    // 18, 30, 50, 70, 90
    public TargetType targetType;    // 6, 10
    public int numOfRound;    // 1, 2
    public int numOfEnd;    // 6, 12
    public int arrowPerEnd;    // 3, 6

    // used equipment
    // todo

    public List<List<ArrowRecord>> records;

    public ScoreNote(System.DateTime timestamp, string title, RecordType recordType, int distance, TargetType targetType, int numOfRound, int numOfEnd, int arrowPerEnd, List<List<ArrowRecord>> records = null)
    {
        this.timestamp = timestamp;
        this.title = title;
        this.recordType = recordType;
        this.distance = distance;
        this.targetType = targetType;
        this.numOfRound = numOfRound;
        this.numOfEnd = numOfEnd;
        this.arrowPerEnd = arrowPerEnd;

        if (records == null)
            initRecord();
        else
            this.records = records;
    }

    public void initRecord()
    {
        // clear all scores
        if (records != null)
        {
            foreach (List<ArrowRecord> temp in records)
                temp.Clear();
            records.Clear();
        }

        // create list of scores (-1, default)
        if (records != null)
            records.Clear();
        records = new();
        for (int i = 0; i < numOfEnd; i++)
        {
            List<ArrowRecord> temp = new();
            for (int j = 0; j < arrowPerEnd; j++)
                temp.Add(new ArrowRecord { score = -1, landPos = default });
            records.Add(temp);
        }
    }

    public void updateScore(int end, int arrow, int score, float[] landPos)
    {
        records[end][arrow] = new ArrowRecord { score = score, landPos = landPos };
    }
    public void addScore(int score, float[] landPos)
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