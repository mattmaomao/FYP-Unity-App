using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RecordDisplay : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI recordTitle;
    [SerializeField] public TextMeshProUGUI recordType;
    [SerializeField] public TextMeshProUGUI recordDistance;
    [SerializeField] public TextMeshProUGUI recordDate;
    [SerializeField] public TextMeshProUGUI recordScore;

    public void init(ScoreNote note) {
        recordTitle.text = note.title;
        recordType.text = note.recordType.ToString();
        recordDistance.text = note.distance.ToString() + "m";
        recordDate.text = note.timestamp.ToString("dd/MM/yyyy");
        recordScore.text = note.getScore().ToString();
    }
}