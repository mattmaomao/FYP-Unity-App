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
        float scale = (float)Screen.height / webCamTexture.height;
        delayDisplay.rectTransform.localScale = new Vector2(-scale, scale);

        timer = 0;
        updateInterval = 1f / FPS;
    }

    void Update()
    {
        Texture2D frame = new Texture2D(webCamTexture.width, webCamTexture.height);
        frame.SetPixels(webCamTexture.GetPixels());
        frame.Apply();
        capturedFrames.Add(frame);

        if (capturedFrames.Count > 1)
        {
            delayDisplay.texture = capturedFrames[1];
            capturedFrames[0].hideFlags = HideFlags.HideAndDontSave;
            Destroy(capturedFrames[0]);
            capturedFrames.RemoveAt(0);
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

        webCamTexture.Play();
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
        if (webCamTexture != null)
        {
            webCamTexture.Stop();
            Destroy(webCamTexture);
        }
    }
}