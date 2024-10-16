using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RecordDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI recordTitle;
    [SerializeField] TextMeshProUGUI recordDate;
    [SerializeField] TextMeshProUGUI recordScore;

    public void init(ScoreNote note) {
        recordTitle.text = note.title;
        recordDate.text = note.timestamp.ToString("dd/mm/yyyy");
        recordScore.text = note.getScore().ToString();
    }
}