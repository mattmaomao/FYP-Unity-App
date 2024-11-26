using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class HandButton : MonoBehaviour
{
    public TextMeshProUGUI guide;
    public TextMeshProUGUI explainGuide;
    public VideoPlayer handPlayer;
    public Button legButton;
    public Button handButton;
    public Button headButton;



    // Start is called before the first frame update
    void Start()
    {
        handButton.onClick.AddListener(OnButtonClick);
    }

    // Update is called once per frame
    void OnButtonClick()
    {
        legButton.gameObject.SetActive(false);
        handButton.gameObject.SetActive(false);
        headButton.gameObject.SetActive(false);
        handPlayer.gameObject.SetActive(true);
        handPlayer.enabled = true;
        guide.gameObject.SetActive(false);
        explainGuide.text = "HAND";
        explainGuide.gameObject.SetActive(true);
        
    }

}
