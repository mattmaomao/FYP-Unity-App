using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroVideoSizeFitter : MonoBehaviour
{
    float videoWidth = 1440;
    float videoHeight = 810;

    [SerializeField] RectTransform pageContainer;
    [SerializeField] RectTransform textArea;

    void Start()
    {
        transform.GetComponent<RectTransform>().sizeDelta = new Vector2(pageContainer.rect.width, pageContainer.rect.width * videoHeight / videoWidth);
        textArea.sizeDelta = new Vector2(pageContainer.rect.width, pageContainer.rect.height -150-64-64 - transform.GetComponent<RectTransform>().rect.height);
    }
}
