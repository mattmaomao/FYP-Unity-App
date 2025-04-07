using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using test;
using System.IO;
using TMPro;



public class MyScreenControls : MonoBehaviour
{
    [SerializeField] GameObject manager;
    MyUbyteParser parser;
    MyNeuralNetwork NN;

    // Camera-related variables
    private bool useCameraInput = true;
    private WebCamTexture webCamTexture;
    private string selectedCamera = "";
    [SerializeField] private RawImage cameraPreview; // Reference to a UI RawImage to display camera feed

    Color eraseColor;
    byte[] pixelData;

    [Header("Display")]
    [SerializeField] List<UnityEngine.UI.Image> displayList;
    [SerializeField] List<TextMeshProUGUI> resultTextList;
    [SerializeField] RectTransform grid;
    [SerializeField] RawImage tempImg;

    [Header("Grid Settings")]
    private int rows = 12;
    private int columns = 3;
    private int cellWidth = 64;
    private int cellHeight = 42;

    private List<Texture2D> cellTextures = new List<Texture2D>();
    private List<string> savedTexturePaths = new();

    [Header("Debug")]
    [SerializeField] RawImage debugImage;
    [SerializeField] TextMeshProUGUI debugText;
    [SerializeField] TextMeshProUGUI debugText2;
    [SerializeField] int debugIndex = 0;

    public void TryImg()
    {
        debugImage.texture = cellTextures[debugIndex];
        int x = parser.ProcessImage(savedTexturePaths[debugIndex]);
        debugText.text = x.ToString();
        debugIndex++;
        if (debugIndex >= cellTextures.Count)
        {
            debugIndex = 0;
        }
    }

    public void CaptureBtn()
    {
        StartCoroutine(Capture());
    }

    IEnumerator Capture()
    {
        // Capture the texture of the RawImage
        Texture2D texture = new Texture2D(webCamTexture.width, webCamTexture.height);

        texture.SetPixels(webCamTexture.GetPixels());
        texture.Apply();

        // Encode the texture to a PNG byte array
        byte[] bytes = texture.EncodeToPNG();

        // Specify the file path to save the image
        string filePath = Application.persistentDataPath + "/savedImage.png";

        // Save the byte array to a file
        File.WriteAllBytes(filePath, bytes);

        Debug.Log("Image saved to: " + filePath);
        tempImg.texture = texture;
        CaptureAndSliceGrid(texture);
        Debug.Log("sliced");

        for (int i = 0; i < cellTextures.Count; i++)
        {
            DisplayCellTexture(cellTextures.Count - 1 - i, displayList[i]);
        }
        // SaveDisplayListImage(displayList[0]);

        yield return new WaitForSeconds(0.1f);

        Debug.Log("Start processing images");
        // process each image
        for (int i = 0; i < savedTexturePaths.Count; i++)
        {
            int x = parser.ProcessImage(savedTexturePaths[i]);
            Debug.Log($"recognized: {x}");
            resultTextList[i].text = x.ToString();
            yield return new WaitForSeconds(0.1f);
        }

        Debug.Log("finish");
        yield return null;
    }

    private void SaveDisplayListImage(UnityEngine.UI.Image image)
    {
        // Create a Texture2D from the Image component
        RenderTexture renderTexture = new RenderTexture((int)image.rectTransform.rect.width, (int)image.rectTransform.rect.height, 24);
        RenderTexture.active = renderTexture;

        // Copy the UI Image to the RenderTexture
        Graphics.Blit(image.sprite.texture, renderTexture);

        // Create a new Texture2D to read the pixels
        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height);
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();

        // Encode the Texture2D to PNG
        byte[] bytes = texture.EncodeToPNG();
        string filePath = Application.persistentDataPath + "/displayListImage.png";
        File.WriteAllBytes(filePath, bytes);

        Debug.Log("DisplayList[0] image saved to: " + filePath);

        // Clean up
        RenderTexture.active = null;
        Destroy(renderTexture);
        Destroy(texture);
    }

    public void CaptureAndSliceGrid(Texture2D texture)
    {
        // Clear previous cell textures
        cellTextures.Clear();

        // Calculate the full texture size
        float textureWidth = cameraPreview.GetComponent<RectTransform>().rect.width;
        float textureHeight = cameraPreview.GetComponent<RectTransform>().rect.height;

        // adjust cell size
        float newCellWidth = (int)(cellWidth / textureWidth * texture.width);
        float newCellHeight = (int)(cellHeight / textureHeight * texture.height);
        float gap = 0f;

        // Create a temporary texture to read the pixels
        Texture2D tempTexture = texture;
        savedTexturePaths.Clear();
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                // Calculate cell position
                float x = col * (newCellWidth + gap) + texture.width / 2 - (grid.rect.width / textureWidth * texture.width / 2);
                float y = row * (newCellHeight + gap) + texture.height / 2 - (grid.rect.height / textureHeight * texture.height / 2);

                // Create a new texture for this cell
                Texture2D cellTexture = new Texture2D((int)newCellWidth, (int)newCellHeight, TextureFormat.RGB24, false);

                // Copy the pixels from the temporary texture
                Color[] pixels = tempTexture.GetPixels((int)x, (int)y, (int)newCellWidth, (int)newCellHeight);
                cellTexture.SetPixels(pixels);
                cellTexture.Apply();

                // Add the cell texture to our list
                cellTextures.Add(cellTexture);

                // save each texture to a file
                string filePath = Application.persistentDataPath + $"/cell_{row}_{col}.png";
                byte[] bytes = cellTexture.EncodeToPNG();
                File.WriteAllBytes(filePath, bytes);
                savedTexturePaths.Add(filePath);
            }
        }

        Debug.Log($"Extracted {cellTextures.Count} grid cells");
    }

    // Method to get the cell at a specific index
    public Texture2D GetCellTexture(int index)
    {
        if (index >= 0 && index < cellTextures.Count)
        {
            return cellTextures[index];
        }
        return null;
    }

    // Method to display a cell texture on a UI Image or other component
    public void DisplayCellTexture(int index, UnityEngine.UI.Image targetImage)
    {
        Texture2D cellTexture = GetCellTexture(index);
        if (cellTexture != null && targetImage != null)
        {
            // Create a sprite from the texture
            Sprite sprite = Sprite.Create(cellTexture, new Rect(0, 0, cellTexture.width, cellTexture.height), Vector2.one * 0.5f);
            targetImage.sprite = sprite;
        }
    }

    public void CustomStart()
    {
        debugText2.text = "";
        NN = manager.GetComponent<MyNeuralNetwork>();
        debugText2.text += "1, ";
        parser = manager.GetComponent<MyUbyteParser>();
        debugText2.text += "2, ";
        eraseColor = Color.white;

        pixelData = new byte[784];

        debugText2.text += "3, ";
        if (useCameraInput)
        {
            InitializeCamera();
        }
    }

    private void InitializeCamera()
    {
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length == 0)
        {
            Debug.LogError("No camera detected on this device");
            return;
        }

        debugText2.text += "4\n";
        // If no camera specified or specified camera not found, use the first available
        if (string.IsNullOrEmpty(selectedCamera) ||
            System.Array.FindIndex(devices, device => device.name == selectedCamera) < 0)
        {
            selectedCamera = devices[0].name;
        }

        // choose back camera
        for (int i = 0; i < devices.Length; i++)
        {
            debugText2.text += $"{devices[i].name} ({(devices[i].isFrontFacing ? "T" : "F")}) \n";
            if (SystemInfo.deviceType == DeviceType.Handheld)
            {
                if (!devices[i].isFrontFacing)
                {
                    selectedCamera = devices[i].name;
                    break;
                }
            }
            else
            {
                if (devices[i].isFrontFacing)
                {
                    selectedCamera = devices[i].name;
                    break;
                }
            }
        }

        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            cameraPreview.transform.localRotation = Quaternion.Euler(0, 0, -90);
            cameraPreview.transform.localScale = new Vector3(1, 1, 1);
        }
        webCamTexture = new WebCamTexture(selectedCamera, 1280, 720, 30);
        webCamTexture.Play();

        // If you have a UI element to display the camera feed
        if (cameraPreview != null)
        {
            cameraPreview.texture = webCamTexture;
        }
    }

    private void Update()
    {
        if (cameraPreview != null)
        {
            cameraPreview.texture = webCamTexture;
        }

        debugText.text = NN.GetRecognizedValue().ToString();
    }


    public void ToggleCamera()
    {
        useCameraInput = !useCameraInput;

        if (useCameraInput && webCamTexture == null)
        {
            InitializeCamera();
        }
        else if (!useCameraInput && webCamTexture != null && webCamTexture.isPlaying)
        {
            webCamTexture.Stop();
        }
        else if (useCameraInput && webCamTexture != null && !webCamTexture.isPlaying)
        {
            webCamTexture.Play();
        }
    }

    void OnDestroy()
    {
        if (webCamTexture != null && webCamTexture.isPlaying)
        {
            webCamTexture.Stop();
        }
    }

}
