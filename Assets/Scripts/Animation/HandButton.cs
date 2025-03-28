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
        explainGuide.text = "For right hand, the index finger, middle finger and ring finger hook the string in the knuckle furthest from the hand.\n\nFor left hand, relax the wrist and bends to match the angle of the bow grip.";
        explainGuide.gameObject.SetActive(true);
        
    }

}
