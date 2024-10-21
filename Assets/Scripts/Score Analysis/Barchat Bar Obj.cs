using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BarchartBarObj : MonoBehaviour
{
    [SerializeField] GameObject bar;
    [SerializeField] public TextMeshProUGUI labelText;
    [SerializeField] public TextMeshProUGUI valueText;

    public void changeBarHeight(float height) {
        bar.GetComponent<RectTransform>().sizeDelta = new Vector2(bar.GetComponent<RectTransform>().sizeDelta.x, height);
    }

    public void changeValue(string newValue) {
        valueText.text = newValue;
    }
}