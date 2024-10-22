using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    #region Singleton
    public static DataManager instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        // DontDestroyOnLoad(gameObject);
        else
            Destroy(gameObject);
    }
    #endregion

    public List<ScoreNote> scoreNoteList = new List<ScoreNote>();
    public string filePath_ScoreNote;


    void Start()
    {
        filePath_ScoreNote = Application.persistentDataPath + "score_notes.json";
        LoadScoreNoteFromFile();
    }

    [ContextMenu("Save All Score Note")]
    public void SaveScoreNoteToFile()
    {
        string json = JsonConvert.SerializeObject(scoreNoteList, Formatting.Indented);

        // debug
        if (filePath_ScoreNote == null)
            filePath_ScoreNote = Application.persistentDataPath + "score_notes.json";

        File.WriteAllText(filePath_ScoreNote, json);

        Debug.Log("Data saved to file successfully. " + filePath_ScoreNote);
        LoadScoreNoteFromFile();
    }

    [ContextMenu("Load Score Note from json")]
    public void LoadScoreNoteFromFile()
    {
        // debug
        if (filePath_ScoreNote == null)
            filePath_ScoreNote = Application.persistentDataPath + "score_notes.json";

        if (File.Exists(filePath_ScoreNote))
        {
            string json = File.ReadAllText(filePath_ScoreNote);
            scoreNoteList.Clear();
            scoreNoteList = new();
            scoreNoteList = JsonConvert.DeserializeObject<List<ScoreNote>>(json);
        }
    }

    [ContextMenu("Gen Fake Score Note Data")]
    public void genFakeData()
    {
        TargetType targetType = (TargetType)Random.Range(0, 2);
        scoreNoteList.Add(new ScoreNote(
            timestamp: System.DateTime.Now,
            title: $"fake record {scoreNoteList.Count + 1}",
            recordType: (RecordType)Random.Range(0, 3),
            distance: new int[] { 18, 30, 50, 70, 90 }[Random.Range(0, 5)],
            targetType: targetType,
            numOfRound: new int[] { 1, 2 }[Random.Range(0, 2)],
            numOfEnd: new int[] { 6, 12 }[Random.Range(0, 2)],
            arrowPerEnd: new int[] { 3, 6 }[Random.Range(0, 2)]
        ));
        for (int i = 0; i < scoreNoteList[^1].numOfEnd; i++)
            for (int j = 0; j < scoreNoteList[^1].arrowPerEnd; j++)
            {
                Vector2 clickPosition = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                float distance = Vector2.Distance(Vector2.zero, clickPosition);
                int score = -1;
                if (targetType == TargetType.Ring10)
                {
                    float temp = distance * 10;
                    score = temp <= 0.5f ? 11 : temp >= 10 ? 0 : Mathf.CeilToInt(10 - temp);
                }
                else
                {
                    float temp = distance * 5;
                    score = temp <= 1f ? 11 : temp >= 5 ? 0 : Mathf.CeilToInt(10 - temp);
                }
                scoreNoteList[^1].updateScore(i, j, score, new float[] { clickPosition.x, clickPosition.y });
            }
    }
}