using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreRow : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI arrowIdx;
    [SerializeField] TextMeshProUGUI arrow1;
    [SerializeField] TextMeshProUGUI arrow2;
    [SerializeField] TextMeshProUGUI arrow3;
    [SerializeField] TextMeshProUGUI endTotalText;
    [SerializeField] TextMeshProUGUI cumTotalText;

    public void updateRow(List<int> scores, int endTotal, int cumTotal)
    {
        // sort scores
        for (int i = 0; i < scores.Count; i++)
        {
            for (int j = i+1; j < scores.Count; j++)
            {
                if (scores[i] < scores[j])
                {
                    int temp = scores[i];
                    scores[i] = scores[j];
                    scores[j] = temp;
                }
            }
        }

        arrow1.text = scores[0] == -1 ? "" : scores[0] == 0 ? "M" : scores[0] == 11 ? "X" : scores[0].ToString();
        arrow2.text = scores[1] == -1 ? "" : scores[1] == 0 ? "M" : scores[1] == 11 ? "X" : scores[1].ToString();
        arrow3.text = scores[2] == -1 ? "" : scores[2] == 0 ? "M" : scores[2] == 11 ? "X" : scores[2].ToString();

        endTotalText.text = endTotal.ToString();
        cumTotalText.text = cumTotal.ToString();
    }

    public void initRow(int idx)
    {
        clearRow();
        arrowIdx.text = idx.ToString();
    }

    public void clearRow()
    {
        arrowIdx.text = "";
        arrow1.text = "";
        arrow2.text = "";
        arrow3.text = "";
        endTotalText.text = "";
        cumTotalText.text = "";
    }
}
