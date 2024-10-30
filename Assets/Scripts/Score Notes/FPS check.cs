using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FPScheck : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI fpsText;
    float highestFPS = 0;
    float highestIn5Sec = 0;
    float timer = 0;

    void Update()
    {
        timer += Time.deltaTime;
        float fps = 1 / Time.deltaTime;
        if (fps > highestFPS)
            highestFPS = fps;

        if (timer >= 5) {
            timer = 0;
            highestIn5Sec = highestFPS;
            highestFPS = 0;
        }

        fpsText.text = $"FPS: {Mathf.Round(fps)} \nHighest FPS: {Mathf.Round(highestIn5Sec)}";
    }
}
