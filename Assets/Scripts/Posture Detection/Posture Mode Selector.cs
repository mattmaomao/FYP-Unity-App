using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostureModeSelector : MonoBehaviour
{
    [SerializeField] GameObject postureDetection;
    [SerializeField] GameObject[] modes;

    void OnEnable()
    {
        resetMode();
    }

    void Start() {
        resetMode();
    }

    // disable all mode on enable
    void resetMode()
    {
        postureDetection.SetActive(false);
        for (int i = 0; i < modes.Length; i++)
            modes[i].SetActive(false);
    }

    // select btn
    public void SelectMode(int mode)
    {
        postureDetection.SetActive(true);
        for (int i = 0; i < modes.Length; i++)
            if (i == mode)
                modes[i].SetActive(true);
            else
                modes[i].SetActive(false);
    }
}