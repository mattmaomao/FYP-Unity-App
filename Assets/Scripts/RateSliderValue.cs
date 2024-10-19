using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RateSliderValue : MonoBehaviour
{

    public Slider slider;
    public TextMeshProUGUI textComp;

    // Start is called before the first frame update
    void Start()
    {
        slider.value = 0.5F;
    }

    // Update is called once per frame
    void Update()
    {
        textComp.text = slider.value.ToString("0.00") + "x";
    }
}
