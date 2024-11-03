using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class SlowMotionReset : MonoBehaviour
{

    public Slider timeIntervalSlider;
    public Slider rateSlider;
    public Button button;
    public Button reset;
    public TextMeshProUGUI prompt;
    public SlowMotionCameraManager cameraManager;
    void Start()
    {
        reset.onClick.AddListener(OnButtonClick);
    }

    void OnButtonClick()
    {

        // Get the slider value and hide UI elements
        cameraManager.disableCamera();
        timeIntervalSlider.value = 1;
        rateSlider.value = 0.5f;
        prompt.gameObject.SetActive(true);
        timeIntervalSlider.gameObject.SetActive(true);
        rateSlider.gameObject.SetActive(true);
        button.gameObject.SetActive(true);

        Debug.Log("Slow Motion reset");


    }

}
