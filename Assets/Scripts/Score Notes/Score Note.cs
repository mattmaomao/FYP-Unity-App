using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public float timestamp;
    public RecordType recordType;
    public List<List<ArrowRecord>> records;
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
        if (records != null)
        {
            foreach (List<ArrowRecord> temp in records)
                temp.Clear();
            records.Clear();
        }

        // create list of scores (-1, default)
        records = new();
        for (int i = 0; i < endNum; i++)
        {
            List<ArrowRecord> temp = new();
            for (int j = 0; j < endArrow; j++)
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
        for (int i = 0; i < records[end].Count; i++)
            if (records[end][i].score == -1)
                return i;

        return -1;
    }
}