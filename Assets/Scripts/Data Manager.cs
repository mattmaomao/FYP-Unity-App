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

    // setting
    JsonSerializerSettings settings;

    // score note
    public List<ScoreNote> scoreNoteList = new List<ScoreNote>();
    string filePath_ScoreNote = "/score_notes.json";

    // posture data
    public List<PostureData> postureDataList = new List<PostureData>();
    string filePath_PostureData = "/posture_data.json";


    void Start()
    {
        // config
        settings = new JsonSerializerSettings();
        settings.Converters.Add(new Vector3Converter());

        // init
        filePath_ScoreNote = Application.persistentDataPath + filePath_ScoreNote;
        LoadScoreNoteFromFile();

        filePath_PostureData = Application.persistentDataPath + filePath_PostureData;
        LoadPostureDataFromFile();
    }

    // score note
    [ContextMenu("Save All Score Note")]
    public void SaveScoreNoteToFile()
    {
        string json = JsonConvert.SerializeObject(scoreNoteList, Formatting.Indented);

        File.WriteAllText(filePath_ScoreNote, json);

        Debug.Log("Data saved to file successfully. " + filePath_ScoreNote);
        LoadScoreNoteFromFile();
    }

    [ContextMenu("Load Score Note from json")]
    public void LoadScoreNoteFromFile()
    {
        if (File.Exists(filePath_ScoreNote))
        {
            string json = File.ReadAllText(filePath_ScoreNote);
            scoreNoteList.Clear();
            scoreNoteList = new();
            scoreNoteList = JsonConvert.DeserializeObject<List<ScoreNote>>(json);

            Debug.Log("Data loaded from file successfully. " + filePath_ScoreNote);
        }
        else
        {
            scoreNoteList.Clear();
            scoreNoteList = new();
        }
    }

    // posture data
    public void SavePostureDataToFile()
    {
        string json = JsonConvert.SerializeObject(postureDataList, Formatting.Indented, settings);

        File.WriteAllText(filePath_PostureData, json);

        Debug.Log("Data saved to file successfully. " + filePath_PostureData);
    }
    public void LoadPostureDataFromFile()
    {
        if (File.Exists(filePath_PostureData))
        {
            string json = File.ReadAllText(filePath_PostureData);
            postureDataList.Clear();
            postureDataList = new();
            postureDataList = JsonConvert.DeserializeObject<List<PostureData>>(json);

            Debug.Log("Data loaded from file successfully. " + filePath_PostureData);
        }
        else
        {
            postureDataList.Clear();
            postureDataList = new();
        }
    }

    // fake data
    [SerializeField] int fakeDataCount = 100;

    [ContextMenu("Gen MANY Fake Score Note Data")]
    public void genScoreNote()
    {
        int c = 1;
        for (int i = 0; i < fakeDataCount; i++)
        {
            float rand = Random.Range(0f, 1f);
            System.DateTime dateTime = System.DateTime.Now.AddDays(rand < 0.3f ? 0 : -c++);
            dateTime.AddHours(-i);
            TargetType targetType = (TargetType)Random.Range(0, 2);
            scoreNoteList.Add(new ScoreNote(
                timestamp: dateTime,
                title: $"fake record {scoreNoteList.Count + 1}",
                recordType: (RecordType)Random.Range(0, 3),
                distance: new int[] { 18, 30, 50, 70, 90 }[Random.Range(0, 5)],
                targetType: targetType,
                numOfRound: new int[] { 1, 2 }[Random.Range(0, 2)],
                numOfEnd: new int[] { 6, 12 }[Random.Range(0, 2)],
                arrowPerEnd: new int[] { 3, 6 }[Random.Range(0, 2)]
            ));
            for (int j = 0; j < scoreNoteList[^1].numOfEnd; j++)
                for (int k = 0; k < scoreNoteList[^1].arrowPerEnd; k++)
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
                    scoreNoteList[^1].updateScore(k, k, score, new float[] { clickPosition.x, clickPosition.y });
                }
        }
        SaveScoreNoteToFile();
    }

    [ContextMenu("Gen MANY Fake Posture Data")]
    public void genPostureData()
    {
        for (int i = 0; i < fakeDataCount; i++)
        {
            // generate a random timestamp
            System.DateTime dateTime = System.DateTime.Now.AddDays(-Random.Range(0, 30));
            dateTime.AddHours(-i);

            postureDataList.Add(new PostureData
            {
                dateTime = dateTime,
                postureName = "Posture_" + (postureDataList != null ? postureDataList.Count : 0),
                // archerLvl = Random.Range(0, 4),
                scores = new() { Random.Range(0, 100f), 0, 0, 0, 0, 0, 0 }
            });
        }
        SavePostureDataToFile();
    }

    // debug
    public void DeletePostureData()
    {
        List<PostureData> emptyList = new();
        string json = JsonConvert.SerializeObject(emptyList, Formatting.Indented, settings);
        File.WriteAllText(filePath_PostureData, json);

        LoadPostureDataFromFile();
    }
    public void LoadTemplatePostureData()
    {
        string json = Resources.Load<TextAsset>("posture_data_template").text;
        postureDataList.Clear();
        postureDataList = new();
        postureDataList = JsonConvert.DeserializeObject<List<PostureData>>(json);
        // save to file
        SavePostureDataToFile();
    }
    public void DeleteScoreNoteData()
    {
        List<ScoreNote> emptyList = new();
        string json = JsonConvert.SerializeObject(emptyList, Formatting.Indented);
        File.WriteAllText(filePath_ScoreNote, json);

        LoadScoreNoteFromFile();
    }
    public void LoadTemplateScoreNoteDate()
    {
        string json = Resources.Load<TextAsset>("score_notes_template").text;
        scoreNoteList.Clear();
        scoreNoteList = new();
        scoreNoteList = JsonConvert.DeserializeObject<List<ScoreNote>>(json);
        // save date to file
        SaveScoreNoteToFile();
    }
    List<InputEntry> bow = new List<InputEntry>();
    public void DeleteBowData()
    {
        List<InputEntry> emptyList = new();
        string json = JsonConvert.SerializeObject(emptyList, Formatting.Indented);
        File.WriteAllText(Application.persistentDataPath + "/" + "bow.json", json);
    }
    public void LoadTemplateBowData()
    {
        string json = Resources.Load<TextAsset>("bow_data_template").text;
        bow.Clear();
        bow = new();
        bow = JsonConvert.DeserializeObject<List<InputEntry>>(json);
        // save date to file
        string content = JsonConvert.SerializeObject(bow, Formatting.Indented);
        File.WriteAllText(Application.persistentDataPath + "/" + "bow.json", content);
    }
}