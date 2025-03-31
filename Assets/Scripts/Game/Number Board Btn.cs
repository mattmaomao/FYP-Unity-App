using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NumberBoardBtn : MonoBehaviour
{
    GameManager gameManager;
    int num;
    TextMeshProUGUI btnText;

    Coroutine animCoroutine;

    void Awake()
    {
        btnText = GetComponentInChildren<TextMeshProUGUI>();
        GetComponent<Button>().onClick.AddListener(clickBtn);
    }

    public void init(GameManager gameManager, int num)
    {
        this.gameManager = gameManager;
        this.num = num;
        btnText.text = num.ToString();
    }

    public void clickBtn()
    {
        gameManager.clickNum(this, num);
    }

    public void showWrong()
    {
        if (animCoroutine == null)
        {
            animCoroutine = StartCoroutine(showWrongAnim());
        }
    }

    IEnumerator showWrongAnim()
    {
        Color color = btnText.color;
        btnText.color = Color.red;
        yield return new WaitForSeconds(0.5f);
        btnText.color = color;
        animCoroutine = null;
    }
}