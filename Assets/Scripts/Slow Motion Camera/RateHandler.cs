using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RateHandler : MonoBehaviour
{
    public Slider timeIntervalSlider;
    public Slider rateSlider;
    public Button button;
    public TextMeshProUGUI load;
    public TextMeshProUGUI prompt;
    public SlowMotionCameraManager slowMotionMan;
    int delayTime;

    void Start()
    {
        button.onClick.AddListener(OnButtonClick);
        load.gameObject.SetActive(false);
    }

    void OnButtonClick()
    {

        // Get the slider value and hide UI elements
        float timeInterval = timeIntervalSlider.value;
        float rate = rateSlider.value;
        prompt.gameObject.SetActive(false);
        timeIntervalSlider.gameObject.SetActive(false);
        rateSlider.gameObject.SetActive(false);
        button.gameObject.SetActive(false);
        load.gameObject.SetActive(true);

        // Set the delay time and update the DelayCameraManager
        slowMotionMan.changeValue(rate, timeInterval);

        Debug.Log("Success: Rate changed to " + rate + "x and time interval changed to " + timeInterval);


    }
}