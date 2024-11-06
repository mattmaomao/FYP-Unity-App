using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TableData : MonoBehaviour
{
    float min;
    float max;
    float dataCount;
    float avg;
    float fluctuate;

    float timer = 0;
    float updateInterval = 3;

    [Header("UI")]
    public TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI minText;
    [SerializeField] TextMeshProUGUI maxText;
    [SerializeField] TextMeshProUGUI avgText;
    [SerializeField] TextMeshProUGUI fluctuateText;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > updateInterval)
        {
            timer = 0;
            min = 10000;
            max = -10000;
            dataCount = 0;
            avg = 0;
            fluctuate = 0;
        }

        minText.text = min.ToString("F1");
        maxText.text = max.ToString("F1");
        avgText.text = avg.ToString("F1");
        fluctuateText.text = fluctuate.ToString("F1");
    }

    public void readData(float data)
    {
        min = Mathf.Min(min, data);
        max = Mathf.Max(max, data);
        avg = (avg * dataCount + data) / (dataCount + 1);
        dataCount++;
        fluctuate = max - min;
    }
}