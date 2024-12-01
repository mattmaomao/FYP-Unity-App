using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroVideoSizeFitter : MonoBehaviour
{
    float videoWidth = 1440;
    float videoHighit = 810;

    [SerializeField] RectTransform textArea;
    [SerializeField] RectTransform HeaderArea;

    void Start()
    {
        transform.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.width * videoHighit / videoWidth);
        textArea.sizeDelta = new Vector2(Screen.width, Screen.height - HeaderArea.sizeDelta.y -150-64 - transform.GetComponent<RectTransform>().sizeDelta.y);
    }
}
