using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewRecords : MonoBehaviour
{
    [Header("sub pages")]
    [SerializeField] GameObject createScoreNote;
    [SerializeField] GameObject scoreNote;
    [SerializeField] GameObject recordsDisplay;

    [Header("score record display")]
    [SerializeField] List<GameObject> recordList = new();
    [SerializeField] Transform recordContainer;
    [SerializeField] GameObject recordPrefab;

    void OnEnable()
    {
        loadRecords();

        // display all saved records
        recordsDisplay.SetActive(true);
        createScoreNote.SetActive(false);
        scoreNote.SetActive(false);
    }

    void OnDisable()
    {
        recordsDisplay.SetActive(true);
        createScoreNote.SetActive(false);
        scoreNote.SetActive(false);
    }

    // read from save file
    void loadRecords()
    {
        foreach (GameObject obj in recordList)
            Destroy(obj);
        if (recordList != null)
            recordList.Clear();
        recordList = new();

        // spawn object for each record
        for (int i = 0; i < DataManager.instance.scoreNoteList.Count; i++)
        {
            int bruh = i;
            GameObject record = Instantiate(recordPrefab, recordContainer);
            recordList.Add(record);
            record.GetComponent<RecordDisplay>().init(DataManager.instance.scoreNoteList[i]);
            // add button to open corresponding record
            record.GetComponent<Button>().onClick.AddListener(() => openNote(DataManager.instance.scoreNoteList[bruh]));
        }
    }

    #region sort display
    // todo, later
    #endregion

    // called when create new note
    public void createNote()
    {
        createScoreNote.GetComponent<CreateScoreNote>().init();
        createScoreNote.SetActive(true);
    }

    public void openNote(ScoreNote note)
    {
        scoreNote.GetComponent<ScoreNotesManager>().initScoreNote(note);
        scoreNote.SetActive(true);
        createScoreNote.SetActive(false);
        recordsDisplay.SetActive(false);
    }
}