using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
//using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

public class SlowMotionCameraManager : MonoBehaviour
{

    private WebCamTexture webCamTexture;
    private List<Texture2D> capturedFrames = new List<Texture2D>();

    public TextMeshProUGUI load;
    public TextMeshProUGUI prompt;
    public Slider timeIntervalSlider;
    public Slider rateSlider;
    public Slider remainSlider;
    public Button button;

    [SerializeField] float timer = 0f;
    [SerializeField] float slowMotionRate = 0.5f;
    int totalFrameCount = 0;
    [SerializeField] float timeInterval = 4f;
    [SerializeField] float waiter = 0;
    [SerializeField] bool recording = false;
    bool wait = true;
    [SerializeField] float readyTime;


    float displayWidth, displayHeight;
    [SerializeField] float textureScaleDown;


    public RawImage delayDisplay;
    bool initDisplaySize = false;

    void Start()
    {
        //delayDisplay.rectTransform.sizeDelta = new Vector2(webCamTexture.width, webCamTexture.height);
        //float scale = (float)Screen.height / webCamTexture.height;
        //delayDisplay.rectTransform.localScale = new Vector2(-scale, scale);

        if (SystemInfo.deviceType == DeviceType.Handheld)
            delayDisplay.rectTransform.localRotation = Quaternion.Euler(0, 0, -90);

        timer = 0;
        recording = false;
        resetLoadingTime();
        remainSlider.gameObject.SetActive(false);


    }

    void Update()
    {
        if (!webCamTexture.isPlaying) return;

        // init display size
        if (!initDisplaySize && webCamTexture.isPlaying)
        {
            resizeDisplay();
        }

        if (wait) {
            if (readyTime > 0)
            {
                readyTime -= Time.deltaTime;
                int seconds = ((int)readyTime);
                load.gameObject.SetActive(true);
                load.SetText(seconds.ToString());
                remainSlider.gameObject.SetActive(false);
                delayDisplay.texture = null;

            }
            else
            {
                wait = false;
                load.gameObject.SetActive(false);
                resetLoadingTime();
                recording = true;
                capturedFrames.Clear();
                timer = 0;
            }
        }

        if (recording) {
            if (timer < timeInterval)
            {
                load.gameObject.SetActive(false);
                remainSlider.gameObject.SetActive(true);
                //delayDisplay.texture = null;
                Texture2D frame = new Texture2D(webCamTexture.width, webCamTexture.height);
                frame.SetPixels(webCamTexture.GetPixels());
                frame.Apply();
                capturedFrames.Add(frame);
                delayDisplay.texture = frame;
                timer += Time.deltaTime;

            }
            else { 
                recording = false;
                remainSlider.gameObject.SetActive(false);
                totalFrameCount = capturedFrames.Count;
                waiter = 0;
            }
        }

        if (!wait && !recording) {
            float secToPlay = timeInterval / slowMotionRate / totalFrameCount;
            if ((capturedFrames.Count > 1) && (waiter >= secToPlay))
            {
                delayDisplay.texture = capturedFrames[1];
                capturedFrames[0].hideFlags = HideFlags.HideAndDontSave;
                Destroy(capturedFrames[0]);
                capturedFrames.RemoveAt(0);
                waiter = 0;
                Debug.Log(capturedFrames.Count);
            }
            else if ((capturedFrames.Count == 1) && !recording) {
                timer = 0;
                capturedFrames.Clear();
                wait = true;
            }
            else if (waiter < secToPlay){
                waiter += Time.deltaTime;
            }
        }
    }

    void OnEnable()
    {
        // Get the device camera
        WebCamDevice[] devices = WebCamTexture.devices;
        foreach (WebCamDevice device in devices)
        {
            if (device.isFrontFacing)
            {
                webCamTexture = new WebCamTexture(device.name);
                break;
            }
        }

        timeIntervalSlider.value = 1;
        rateSlider.value = 0.5f;
        timeInterval = 1f;
        slowMotionRate = 0.5f;
        prompt.gameObject.SetActive(true);
        timeIntervalSlider.gameObject.SetActive(true);
        rateSlider.gameObject.SetActive(true); 
        button.gameObject.SetActive(true);
        resetLoadingTime();

    }

    void OnDisable()
    {
        // Stop the webcam when the GameObject is deactivated
        if (webCamTexture != null && webCamTexture.isPlaying)
        {
            webCamTexture.Stop();
        }

        // Clear the list of captured frames
        foreach (Texture2D frame in capturedFrames)
        {
            frame.hideFlags = HideFlags.HideAndDontSave;
            Destroy(frame);
        }
        capturedFrames.Clear();
        timer = 0;
        load.gameObject.SetActive(false);
        remainSlider.gameObject.SetActive(false);
        delayDisplay.gameObject.SetActive(false);
        resetLoadingTime();
    }

    void OnDestroy()
    {
        // Release the webcam texture when the GameObject is destroyed
        if (webCamTexture != null)
        {
            webCamTexture.Stop();
            Destroy(webCamTexture);
        }
    }

    public void resetLoadingTime() {
        readyTime = 4f;
    }

    public void resetStateValue() {
        wait = false;
        recording = false;
        timer = 0;
        totalFrameCount = 0;
        capturedFrames.Clear();
        waiter = 0;
    }

    public void playWebCam()
    {
        delayDisplay.gameObject.SetActive(true);

        if (webCamTexture != null)
        {
            webCamTexture.Play();
            wait = true;
        }
    }

    void resizeDisplay()
    {
        displayWidth = webCamTexture.width;
        displayHeight = webCamTexture.height;

        float scale = Screen.height / displayHeight;

        delayDisplay.rectTransform.sizeDelta = new Vector2(displayWidth, displayHeight);
        delayDisplay.rectTransform.localScale = new Vector2(-scale, scale);

        initDisplaySize = true;
    }

    public void changeValue(float rate, float interval)
    {
        slowMotionRate = rate;
        timeInterval = interval;
        Debug.Log("Successfully changed value");
    }

    public void disableCamera()
    {
        OnDisable();
    }

    public float getRemainTime()
    {
        return timeInterval - timer;
    }
    public float getInterval()
    {
        return timeInterval;
    }

}