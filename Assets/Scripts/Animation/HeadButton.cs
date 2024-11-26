using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class HeadButton : MonoBehaviour
{
    public TextMeshProUGUI guide;
    public TextMeshProUGUI explainGuide;
    public VideoPlayer headPlayer;
    public Button legButton;
    public Button handButton;
    public Button headButton;



    // Start is called before the first frame update
    void Start()
    {
        headButton.onClick.AddListener(OnButtonClick);
    }

    // Update is called once per frame
    void OnButtonClick()
    {
        legButton.gameObject.SetActive(false);
        handButton.gameObject.SetActive(false);
        headButton.gameObject.SetActive(false);
        headPlayer.gameObject.SetActive(true);
        headPlayer.enabled = true;
        guide.gameObject.SetActive(false);
        explainGuide.text = "HEAD";
        explainGuide.gameObject.SetActive(true);
        
    }

}
