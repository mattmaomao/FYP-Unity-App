using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateScoreNote : MonoBehaviour
{
    [SerializeField] ViewRecords viewRecords;
    [SerializeField] TMP_InputField titleName;
    [SerializeField] RadioSelection radio_recordType;
    [SerializeField] RadioSelection radio_distance;
    [SerializeField] RadioSelection radio_targetType;
    [SerializeField] RadioSelection radio_numOfRound;
    [SerializeField] RadioSelection radio_numOfEnd;
    [SerializeField] RadioSelection radio_arrowPerEnd;

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

    public void init()
    {
        radio_recordType.init();
        radio_distance.init();
        radio_targetType.init();
        radio_numOfRound.init();
        radio_numOfEnd.init();
        radio_arrowPerEnd.init();
    }

    void Update()
    {
        // auto generate name
        titleName.placeholder.GetComponent<TextMeshProUGUI>().text = "Record " + DataManager.instance.scoreNoteList.Count;
    }

    public void loadOldNote(ScoreNote oldNote)
    {
        titleName.text = oldNote.title;
        radio_recordType.initSelection((int)oldNote.recordType);
        radio_distance.initSelection((int)oldNote.distance);
        radio_targetType.initSelection((int)oldNote.targetType);
        radio_numOfRound.initSelection((int)oldNote.numOfRound);
        radio_numOfEnd.initSelection((int)oldNote.numOfEnd);
        radio_arrowPerEnd.initSelection((int)oldNote.arrowPerEnd);
    }

    public void saveNote(ScoreNote oldNote)
    {
        ScoreNote note = new(
            timestamp: oldNote.timestamp,
            title: titleName.text == "" ? titleName.placeholder.GetComponent<TextMeshProUGUI>().text : titleName.text,
            recordType: recordTypes[radio_recordType.selection],
            distance: distances[radio_distance.selection],
            targetType: targetTypes[radio_targetType.selection],
            numOfRound: numOfRounds[radio_numOfRound.selection],
            numOfEnd: numOfEnds[radio_numOfEnd.selection],
            arrowPerEnd: arrowPerEnds[radio_arrowPerEnd.selection]
        );

        DataManager.instance.scoreNoteList[noteIndex] = note;
        DataManager.instance.SaveScoreNoteToFile();
        viewRecords.openNote(note);
    }

    public void createNote()
    {
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

        DataManager.instance.scoreNoteList.Add(note);
        DataManager.instance.SaveScoreNoteToFile();
        viewRecords.openNote(note);
    }

    public void cancel()
    {
        gameObject.SetActive(false);
    }
}