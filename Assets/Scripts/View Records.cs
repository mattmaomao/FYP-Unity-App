using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewRecords : MonoBehaviour
{
    // read list from save file
    public List<ScoreNote> scoreNoteList = new List<ScoreNote>();

    [Header("sub pages")]
    [SerializeField] GameObject createScoreNote;
    [SerializeField] GameObject scoreNote;
    [SerializeField] GameObject recordsDisplay;

    [Header("score record display")]
    [SerializeField] List<GameObject> recordList = new();
    [SerializeField] Transform recordContainer;
    [SerializeField] GameObject recordPrefab;

    void Start()
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
        // todo
        // debug temp
        scoreNoteList.Add(new ScoreNote(
            timestamp: System.DateTime.Now,
            title: "record 1",
            recordType: RecordType.Practice,
            distance: 18,
            targetType: TargetType.Ring10,
            numOfRound: 2,
            numOfEnd: 12,
            arrowPerEnd: 6
        ));
        scoreNoteList[0].initScores();
        for (int i = 0; i < scoreNoteList[0].numOfEnd / 2; i++)
            for (int j = 0; j < scoreNoteList[0].arrowPerEnd; j++)
                scoreNoteList[0].updateScore(i, j, Random.Range(0, 12), default);

        // spawn object for each record
        for (int i = 0; i < scoreNoteList.Count; i++)
        {
            int bruh = i;
            foreach (GameObject obj in recordList)
                Destroy(obj);
            recordList.Clear();
            recordList = new();
            GameObject record = Instantiate(recordPrefab, recordContainer);
            record.GetComponent<RecordDisplay>().init(scoreNoteList[i]);
            // add button to open corresponding record
            record.GetComponent<Button>().onClick.AddListener(() => openNote(scoreNoteList[bruh]));
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