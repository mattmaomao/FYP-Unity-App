using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderButtonHandler : MonoBehaviour
{
    public Slider slider;
    public Button button;
    public TextMeshProUGUI load;
    public TextMeshProUGUI prompt;
    public DelayCameraManager delayMan;
    int delayTime;

    void Start()
    {
        button.onClick.AddListener(OnButtonClick);
        load.gameObject.SetActive(false);
    }

    void OnButtonClick()
    {

        // Get the slider value and hide UI elements
        float sliderValue = slider.value;
        prompt.gameObject.SetActive(false);
        slider.gameObject.SetActive(false);
        button.gameObject.SetActive(false);
        load.gameObject.SetActive(true);

        // Set the delay time and update the DelayCameraManager
        delayTime = (int)sliderValue;
        delayMan.changeDelayTime(delayTime);

        Debug.Log("Success: Delay time changed to " + delayTime);


    }
}