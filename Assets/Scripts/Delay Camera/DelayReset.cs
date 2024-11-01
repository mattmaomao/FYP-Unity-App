using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class DelayReset : MonoBehaviour
{

    public Slider slider;
    public Button button;
    public Button reset;
    public TextMeshProUGUI prompt;
    public DelayCameraManager cameraManager;
    void Start()
    {
        reset.onClick.AddListener(OnButtonClick);
    }

    void OnButtonClick()
    {

        // Get the slider value and hide UI elements
        cameraManager.disableCamera();
        slider.value = 1;
        prompt.gameObject.SetActive(true);
        slider.gameObject.SetActive(true);
        button.gameObject.SetActive(true);

        Debug.Log("Delay time reset");


    }

}
