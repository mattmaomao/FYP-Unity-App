using System;
using System.Collections;
using System.Collections.Generic;
using UI.Dates;
using UnityEngine;
using UnityEngine.UI;

public class ScoreAnalysisDateRange : MonoBehaviour
{
    [SerializeField] ScoreAnalysis scoreAnalysis;
    [SerializeField] RectTransform dateRangeContainer;
    [SerializeField] DatePicker datePicker_From;
    [SerializeField] DatePicker datePicker_To;

    void OnEnable()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(dateRangeContainer);
    }

    // load btn
    public void loadDateRange()
    {
        DateTime dateFrom = datePicker_From.SelectedDate.Date;
        DateTime dateTo = datePicker_To.SelectedDate.Date;
        // Debug.Log("Date From: " + dateFrom + " Date To: " + dateTo);
        scoreAnalysis.loadDateRange(dateFrom, dateTo);
    }
}
