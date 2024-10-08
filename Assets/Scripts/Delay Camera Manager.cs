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
    [SerializeField] float delayTime = 3;
    [SerializeField] int FPS = 60;
    float updateInterval = 1;

    public RawImage delayDisplay;
    public TextMeshProUGUI hintText;

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
        // // Capture frames and save them into the list
        Texture2D frame = new Texture2D(webCamTexture.width, webCamTexture.height);
        frame.SetPixels(webCamTexture.GetPixels());
        frame.Apply();
        if (capturedFrames.Count < 1000)
        {
            capturedFrames.Add(frame);
        }

        timer += Time.deltaTime;
        if (timer > delayTime)
        {
            delayDisplay.texture = capturedFrames[0];
            capturedFrames.RemoveAt(0);

            hintText.gameObject.SetActive(false);
        }
        else
            hintText.gameObject.SetActive(true);
    }

    void OnEnable() {
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

    public void changeDelayTime(int i)
    {
        delayTime = i;
    }
}
