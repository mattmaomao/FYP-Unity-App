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

    [ContextMenu("get text")]
    public void turnDataTxt()
    {
        string json = File.ReadAllText(Application.persistentDataPath + filePath_PostureData);
        List<PostureData> temp = JsonConvert.DeserializeObject<List<PostureData>>(json);
        // only fetch certain fields

        float minFrontWristFluctuate = 99999;
        float minbackWristFluctuate = 99999;
        float minfrontElbowAngleFluctuate = 99999;
        float minbackElbowAngleFluctuate = 99999;
        float minfrontShoulderAngleFluctuate = 99999;
        float minbackShoulderAngleFluctuate = 99999;

        float maxFrontWristFluctuate = 0;
        float maxbackWristFluctuate = 0;
        float maxfrontElbowAngleFluctuate = 0;
        float maxbackElbowAngleFluctuate = 0;
        float maxfrontShoulderAngleFluctuate = 0;
        float maxbackShoulderAngleFluctuate = 0;
        foreach (var item in temp)
        {
            // find min
            if (item.frontWristFluctuate < minFrontWristFluctuate)
                minFrontWristFluctuate = item.frontWristFluctuate;
            if (item.backWristFluctuate < minbackWristFluctuate)
                minbackWristFluctuate = item.backWristFluctuate;
            if (item.frontElbowAngleFluctuate < minfrontElbowAngleFluctuate)
                minfrontElbowAngleFluctuate = item.frontElbowAngleFluctuate;
            if (item.backElbowAngleFluctuate < minbackElbowAngleFluctuate)
                minbackElbowAngleFluctuate = item.backElbowAngleFluctuate;
            if (item.frontShoulderAngleFluctuate < minfrontShoulderAngleFluctuate)
                minfrontShoulderAngleFluctuate = item.frontShoulderAngleFluctuate;
            if (item.backShoulderAngleFluctuate < minbackShoulderAngleFluctuate)
                minbackShoulderAngleFluctuate = item.backShoulderAngleFluctuate;

            // find max
            if (item.frontWristFluctuate > maxFrontWristFluctuate)
                maxFrontWristFluctuate = item.frontWristFluctuate;
            if (item.backWristFluctuate > maxbackWristFluctuate)
                maxbackWristFluctuate = item.backWristFluctuate;
            if (item.frontElbowAngleFluctuate > maxfrontElbowAngleFluctuate)
                maxfrontElbowAngleFluctuate = item.frontElbowAngleFluctuate;
            if (item.backElbowAngleFluctuate > maxbackElbowAngleFluctuate)
                maxbackElbowAngleFluctuate = item.backElbowAngleFluctuate;
            if (item.frontShoulderAngleFluctuate > maxfrontShoulderAngleFluctuate)
                maxfrontShoulderAngleFluctuate = item.frontShoulderAngleFluctuate;
            if (item.backShoulderAngleFluctuate > maxbackShoulderAngleFluctuate)
                maxbackShoulderAngleFluctuate = item.backShoulderAngleFluctuate;
        }

        Debug.Log($"minFrontWristFluctuate: {minFrontWristFluctuate}");
        Debug.Log($"minbackWristFluctuate: {minbackWristFluctuate}");
        Debug.Log($"minfrontElbowAngleFluctuate: {minfrontElbowAngleFluctuate}");
        Debug.Log($"minbackElbowAngleFluctuate: {minbackElbowAngleFluctuate}");
        Debug.Log($"minfrontShoulderAngleFluctuate: {minfrontShoulderAngleFluctuate}");
        Debug.Log($"minbackShoulderAngleFluctuate: {minbackShoulderAngleFluctuate}");
        
        Debug.Log($"maxFrontWristFluctuate: {maxFrontWristFluctuate}");
        Debug.Log($"maxbackWristFluctuate: {maxbackWristFluctuate}");
        Debug.Log($"maxfrontElbowAngleFluctuate: {maxfrontElbowAngleFluctuate}");
        Debug.Log($"maxbackElbowAngleFluctuate: {maxbackElbowAngleFluctuate}");
        Debug.Log($"maxfrontShoulderAngleFluctuate: {maxfrontShoulderAngleFluctuate}");
        Debug.Log($"maxbackShoulderAngleFluctuate: {maxbackShoulderAngleFluctuate}");
    }
}



// minFrontWristFluctuate: 7.470885
// minbackWristFluctuate: 44.32072
// minfrontElbowAngleFluctuate: 0.3370529
// minbackElbowAngleFluctuate: 0.4070209
// minfrontShoulderAngleFluctuate: 0.2263088
// minbackShoulderAngleFluctuate: 0.9151633

// maxFrontWristFluctuate: 129.0781
// maxbackWristFluctuate: 141.2049
// maxfrontElbowAngleFluctuate: 4.706191
// maxbackElbowAngleFluctuate: 14.23022
// maxfrontShoulderAngleFluctuate: 5.615577
// maxbackShoulderAngleFluctuate: 10.8756
