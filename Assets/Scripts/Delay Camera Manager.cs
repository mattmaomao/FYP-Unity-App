using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DelayCameraManager : MonoBehaviour
{

    private WebCamTexture webCamTexture;
    private List<Texture2D> capturedFrames = new List<Texture2D>();

    [SerializeField] float timer = 0f;
    [SerializeField] float FPSTimer = 0f;
    [SerializeField] float delayTime = 3;
    [SerializeField] int FPS = 60;
    [SerializeField] int maxFrameStorage = 1000;
    float updateInterval = 1;

    public RawImage delayDisplay;
    float displayWidth, displayHeight;
    public TextMeshProUGUI hintText;
    [SerializeField] float textureScaleDown;

    // debug
    [SerializeField] long usedMB;

    void Start()
    {
        timer = 0;
        updateInterval = 1f / FPS;
    }

    void Update()
    {
        if (!webCamTexture.isPlaying) return;

        timer += Time.deltaTime;
        FPSTimer += Time.deltaTime;
        updateInterval = 1f / Mathf.Min(FPS, 1 / Time.deltaTime);

        // Capture frames and save them into the list
        if (capturedFrames.Count < maxFrameStorage)
        {
            if (FPSTimer > updateInterval)
            {
                Texture2D frame = new Texture2D(webCamTexture.width, webCamTexture.height, TextureFormat.RGB24, false);
                frame.SetPixels(webCamTexture.GetPixels());
                frame.Apply();
                capturedFrames.Add(frame);
            }
        }

        if (timer > delayTime)
        {
            if (FPSTimer > updateInterval)
            {
                delayDisplay.texture = capturedFrames[1];

                capturedFrames[0].hideFlags = HideFlags.HideAndDontSave;
                Destroy(capturedFrames[0]);
                capturedFrames.RemoveAt(0);
            }

            hintText.gameObject.SetActive(false);
        }
        else
            hintText.gameObject.SetActive(true);

        if (FPSTimer > updateInterval)
            FPSTimer = 0;

        // debug
        long memoryUsage = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong();
        usedMB = memoryUsage / (1024 * 1024);
    }

    void OnEnable()
    {
        // Get the device camera
        webCamTexture = new WebCamTexture();
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
    }

    void OnDestroy()
    {
        // Release the webcam texture when the GameObject is destroyed
        if (webCamTexture != null && !webCamTexture.isPlaying)
        {
            webCamTexture.Stop();
            Destroy(webCamTexture);
        }
    }

    public void playWebCam()
    {
        if (webCamTexture != null)
        {
            displayWidth = 1280 / textureScaleDown;
            displayHeight = 720 / textureScaleDown;

            float scale = Screen.height / displayHeight;

            delayDisplay.rectTransform.sizeDelta = new Vector2(displayWidth, displayHeight);
            delayDisplay.rectTransform.localScale = new Vector2(-scale, scale);

            webCamTexture.requestedWidth = (int)displayWidth;
            webCamTexture.requestedHeight = (int)displayHeight;

            webCamTexture.Play();
        }
    }

    public void changeDelayTime(int i)
    {
        delayTime = i;
    }
}
