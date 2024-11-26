using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class NextButton : MonoBehaviour
{
    public Button next;
    public TextMeshProUGUI welcomePrompt;
    public TextMeshProUGUI guide;
    public TextMeshProUGUI explainGuide;
    public Image fullBodyImage;
    public Image legImage;
    public VideoPlayer fullBodyPlayer;
    public VideoPlayer handPlayer;
    public VideoPlayer headPlayer;
    public Button legButton;
    public Button handButton;
    public Button headButton;




    // Start is called before the first frame update
    void Start()
    {
        next.onClick.AddListener(OnButtonClick);
    }

    // Update is called once per frame
    void OnButtonClick()
    {
        if (welcomePrompt.IsActive())
        {
            welcomePrompt.gameObject.SetActive(false);
            guide.gameObject.SetActive(true);
            fullBodyImage.gameObject.SetActive(true);
            legButton.gameObject.SetActive(true);
            handButton.gameObject.SetActive(true);
            headButton.gameObject.SetActive(true);
        }
        else if (guide.IsActive())
        {
            guide.gameObject.SetActive(false);
            fullBodyPlayer.gameObject.SetActive(true);
            fullBodyPlayer.enabled = true;
            legButton.gameObject.SetActive(false);
            handButton.gameObject.SetActive(false);
            headButton.gameObject.SetActive(false);
            explainGuide.text = "FULL";
            explainGuide.gameObject.SetActive(true);
        } else if (fullBodyPlayer.isActiveAndEnabled)
        {
            fullBodyPlayer.gameObject.SetActive(false);
            fullBodyPlayer.enabled = false;
            legButton.gameObject.SetActive(true);
            handButton.gameObject.SetActive(true);
            headButton.gameObject.SetActive(true);
            explainGuide.gameObject.SetActive(false);
            guide.gameObject.SetActive(true);
        } else if (explainGuide.IsActive())
        {
            explainGuide.gameObject.SetActive(false);
            legImage.gameObject.SetActive(false);
            handPlayer.gameObject.SetActive(false);
            handPlayer.enabled = false;
            headPlayer.gameObject.SetActive(false);
            headPlayer.enabled = false;
            fullBodyPlayer.gameObject.SetActive(false);
            fullBodyPlayer.enabled = false;
            legButton.gameObject.SetActive(true);
            handButton.gameObject.SetActive(true);
            headButton.gameObject.SetActive(true);
            guide.gameObject.SetActive(true);

        }
    }

}
