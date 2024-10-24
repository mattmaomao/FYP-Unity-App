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
            float[] pos = new float[] {localPoint.x / maxBound, localPoint.y / maxBound};
            scoreNotesManager.addScore(calculateScore(localPoint), pos);
        }
    }

    int calculateScore(Vector2 clickPosition)
    {
        float distance = Vector2.Distance(Vector2.zero, clickPosition);
        if (scoreNotesManager.scoreNote.targetType == TargetType.Ring10) {
            float temp = distance / maxBound * 10;
            return temp <= 0.5f ? 11 : temp >= 10 ? 0 : Mathf.CeilToInt(10 - temp);
        }
        else {
            float temp = distance / maxBound * 5;
            return temp <= 1f ? 11 : temp >= 5 ? 0 : Mathf.CeilToInt(10 - temp);
        }
    }
}