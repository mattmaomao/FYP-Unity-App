using UnityEngine;
using UnityEngine.UI;

public class SliderValueText : MonoBehaviour
{
    private Slider slider;
    private Text textComp;

    void Awake()
    {
        slider = GetComponentInParent<Slider>();
        textComp = GetComponent<Text>();
    }

    void Start()
    {
        UpdateText(slider.value);
        slider.onValueChanged.AddListener(UpdateText);
    }

    void UpdateText(float val)
    {
        textComp.text = slider.value.ToString() + " s";
    }
}
