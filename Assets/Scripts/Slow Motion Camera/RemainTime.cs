using UnityEngine;
using UnityEngine.UI;

public class RemainTime : MonoBehaviour
{
    // Start is called before the first frame update
    public Slider slider;
    public SlowMotionCameraManager slowMotionCameraManager;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        slider.maxValue = slowMotionCameraManager.getInterval();
        slider.value = slowMotionCameraManager.getRemainTime();
    }
}
