using System.Collections;
using System.Collections.Generic;
using Unity.Theme;
using UnityEngine;
using UnityEngine.UI;

public class RadioBtn : MonoBehaviour
{
    [SerializeField] Image radioBtns;
    [SerializeField] Sprite notSelectedSprite;
    [SerializeField] Sprite selectedSprite;

    public void setSelected(bool selected) {
        radioBtns.sprite = selected ? selectedSprite : notSelectedSprite;
        GetComponent<Image>().color = !selected ? Color.white : Theme.Instance.GetColorByName("Primary Pale").Color;
    }
}
