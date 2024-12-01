using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Theme;
using UnityEngine;
using UnityEngine.UI;

public class ViewRecords : MonoBehaviour
{
    [Header("sub pages")]
    [SerializeField] GameObject createScoreNote;
    [SerializeField] GameObject scoreNote;
    [SerializeField] GameObject recordsDisplay;

    [Header("score record display")]
    [SerializeField] List<ScoreNote> currentScoreNoteList = new();
    [SerializeField] List<GameObject> recordList = new();
    [SerializeField] Transform recordContainer;
    [SerializeField] GameObject recordPrefab;

    [Header("filter display")]
    [SerializeField] Button filterExtendBtn;

    [Header("sort display")]
    [SerializeField] Button sortBtn_byName;
    [SerializeField] Image sortArrow_byName;
    [SerializeField] Button sortBtn_byScore;
    [SerializeField] Image sortArrow_byScore;
    [SerializeField] Button sortBtn_byTime;
    [SerializeField] Image sortArrow_byTime;
    [SerializeField] Sprite sprite_asc;
    [SerializeField] Sprite sprite_dec;
    int currentSort = -1;
    bool isNameAsc = true;
    bool isScoreAsc = false;
    bool isTimeAsc = false;

    void OnEnable()
    {
        currentScoreNoteList = DataManager.instance.scoreNoteList;
        showRecords();

        if (currentSort != 2)
            sortByTime();
        if (isTimeAsc)
            sortByTime();

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
    void showRecords(List<ScoreNote> scoreNoteList = null)
    {
        if (scoreNoteList == null)
            scoreNoteList = DataManager.instance.scoreNoteList;

        // delete all previous shown records
        foreach (GameObject obj in recordList)
            Destroy(obj);
        if (recordList != null)
            recordList.Clear();
        recordList = new();

        // spawn object for each record
        for (int i = 0; i < scoreNoteList.Count; i++)
        {
            int bruh = i;
            GameObject record = Instantiate(recordPrefab, recordContainer);
            recordList.Add(record);
            record.GetComponent<RecordDisplay>().init(scoreNoteList[i]);
            // add button to open corresponding record
            record.GetComponent<Button>().onClick.AddListener(() => openNote(scoreNoteList[bruh]));
        }
    }

    #region sort display
    public void sortByName()
    {
        sortBtn_byName.GetComponent<Image>().color = Theme.Instance.GetColorByName("Primary Pale").Color;
        sortBtn_byTime.GetComponent<Image>().color = Color.white;
        sortBtn_byScore.GetComponent<Image>().color = Color.white;

        if (currentSort == 0)
            isNameAsc = !isNameAsc;
        else
            currentSort = 0;

        currentScoreNoteList = currentScoreNoteList.OrderBy(note => note.title).ToList();
        if (!isNameAsc)
            currentScoreNoteList.Reverse();

        showRecords(currentScoreNoteList);
        sortArrow_byName.GetComponent<Image>().sprite = isNameAsc ? sprite_asc : sprite_dec;
    }

    public void sortByTime()
    {
        sortBtn_byName.GetComponent<Image>().color = Color.white;
        sortBtn_byTime.GetComponent<Image>().color = Theme.Instance.GetColorByName("Primary Pale").Color;
        sortBtn_byScore.GetComponent<Image>().color = Color.white;

        if (currentSort == 1)
            isTimeAsc = !isTimeAsc;
        else
            currentSort = 1;

        currentScoreNoteList = currentScoreNoteList.OrderBy(note => note.timestamp).ToList();
        if (!isTimeAsc)
            currentScoreNoteList.Reverse();

        showRecords(currentScoreNoteList);

        sortArrow_byTime.GetComponent<Image>().sprite = isTimeAsc ? sprite_asc : sprite_dec;
    }

    public void sortByScore()
    {
        sortBtn_byName.GetComponent<Image>().color = Color.white;
        sortBtn_byTime.GetComponent<Image>().color = Color.white;
        sortBtn_byScore.GetComponent<Image>().color = Theme.Instance.GetColorByName("Primary Pale").Color;

        if (currentSort == 2)
            isScoreAsc = !isScoreAsc;
        else
            currentSort = 2;

        currentScoreNoteList = currentScoreNoteList.OrderBy(note => note.getScore()).ToList();
        if (!isScoreAsc)
            currentScoreNoteList.Reverse();

        showRecords(currentScoreNoteList);

        sortArrow_byScore.GetComponent<Image>().sprite = isScoreAsc ? sprite_asc : sprite_dec;
    }
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
        NavigationManager.instance.inScoreNote = true;
    }

    public void closeNote() {
        scoreNote.SetActive(false);
        recordsDisplay.SetActive(true);
        NavigationManager.instance.inScoreNote = false;
    }
}