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

    public void init()
    {
        for (int i = 0; i < radioContainer.childCount; i++)
            radioBtns.Add(radioContainer.GetChild(i).gameObject);

        selectRadio(defaultSelection);
    }

    public void selectRadio(int idx)
    {
        foreach (GameObject btn in radioBtns)
            btn.GetComponent<RadioBtn>().setSelected(false);

        radioBtns[idx].GetComponent<RadioBtn>().setSelected(true);
        selection = idx;
    }
}
