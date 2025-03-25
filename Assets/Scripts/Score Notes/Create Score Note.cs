using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CreateScoreNote : MonoBehaviour
{
    [SerializeField] ViewRecords viewRecords;
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TMP_InputField titleName;
    [SerializeField] RadioSelection radio_recordType;
    [SerializeField] RadioSelection radio_distance;
    [SerializeField] RadioSelection radio_targetType;
    [SerializeField] RadioSelection radio_numOfRound;
    [SerializeField] RadioSelection radio_numOfEnd;
    [SerializeField] RadioSelection radio_arrowPerEnd;
    [SerializeField] GameObject createBtn;
    [SerializeField] GameObject saveBtn;

    // equipment set used
    // todo
    [SerializeField] TMP_Dropdown equipmentSet;

    [Header("UI")]
    readonly List<RecordType> recordTypes = new() { RecordType.Practice, RecordType.Competition, RecordType.Other };
    readonly List<int> distances = new() { 18, 30, 40, 50, 60, 70, 90 };
    readonly List<TargetType> targetTypes = new() { TargetType.Ring6, TargetType.Ring10 };
    readonly List<int> numOfRounds = new() { 1, 2 };
    readonly List<int> numOfEnds = new() { 6, 12 };
    readonly List<int> arrowPerEnds = new() { 3, 6 };

    [Header("edit note")]
    int noteIndex = -1;
    ScoreNote oldNote;

    public void init()
    {
        // auto generate name
        titleName.placeholder.GetComponent<TextMeshProUGUI>().text = "Record " + DataManager.instance.scoreNoteList.Count;

        titleText.text = "Create Score Note";
        radio_recordType.init();
        radio_distance.init();
        radio_targetType.init();
        radio_numOfRound.init();
        radio_numOfEnd.init();
        radio_arrowPerEnd.init();
        createBtn.SetActive(true);
        saveBtn.SetActive(false);
    }

    public void loadOldNote(ScoreNote oldNote)
    {
        init();

        this.oldNote = oldNote;
        titleText.text = "Edit Score Note";

        titleName.text = oldNote.title;
        int targetIdx = 0;
        switch (oldNote.distance)
        {
            case 18: targetIdx = 0; break;
            case 30: targetIdx = 1; break;
            case 40: targetIdx = 2; break;
            case 50: targetIdx = 3; break;
            case 60: targetIdx = 4; break;
            case 70: targetIdx = 5; break;
            case 90: targetIdx = 6; break;
            default: targetIdx = 0; break;
        }
        Debug.Log($"{oldNote.title}");
        Debug.Log($"recordType: {(int)oldNote.recordType}");
        Debug.Log($"distance: {targetIdx}");
        Debug.Log($"targetType: {(int)oldNote.targetType}");
        Debug.Log($"numOfRound: {(oldNote.numOfRound == 1 ? 0 : 1)}");
        Debug.Log($"numOfEnd: {(oldNote.numOfEnd == 6 ? 0 : 1)}");
        Debug.Log($"arrowPerEnd: {(oldNote.arrowPerEnd == 3 ? 0 : 1)}");

        radio_recordType.selectRadio((int)oldNote.recordType);
        radio_distance.selectRadio(targetIdx);
        radio_targetType.selectRadio((int)oldNote.targetType);
        radio_numOfRound.selectRadio(oldNote.numOfRound == 1 ? 0 : 1);
        radio_numOfEnd.selectRadio(oldNote.numOfEnd == 6 ? 0 : 1);
        radio_arrowPerEnd.selectRadio(oldNote.arrowPerEnd == 3 ? 0 : 1);
        createBtn.SetActive(false);
        saveBtn.SetActive(true);
    }

    public void saveNote()
    {
        ScoreNote note = new(
            timestamp: oldNote.timestamp,
            title: titleName.text == "" ? titleName.placeholder.GetComponent<TextMeshProUGUI>().text : titleName.text,
            recordType: recordTypes[radio_recordType.selection],
            distance: distances[radio_distance.selection],
            targetType: targetTypes[radio_targetType.selection],
            numOfRound: numOfRounds[radio_numOfRound.selection],
            numOfEnd: numOfEnds[radio_numOfEnd.selection],
            arrowPerEnd: arrowPerEnds[radio_arrowPerEnd.selection],
            records: oldNote.records
        );

        for (int i = 0; i < DataManager.instance.scoreNoteList.Count; i++)
        {
            if (DataManager.instance.scoreNoteList[i].timestamp == oldNote.timestamp)
            {
                noteIndex = i;
                break;
            }
        }

        DataManager.instance.scoreNoteList[noteIndex] = note;
        DataManager.instance.SaveScoreNoteToFile();
        viewRecords.openNote(note);
    }

    public void createNote()
    {
        // // copy old records
        // List<List<ArrowRecord>> tempRecords = new();
        // for (int i = 0; i < oldNote.records.Count; i++)
        // {
        //     tempRecords.Add(new List<ArrowRecord>());
        //     for (int j = 0; j < oldNote.records[i].Count; j++)
        //         tempRecords[i].Add(new ArrowRecord { score = oldNote.records[i][j].score, landPos = oldNote.records[i][j].landPos });
        // }

        ScoreNote note = new(
            timestamp: System.DateTime.Now,
            title: titleName.text == "" ? titleName.placeholder.GetComponent<TextMeshProUGUI>().text : titleName.text,
            recordType: recordTypes[radio_recordType.selection],
            distance: distances[radio_distance.selection],
            targetType: targetTypes[radio_targetType.selection],
            numOfRound: numOfRounds[radio_numOfRound.selection],
            numOfEnd: numOfEnds[radio_numOfEnd.selection],
            arrowPerEnd: arrowPerEnds[radio_arrowPerEnd.selection]
        );
        // Debug.Log($"old note record: [{oldNote.records.Count}][{oldNote.records[0].Count}], new note record: [{note.records.Count}][{note.records[0].Count}]");

        DataManager.instance.scoreNoteList.Add(note);
        DataManager.instance.SaveScoreNoteToFile();
        viewRecords.openNote(note);
    }

    public void cancel()
    {
        gameObject.SetActive(false);
    }
}