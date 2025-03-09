using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI.Dates;
using UnityEngine;
using UnityEngine.UI;

public class ScoreAnalysisFilterManager : MonoBehaviour
{
    [SerializeField] ScoreAnalysis scoreAnalysis;
    [SerializeField] GameObject filterPanel;
    [SerializeField] TextMeshProUGUI filterToggleBtnText;
    [SerializeField] RectTransform filterContainer;

    [Header("Date Range")]
    [SerializeField] DatePicker datePicker_From;
    [SerializeField] DatePicker datePicker_To;
    [SerializeField] TMP_InputField datePicker_From_InputField;
    [SerializeField] TMP_InputField datePicker_To_InputField;
    [Header("Score Notes")]
    [SerializeField] TMP_Dropdown recordTypeDropdown;
    [SerializeField] TMP_Dropdown distanceDropdown;

    void OnEnable()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(filterContainer);
    }

    #region UI btns
    // open filter panel
    public void openFilterPanel()
    {
        filterPanel.SetActive(true);
    }

    // close filter panel
    public void closeFilterPanel()
    {
        filterPanel.SetActive(false);
    }

    public void clearFilterBtn() {
        // clear selection
        datePicker_From.SelectedDate = new();
        datePicker_To.SelectedDate = new();
        datePicker_From_InputField.text = "";
        datePicker_To_InputField.text = "";
        recordTypeDropdown.value = 0;
        distanceDropdown.value = 0;
    }

    // load btn
    public void loadDataFilter()
    {
        DateTime dateFrom = DateTime.MinValue;
        DateTime dateTo = DateTime.MaxValue;

        if (datePicker_From_InputField.text != "")
            dateFrom = datePicker_From.SelectedDate.Date;
        if (datePicker_To_InputField.text != "")
            dateTo = datePicker_To.SelectedDate.Date;

        int recordTypeChoice = recordTypeDropdown.value - 1;
        int distanceChoice = distanceDropdown.value - 1;

        scoreAnalysis.loadDataFilter(dateFrom, dateTo, recordTypeChoice, distanceChoice);
    }

    #endregion
}
