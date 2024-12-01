using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroVideoSizeFitter : MonoBehaviour
{
    float videoWidth = 1440;
    float videoHighit = 810;

    void Start()
    {
        transform.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.width * videoHighit / videoWidth);
    }
}
