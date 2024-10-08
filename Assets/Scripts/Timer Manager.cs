using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimerManager : MonoBehaviour
{
    // variables
    int prepareSec, readySec, endSec;
    float prepareTimer, readyTimer, endTimer;
    bool prepareEnded, readyEnded, endEnded;
    bool isRounded;
    bool isRoundA;

    // timer
    int displaySec;
    bool isRunning;
    bool isSetting;
    bool playingSE;

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
        playingSE = false;

        ResetTimer();
    }
    // Update is called once per frame
    void Update()
    {
        if (isRunning)
        {
            // ready time
            if (prepareTimer > 0)
            {
                prepareTimer -= Time.deltaTime;
                displaySec = (int)Mathf.Ceil(prepareTimer);
            }
            // ready time
            else if (readyTimer > 0)
            {
                if (!prepareEnded)
                {
                    prepareEnded = true;
                    StartCoroutine(playXBeep(2));
                }
                if (!playingSE)
                {
                    readyTimer -= Time.deltaTime;
                    displaySec = (int)Mathf.Ceil(readyTimer);
                }
            }
            // end time
            else if (endTimer > 0)
            {
                if (!readyEnded)
                {
                    readyEnded = true;
                    StartCoroutine(playXBeep(1));
                }
                if (!playingSE)
                {
                    endTimer -= Time.deltaTime;
                    displaySec = (int)Mathf.Ceil(endTimer);
                }
            }
            // switch round
            else if (isRounded && isRoundA)
            {
                if (!endEnded)
                {
                    endEnded = true;
                    StartCoroutine(playXBeep(3));
                }
                if (!playingSE)
                {
                    resetTimerCounter();
                }
            }
            // reset timer
            else
            {
                if (!endEnded)
                {
                    endEnded = true;
                    StartCoroutine(playXBeep(3));
                }
                if (!playingSE)
                {
                    ResetTimer();
                }
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

    // play sound X times
    IEnumerator playXBeep(int i)
    {
        playingSE = true;
        for (int j = 0; j < i; j++)
        {
            float waitTime = AudioManager.instance.PlaySE(AudioManager.instance.beepbeep);
            yield return new WaitForSeconds(waitTime);
        }
        playingSE = false;
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
        prepareEnded = false;
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

        readyEnded = false;
        endEnded = false;

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
