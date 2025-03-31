using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI fastestTimeText;
    [SerializeField] GameObject welcomePage;
    [SerializeField] GameObject tutPanel;
    [SerializeField] GameObject gamePage;
    [SerializeField] GameObject endGamePanel;

    [Header("Game")]
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] TextMeshProUGUI countDownText;
    [SerializeField] TextMeshProUGUI startingNumberText;
    [SerializeField] TextMeshProUGUI currNumberText;
    [SerializeField] List<Button> gridBtns;
    int startingNum = 0;
    [SerializeField] int currNum = -1;
    bool playing = false;
    bool ready = false;
    float readyTime = 3f;
    float timer;

    [Header("end game display")]
    [SerializeField] TextMeshProUGUI endgameText1;
    [SerializeField] TextMeshProUGUI endgameText2;

    void Start()
    {
        fastestTimeText.text = $"Fastest Time:\n{PlayerPrefs.GetFloat("FastestTime", 99) / 60:00}:{PlayerPrefs.GetFloat("FastestTime", 99) % 60:00}";
        welcomePage.SetActive(true);
        tutPanel.SetActive(false);
        gamePage.SetActive(false);
        endGamePanel.SetActive(false);
        resetGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (!playing) return;

        if (!ready)
        {
            timer += Time.deltaTime;
            countDownText.text = $"Game Starting In:\n<color=red>{(readyTime - timer).ToString("0")}</color>";
            if (timer >= readyTime)
            {
                ready = true;
                timer = 0;
            }
        }

        if (ready)
        {
            timer += Time.deltaTime;
            timerText.text = $"{timer / 60:00}:{timer % 60:00}";
        }
    }

    public void clickNum(NumberBoardBtn btn, int num)
    {
        if (!playing) return;
        
        // wrong number
        if (num != currNum + 1)
        {
            btn.showWrong();
            return;
        }

        currNum = num;
        currNumberText.text = $"Current Number: {currNum}";

        if (currNum == startingNum + 24)
        {
            playing = false;
            if (timer < PlayerPrefs.GetFloat("FastestTime", 99))
            {
                PlayerPrefs.SetFloat("FastestTime", timer);
                fastestTimeText.text = $"Fastest Time:\n{PlayerPrefs.GetFloat("FastestTime", 99) / 60:00}:{PlayerPrefs.GetFloat("FastestTime", 99) % 60:00}";
            }
            endGame();
        }
    }

    void setupGame()
    {
        startingNum = Random.Range(10, 75);
        List<int> numbers = Enumerable.Range(startingNum, 25).ToList();
        // shuffle
        for (int i = 0; i < numbers.Count; i++)
        {
            int rnd = Random.Range(i, numbers.Count);
            var temp = numbers[i];
            numbers[i] = numbers[rnd];
            numbers[rnd] = temp;
        }

        for (int i = 0; i < 25; i++)
        {
            gridBtns[i].gameObject.GetComponent<NumberBoardBtn>().init(this, numbers[i]);
        }

        currNum = startingNum - 1;
        startingNumberText.text = $"Starting Number: <color=red>{startingNum}</color>";
        currNumberText.text = "Current Number: -";
        playing = true;
    }

    void endGame() {
        endGamePanel.SetActive(true);
        endgameText1.text = $"Time Completed:\n{timer / 60:00}:{timer % 60:00}";
        endgameText2.text = $"Fastest Time:\n{PlayerPrefs.GetFloat("FastestTime", 99) / 60:00}:{PlayerPrefs.GetFloat("FastestTime", 99) % 60:00}";
    }

    void resetGame()
    {
        playing = false;
        ready = false;
        timer = 0;
    }

    // ui
    public void restart() {
        endGamePanel.SetActive(false);
        resetGame();
        setupGame();
    }

    public void EnterGame()
    {
        welcomePage.SetActive(false);
        gamePage.SetActive(true);
        endGamePanel.SetActive(false);
        resetGame();
        setupGame();
    }

    public void QuitGame()
    {
        welcomePage.SetActive(true);
        gamePage.SetActive(false);
        endGamePanel.SetActive(false);
    }

    public void ExitGame()
    {
        SceneManager.LoadScene("Main Scene");
    }
}
