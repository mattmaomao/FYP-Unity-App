using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetClick : MonoBehaviour
{
    [SerializeField] ScoreNotesManager scoreNotesManager;

    public Image image;
    float maxBound;

    void Start()
    {
        maxBound = image.rectTransform.rect.height / 2;
    }

    public void readInput()
    {
        // Convert the screen point of the click to local position within the image's RectTransform
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(image.rectTransform, Input.mousePosition, null, out localPoint);

        if (localPoint.y <= maxBound && localPoint.y >= -maxBound)
        {
            scoreNotesManager.addScore(calculateScore(localPoint), localPoint);
        }
    }

    int calculateScore(Vector2 clickPosition)
    {
        float distance = Vector2.Distance(Vector2.zero, clickPosition);
        float temp = distance / maxBound * 10;
        string score = temp <= 0.5f ? "X" : temp >= 10 ? "M" : Mathf.CeilToInt(10 - temp).ToString();

        return temp <= 0.5f ? 11 : temp >= 10 ? 0 : Mathf.CeilToInt(10 - temp);
    }
}