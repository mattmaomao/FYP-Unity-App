using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadioSelection : MonoBehaviour
{
    [SerializeField] Transform radioContainer;
    List<GameObject> radioBtns = new();
    [SerializeField] int defaultSelection;
    public int selection;

    void Start()
    {
        init();
    }

    public void init()
    {
        for (int i = 0; i < radioContainer.childCount; i++)
            radioBtns.Add(radioContainer.GetChild(i).gameObject);

        // // add listeners to btns
        // for (int i = 0; i < radioBtns.Count; i++)
        // {
        //     Button btn = radioBtns[i].GetComponent<Button>();
        //     // btn.onClick.AddListener(() => { selectRadio(bruh); });
        // }

        selectRadio(defaultSelection);
    }

    public void initSelection(int idx = -1)
    {
        if (idx == -1)
            selectRadio(defaultSelection);
        else
            selectRadio(idx);
    }

    public void selectRadio(int idx)
    {
        foreach (GameObject btn in radioBtns)
            btn.GetComponent<RadioBtn>().setSelected(false);

        radioBtns[idx].GetComponent<RadioBtn>().setSelected(true);
        selection = idx;
    }
}
