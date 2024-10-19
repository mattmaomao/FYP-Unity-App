using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

public class SlowMotionCameraManager : MonoBehaviour
{

    private WebCamTexture webCamTexture;
    private List<Texture2D> capturedFrames = new List<Texture2D>();

    public TextMeshProUGUI load;
    public TextMeshProUGUI prompt;
    public Slider timeIntervalSlider;
    public Slider rateSlider; 
    public Button button;

    [SerializeField] float timer = 0f;
    [SerializeField] static int FPS = 30;
    [SerializeField] static float slowMotionRate = 0.5f;
    [SerializeField] float frameInterval = 1f / FPS;
    [SerializeField] float timeInterval = 4f;
    [SerializeField] float waiter = 0;
    [SerializeField] bool recording = false;

    float displayWidth, displayHeight;
    [SerializeField] float textureScaleDown;


    public RawImage delayDisplay;

    void Start()
    {
        delayDisplay.rectTransform.sizeDelta = new Vector2(webCamTexture.width, webCamTexture.height);
        float scale = (float)Screen.height / webCamTexture.height;
        delayDisplay.rectTransform.localScale = new Vector2(-scale, scale);

        timer = 0;
        recording = false;

    }

    void Update()
    {
        if (!webCamTexture.isPlaying) return;


        if (recording &&(timer < timeInterval))
        {
            load.gameObject.SetActive(true);
            delayDisplay.texture = null;
            Texture2D frame = new Texture2D(webCamTexture.width, webCamTexture.height);
            frame.SetPixels(webCamTexture.GetPixels());
            frame.Apply();
            capturedFrames.Add(frame);
            timer += frameInterval;
        } else { recording = false; }

        if (!recording)
        {
            load.gameObject.SetActive(false);   
        }

        if ((capturedFrames.Count > 1) && (!recording) && (waiter >= (1f / (FPS * slowMotionRate))))
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
            recording = true;
        }
        else if (waiter < (1f / (FPS * slowMotionRate))){
            waiter += frameInterval;
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
        prompt.gameObject.SetActive(true);
        timeIntervalSlider.gameObject.SetActive(true);
        rateSlider.gameObject.SetActive(true); 
        button.gameObject.SetActive(true);


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

    public void playWebCam()
    {
        if (webCamTexture != null)
        {
            //displayWidth = 1280 / textureScaleDown;
            //displayHeight = 720 / textureScaleDown;

            //float scale = Screen.height / displayHeight;

            //delayDisplay.rectTransform.sizeDelta = new Vector2(displayWidth, displayHeight);
            //delayDisplay.rectTransform.localScale = new Vector2(-scale, scale);

            //webCamTexture.requestedWidth = (int)displayWidth;
            //webCamTexture.requestedHeight = (int)displayHeight;

            webCamTexture.Play();
            recording = true;
        }
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

}