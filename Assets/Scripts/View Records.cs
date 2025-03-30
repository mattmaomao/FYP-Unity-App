using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Theme;
using UnityEngine;
using UnityEngine.UI;

public class ViewRecords : MonoBehaviour
{
    [SerializeField] ScoreAnalysisFilterManager filterManager;
    [Header("UI")]
    [SerializeField] GameObject popUpPanel;
    [SerializeField] TextMeshProUGUI popUpText;
    [SerializeField] RectTransform pageRect;
    [SerializeField] RectTransform sortRect;
    [SerializeField] RectTransform filterRect;
    [SerializeField] RectTransform scrollRect;

    [Header("sub pages")]
    [SerializeField] GameObject createScoreNote;
    [SerializeField] GameObject scoreNote;
    [SerializeField] GameObject recordsDisplay;

    [Header("score record display")]
    [SerializeField] List<ScoreNote> currentScoreNoteList = new();
    [SerializeField] List<GameObject> recordList = new();
    [SerializeField] Transform recordContainer;
    [SerializeField] GameObject recordPrefab;

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

    int currentNoteIdx = -1;

    void Update()
    {
        scrollRect.sizeDelta = new Vector2(scrollRect.sizeDelta.x, pageRect.rect.height - 160 - sortRect.rect.height - filterRect.rect.height - 64);
    }

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

    void reloadRecords()
    {
        currentScoreNoteList = DataManager.instance.scoreNoteList;
        showRecords();

        switch (currentSort)
        {
            case 0:
                currentSort = -1;
                sortByName();
                break;
            case 1:
                currentSort = -1;
                sortByScore();
                break;
            case 2:
                currentSort = -1;
                sortByTime();
                break;
            default:
                break;
        }
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
            int temp = i;
            GameObject record = Instantiate(recordPrefab, recordContainer);
            recordList.Add(record);
            record.GetComponent<RecordDisplay>().init(scoreNoteList[i]);
            // add button to open corresponding record
            record.GetComponent<Button>().onClick.AddListener(() =>
            {
                openNote(scoreNoteList[temp]);
                currentNoteIdx = temp;
            });
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
    public void sortByScore()
    {
        sortBtn_byName.GetComponent<Image>().color = Color.white;
        sortBtn_byTime.GetComponent<Image>().color = Color.white;
        sortBtn_byScore.GetComponent<Image>().color = Theme.Instance.GetColorByName("Primary Pale").Color;

        if (currentSort == 1)
            isScoreAsc = !isScoreAsc;
        else
            currentSort = 1;

        currentScoreNoteList = currentScoreNoteList.OrderBy(note => note.getScore()).ToList();
        if (!isScoreAsc)
            currentScoreNoteList.Reverse();

        showRecords(currentScoreNoteList);

        sortArrow_byScore.GetComponent<Image>().sprite = isScoreAsc ? sprite_asc : sprite_dec;
    }

    public void sortByTime()
    {
        sortBtn_byName.GetComponent<Image>().color = Color.white;
        sortBtn_byTime.GetComponent<Image>().color = Theme.Instance.GetColorByName("Primary Pale").Color;
        sortBtn_byScore.GetComponent<Image>().color = Color.white;

        if (currentSort == 2)
            isTimeAsc = !isTimeAsc;
        else
            currentSort = 2;

        currentScoreNoteList = currentScoreNoteList.OrderBy(note => note.timestamp).ToList();
        string s = "";
        foreach (ScoreNote note in currentScoreNoteList)
            s += note.timestamp.ToString("MM-dd HH") + ", ";
        Debug.Log(s);
        if (!isTimeAsc) {
            currentScoreNoteList.Reverse();
            Debug.Log("reversed");
        }

        showRecords(currentScoreNoteList);

        sortArrow_byTime.GetComponent<Image>().sprite = isTimeAsc ? sprite_asc : sprite_dec;
    }

    #endregion

    #region filter

    // filter btn
    public void loadDataFilter()
    {
        FilterData filterData = filterManager.loadDataFilter();
        DateTime dateFrom = filterData.dateFrom;
        DateTime dateTo = filterData.dateTo;
        int recordType = filterData.recordType;
        int distance = filterData.distance;

        int[] distanceChoice = { 18, 30, 50, 70, 90 };

        // filter data
        currentScoreNoteList = DataManager.instance.scoreNoteList.FindAll(d => d.timestamp >= dateFrom && d.timestamp <= dateTo);
        if (recordType != -1 && currentScoreNoteList.Count > 0)
            currentScoreNoteList = currentScoreNoteList.FindAll(d => d.recordType == (RecordType)recordType);
        if (distance != -1 && currentScoreNoteList.Count > 0)
            currentScoreNoteList = currentScoreNoteList.FindAll(d => d.distance == distanceChoice[distance]);
        if (currentScoreNoteList.Count > 0)
            showRecords(currentScoreNoteList);
        else
        {
            popUpText.text += "No score note found!\n";
        }

        // show pop up
        if (popUpText.text != "")
        {
            popUpText.text += "Please adjust the filters!";
            popUpPanel.SetActive(true);
        }
    }

    public void closePopUp()
    {
        popUpPanel.SetActive(false);
        popUpText.text = "";
    }


    #endregion

    // called when edit old note
    public void editNote()
    {
        ScoreNote i = currentScoreNoteList[currentNoteIdx];
        createScoreNote.GetComponent<CreateScoreNote>().loadOldNote(currentScoreNoteList[currentNoteIdx]);
        createScoreNote.SetActive(true);
        NavigationManager.instance.inScoreNote = true;
    }

    // called when create new note
    public void createNote()
    {
        createScoreNote.GetComponent<CreateScoreNote>().init();
        createScoreNote.SetActive(true);
        NavigationManager.instance.inScoreNote = true;
    }

    public void openNote(ScoreNote note)
    {
        scoreNote.GetComponent<ScoreNotesManager>().initScoreNote(note);
        scoreNote.SetActive(true);
        createScoreNote.SetActive(false);
        recordsDisplay.SetActive(false);
        NavigationManager.instance.inScoreNote = true;
    }

    public void closeNote()
    {
        scoreNote.SetActive(false);
        createScoreNote.SetActive(false);
        recordsDisplay.SetActive(true);
        NavigationManager.instance.inScoreNote = false;

        // check if notes updated
        reloadRecords();
    }
}