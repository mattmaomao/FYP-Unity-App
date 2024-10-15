using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreRecords : MonoBehaviour
{
    // read list from save file
    // todo
    public List<ScoreNote> scoreNoteList = new List<ScoreNote>();

    [Header("sub pages")]
    [SerializeField] GameObject createScoreNote;
    [SerializeField] GameObject scoreNote;

    void Start()
    {
        // display all saved records
        // todo
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
    void OnDisable()
    {
        createScoreNote.SetActive(false);
    }
}