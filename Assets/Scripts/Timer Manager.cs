using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimerManager : MonoBehaviour
{
    // variables
    int prepareSec;
    int readySec;
    int endSec;
    float prepareTimer;
    float readyTimer;
    float endTimer;
    bool isRounded;
    bool isRoundA;

    // timer
    int displaySec;
    bool isRunning;

    bool isSetting;

    // env objs
    [SerializeField] TextMeshProUGUI timerDisplayText;
    [SerializeField] GameObject roundsA;
    [SerializeField] GameObject roundsB;
    [SerializeField] GameObject settingBtns;
    [SerializeField] GameObject controlBtns;
    [SerializeField] GameObject pauseBtn;


    // inputs
    [SerializeField] TMP_InputField prepareSecInput;
    [SerializeField] TMP_InputField readySecInput;
    [SerializeField] TMP_InputField endSecInput;
    [SerializeField] Toggle roundedToggle;

    void Start()
    {
        // todo
        // change to load from preset / previous use (PlayerPrefs)
        prepareSec = 3;
        readySec = 10;
        endSec = 120;
        isRounded = false;
        isRoundA = true;
        isSetting = true;

        ResetTimer();
    }
    // Update is called once per frame
    void Update()
    {
        if (isRunning)
        {

            if (prepareTimer > 0)
            {
                prepareTimer -= Time.deltaTime;
                displaySec = (int)Mathf.Ceil(prepareTimer);
            }
            else if (readyTimer > 0)
            {
                readyTimer -= Time.deltaTime;
                displaySec = (int)Mathf.Ceil(readyTimer);
            }
            else if (endTimer > 0)
            {
                endTimer -= Time.deltaTime;
                displaySec = (int)Mathf.Ceil(endTimer);
            }
            else if (isRounded && isRoundA)
            {
                resetTimerCounter();
            }
            else
            {
                ResetTimer();
            }

            // update display text
            timerDisplayText.text = displaySec.ToString("D4");
        }
        else if (isSetting)
        {
            // read input field
            if (prepareSecInput.text == "")
                prepareSecInput.text = "0";
            else
                prepareSec = int.Parse(prepareSecInput.text);

            if (readySecInput.text == "")
                readySecInput.text = "0";
            else
                readySec = int.Parse(readySecInput.text);

            if (endSecInput.text == "")
                endSecInput.text = "0";
            else
                endSec = int.Parse(endSecInput.text);

            isRounded = roundedToggle.isOn;
            if (isRounded)
            {
                roundsA.SetActive(true);
                roundsB.SetActive(true);
            }
            else
            {
                roundsA.SetActive(false);
                roundsB.SetActive(false);
            }

            // update display text
            timerDisplayText.text = int.Parse(endSecInput.text).ToString("D4");

        }

        pauseBtn.SetActive(isRunning);
        settingBtns.SetActive(isSetting);
        controlBtns.SetActive(!isSetting);

    }

    // switch between setting and control
    public void ConfirmSetting()
    {
        isSetting = false;
        ResetTimer();
    }
    public void BackToSetting()
    {
        isRunning = false;
        isSetting = true;
    }

    // timer control
    public void StartTimer()
    {
        isRunning = true;
    }
    public void PauseTimer()
    {
        isRunning = false;
    }
    public void ResetTimer()
    {
        isRunning = false;
        resetTimerCounter();
    }

    void resetTimerCounter()
    {
        prepareTimer = prepareSec;
        readyTimer = readySec;
        endTimer = endSec;
        prepareSecInput.text = prepareSec.ToString();
        readySecInput.text = readySec.ToString();
        endSecInput.text = endSec.ToString();

        // auto start again if is round A
        if (isRunning && isRounded && isRoundA)
        {
            isRunning = true;
            isRoundA = false;
            roundsA.SetActive(false);
            roundsB.SetActive(true);
            prepareTimer = 0;
        }
        // change round
        else if (isRounded)
        {
            isRoundA = true;
            roundsA.SetActive(true);
            roundsB.SetActive(false);
        }
        // hide round display
        else if (!isRounded)
        {
            roundsA.SetActive(false);
            roundsB.SetActive(false);
        }

        // update display text
        displaySec = (int)Mathf.Ceil(prepareTimer);
        timerDisplayText.text = displaySec.ToString("D4");
    }
}
