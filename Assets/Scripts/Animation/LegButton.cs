using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class LegButton : MonoBehaviour
{
    public TextMeshProUGUI guide;
    public TextMeshProUGUI explainGuide;
    public Image legImage;
    public Button legButton;
    public Button handButton;
    public Button headButton;



    // Start is called before the first frame update
    void Start()
    {
        legButton.onClick.AddListener(OnButtonClick);
    }

    // Update is called once per frame
    void OnButtonClick()
    {
        legButton.gameObject.SetActive(false);
        handButton.gameObject.SetActive(false);
        headButton.gameObject.SetActive(false);
        legImage.gameObject.SetActive(true);
        guide.gameObject.SetActive(false);
        explainGuide.text = "Feet parallel to the shooting line, positioned at approximately shoulder width apart, with a little more weight on the front of the feet";
        explainGuide.gameObject.SetActive(true);
        
    }

}
