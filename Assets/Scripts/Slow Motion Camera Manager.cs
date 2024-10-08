using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlowMotionCameraManager : MonoBehaviour
{

    private WebCamTexture webCamTexture;
    private List<Texture2D> capturedFrames = new List<Texture2D>();

    [SerializeField] float timer = 0f;
    [SerializeField] int FPS = 30;
    [SerializeField] float updateInterval = 1;
    [SerializeField] float _fps;

    public RawImage delayDisplay;

    void Start()
    {
        delayDisplay.rectTransform.sizeDelta = new Vector2(webCamTexture.width, webCamTexture.height);

        timer = 0;
        updateInterval = 1f / FPS;
    }

    void Update()
    {
        // // Capture frames and save them into the list
        Texture2D frame = new Texture2D((int)(webCamTexture.height / delayDisplay.rectTransform.sizeDelta.y * webCamTexture.width), (int)delayDisplay.rectTransform.sizeDelta.y);
        frame.SetPixels(webCamTexture.GetPixels());
        frame.Apply();
        if (capturedFrames.Count < 1000)
        {
            capturedFrames.Add(frame);
        }

        // limit fps by device
        _fps = 1 / Time.deltaTime;
        updateInterval = 1f / Mathf.Min(FPS, _fps);

        timer += Time.deltaTime;
        // fps control
        if (timer > updateInterval)
        {
            timer = 0;
            delayDisplay.texture = capturedFrames[0];
        }
        capturedFrames.RemoveAt(0);
    }

    void OnEnable()
    {
        // Get the device camera
        webCamTexture = new WebCamTexture();
        webCamTexture.Play();
    }

    void OnDisable()
    {
        // Stop the webcam when the GameObject is deactivated
        if (webCamTexture != null && webCamTexture.isPlaying)
        {
            webCamTexture.Stop();
        }
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

    public void changeFPS(int i)
    {
        FPS = i;
    }
}
