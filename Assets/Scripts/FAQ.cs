using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using TMPro;


public class FAQ : MonoBehaviour
{
    public TMP_Dropdown menu;
    public ScrollRect about;
    public ScrollRect cameras;
    public ScrollRect posture;
    public ScrollRect scores;
    public ScrollRect timer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (menu.options[menu.value].text) {
            case "Cameras":
                posture.gameObject.SetActive(false);
                scores.gameObject.SetActive(false);
                timer.gameObject.SetActive(false);
                about.gameObject.SetActive(false);
                cameras.gameObject.SetActive(true); break;
            case "Posture":
                cameras.gameObject.SetActive(false);
                scores.gameObject.SetActive(false);
                timer.gameObject.SetActive(false);
                about.gameObject.SetActive(false); 
                posture.gameObject.SetActive(true); break;
            case "Scores":
                posture.gameObject.SetActive(false);
                cameras.gameObject.SetActive(false);
                timer.gameObject.SetActive(false);
                about.gameObject.SetActive(false); 
                scores.gameObject.SetActive(true); break;
            case "Timer":
                posture.gameObject.SetActive(false);
                scores.gameObject.SetActive(false);
                cameras.gameObject.SetActive(false);
                about.gameObject.SetActive(false); 
                timer.gameObject.SetActive(true); break;
            default:
                posture.gameObject.SetActive(false);
                scores.gameObject.SetActive(false);
                timer.gameObject.SetActive(false);
                cameras.gameObject.SetActive(false); 
                about.gameObject.SetActive(true); break;
        }
    }
}
