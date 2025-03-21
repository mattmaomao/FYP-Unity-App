using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI.Dates;
using UnityEngine;
using UnityEngine.UI;

public class ScoreAnalysisFilterManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] RectTransform filterContainer;
    [SerializeField] RectTransform filterMask;
    [SerializeField] float targetMaskHeight = 0;
    [SerializeField] TextMeshProUGUI filterToggleBtnText;
    bool isFilterOpen = false;

    [Header("Date Range")]
    [SerializeField] DatePicker datePicker_From;
    [SerializeField] DatePicker datePicker_To;
    [SerializeField] TMP_InputField datePicker_From_InputField;
    [SerializeField] TMP_InputField datePicker_To_InputField;
    [Header("Score Notes")]
    [SerializeField] TMP_Dropdown recordTypeDropdown;
    [SerializeField] TMP_Dropdown distanceDropdown;

    void Start()
    {
        forceHideFilter();
    }

    void OnEnable()
    {
        StartCoroutine(loadFilterLayout());
    }

    public IEnumerator loadFilterLayout()
    {
        yield return new WaitForEndOfFrame();
        LayoutRebuilder.ForceRebuildLayoutImmediate(filterContainer);
    }

    #region UI btns
    // ui
    public void toggleFilter()
    {
        isFilterOpen = !isFilterOpen;
        StartCoroutine(scrollFilter());
    }
    IEnumerator scrollFilter()
    {
        float target = isFilterOpen ? targetMaskHeight : 0;
        float animationSpeed = 10f;
        float speed = (target - filterMask.sizeDelta.y) / animationSpeed;

        while (Mathf.Abs(filterMask.sizeDelta.y - target) > 0.1f)
        {
            filterMask.sizeDelta = new Vector2(filterMask.sizeDelta.x, filterMask.sizeDelta.y + speed);
            yield return 0;
        }

        yield return loadFilterLayout();
        yield return new WaitForEndOfFrame();
        yield return loadFilterLayout();

        filterToggleBtnText.text = isFilterOpen ? "Close Filter" : "Open Filter";
        yield return 0;
    }
    void forceHideFilter()
    {
        filterMask.sizeDelta = new Vector2(filterMask.sizeDelta.x, 0);
        filterToggleBtnText.text = "Open Filter";
        isFilterOpen = false;
    }

    public void clearFilterBtn()
    {
        // clear selection
        datePicker_From.SelectedDate = new();
        datePicker_To.SelectedDate = new();
        datePicker_From_InputField.text = "";
        datePicker_To_InputField.text = "";
        recordTypeDropdown.value = 0;
        distanceDropdown.value = 0;
    }

    // load btn
    public FilterData loadDataFilter()
    {
        DateTime dateFrom = DateTime.MinValue;
        DateTime dateTo = DateTime.MaxValue;

        if (datePicker_From_InputField.text != "")
            dateFrom = datePicker_From.SelectedDate.Date;
        if (datePicker_To_InputField.text != "")
            dateTo = datePicker_To.SelectedDate.Date;

        int recordTypeChoice = recordTypeDropdown.value - 1;
        int distanceChoice = distanceDropdown.value - 1;

        return new FilterData
        {
            dateFrom = dateFrom,
            dateTo = dateTo,
            recordType = recordTypeChoice,
            distance = distanceChoice
        };
    }

    #endregion
}

public struct FilterData
{
    public DateTime dateFrom;
    public DateTime dateTo;
    public int recordType;
    public int distance;
}