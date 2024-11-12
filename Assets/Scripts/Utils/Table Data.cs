using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TableData : MonoBehaviour
{
    Vector3 min;
    Vector3 max;
    float dataCount;
    Vector3 avg;
    Vector3 fluctuate;

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
            min = new Vector3(10000, 10000, 10000);
            max = new Vector3(-10000, -10000, -10000);
            dataCount = 0;
            avg = Vector3.zero;
            fluctuate = Vector3.zero;
        }

        minText.text = min.ToString("F1");
        maxText.text = max.ToString("F1");
        avgText.text = avg.ToString("F1");
        fluctuateText.text = fluctuate.ToString("F1");
    }

    public void readData(Vector3 data)
    {
        min = new Vector3(Mathf.Min(min.x, data.x), Mathf.Min(min.y, data.y), Mathf.Min(min.z, data.z));
        max = new Vector3(Mathf.Max(max.x, data.x), Mathf.Max(max.y, data.y), Mathf.Max(max.z, data.z));
        avg = (avg * dataCount + data) / (dataCount + 1);
        dataCount++;
        fluctuate = max - min;
    }

    public string printSingleData() {
        return $"{nameText.text}\t{min.x}\t{min.y}\t{min.z}\t{max.x}\t{max.y}\t{max.z}\t{avg.x}\t{avg.y}\t{avg.z}\t{fluctuate.x}\t{fluctuate.y}\t{fluctuate.z}";
    }
}