using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NavigationManager : MonoBehaviour
{
    // pages
    public List<GameObject> pages = new List<GameObject>();
    int currentPageIdx = 0;

    [Header("Header, Footer")]
    [SerializeField] TextMeshProUGUI headerText;
    [SerializeField] GameObject headerBackBtn;
    [SerializeField] GameObject footer;

    [Header("Footer")]
    [SerializeField] Image homeIcon;
    [SerializeField] Image userIcon;
    [SerializeField] Image settingIcon;
    [SerializeField] Sprite homeIconSprite;
    [SerializeField] Sprite userIconSprite;
    [SerializeField] Sprite settingIconSprite;
    [SerializeField] Sprite homeIconSprite_filled;
    [SerializeField] Sprite userIconSprite_filled;
    [SerializeField] Sprite settingIconSprite_filled;

    List<int> pagesStack = new();


    // Start is called before the first frame update
    void Start()
    {
        ChangePage(0);
        pagesStack.Add(0);
    }

    public void ChangePage(int i, bool isBack)
    {
        // todo
        // make animation
        foreach (GameObject page in pages)
            page.SetActive(false);

        pages[i].SetActive(true);
        currentPageIdx = i;

        // change header text
        headerText.text = pages[i].name;

        // show back btn
        headerBackBtn.SetActive(i >= 3);

        // show footer
        footer.SetActive(i < 3);
        changeFooterIcon(i);

        if (i >= 3 && !isBack)
            pagesStack.Add(i);
    }
    public void ChangePage(int i) {
        ChangePage(i, false);
    }


    public void previousPage()
    {
        if (pagesStack.Count <= 1)
        {
            ChangePage(0, true);
            return;
        }

        int i = pagesStack[^2];
        pagesStack.RemoveAt(pagesStack.Count - 1);
        ChangePage(i, true);
    }

    void changeFooterIcon(int i)
    {
        if (i >= 3) return;

        // reset all icon
        homeIcon.sprite = homeIconSprite;
        userIcon.sprite = userIconSprite;
        settingIcon.sprite = settingIconSprite;

        switch (i)
        {
            case 0:
                homeIcon.sprite = homeIconSprite_filled;
                break;
            case 1:
                userIcon.sprite = userIconSprite_filled;
                break;
            case 2:
                settingIcon.sprite = settingIconSprite_filled;
                break;
            default:
                break;
        }
    }
}
