using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

namespace test
{
    public class DigitImage
    {
        public byte[][] pixels;
        public byte[] arrayPixels;
        public byte label;
        

        public DigitImage(byte[][] _pixels, byte _label, byte[] _arrayPixels)
        {
            this.pixels = new byte[28][];
            for (int i = 0; i < this.pixels.Length; ++i)
                this.pixels[i] = new byte[28];

            for (int i = 0; i < 28; ++i)
                for (int j = 0; j < 28; ++j)
                    this.pixels[i][j] = _pixels[i][j];


            this.label = _label;           


            this.arrayPixels = new byte[784];
            for (int z = 0; z < 784; z++)
            {
                this.arrayPixels[z] = _arrayPixels[z];
            }
        }

        public override string ToString()
        {
            string s = "";
            for (int i = 0; i < 28; ++i)
            {
                for (int j = 0; j < 28; ++j)
                {
                    if (this.pixels[i][j] == 0)
                        s += " "; // white
                    else if (this.pixels[i][j] == 255)
                        s += "O"; // black
                    else
                        s += "."; // gray
                }
                s += "\n";
            }
            s += this.label.ToString();
            return s;
        } // ToString

    }

    public class MyUbyteParser : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI debugText;
        MyNeuralNetwork neuralNetwork;

        byte[][] pixels;
        byte[] arrayPixels;
        int k = 0;

        int fileDataCount = 60000;

        MyScreenControls screen;

        FileStream ifsLabels;
        FileStream ifsImages;
        BinaryReader brLabels;
        BinaryReader brImages;


        DigitImage[] allImages;

        int miniBatchSize = 8;
        int[] randomIndexes;


        bool ready = false;

        bool start = false;

        public bool IsRunning()
        {
            return start;
        }

        private void Update()
        {
            if (!ready)
            {
                return;
            }

        }

        public void StartLearning()
        {
            start = true;
        }

        public void PauseLearning()
        {
            start = false;
        }

        public void CustomStart()
        {
            neuralNetwork = gameObject.GetComponent<MyNeuralNetwork>();
            screen = GameObject.Find("Screen Control").GetComponent<MyScreenControls>();

            pixels = new byte[28][];

            for (int i = 0; i < pixels.Length; ++i)
            {
                pixels[i] = new byte[28];
            }

            arrayPixels = new byte[784];

            randomIndexes = new int[miniBatchSize];
            GenerateRandomIndex();

            allImages = new DigitImage[fileDataCount];

            OpenFiles();
            ParseImages(fileDataCount);
            CloseFiles();

            ready = true;

        }

        private void OpenFiles()
        {
            debugText.text += "parser try\n";

            ifsLabels = new FileStream(Application.streamingAssetsPath + @"/PictureData\train-labels.idx1-ubyte", FileMode.Open); // test labels
            ifsImages = new FileStream(Application.streamingAssetsPath + @"/PictureData\train-images.idx3-ubyte", FileMode.Open); // test images

            debugText.text += "opened\n";

            brLabels = new BinaryReader(ifsLabels);
            brImages = new BinaryReader(ifsImages);
            debugText.text += "binary\n";

            int magic1 = brImages.ReadInt32(); // discard
            int numImages = brImages.ReadInt32();
            int numRows = brImages.ReadInt32();
            int numCols = brImages.ReadInt32();

            int magic2 = brLabels.ReadInt32();
            int numLabels = brLabels.ReadInt32();
        }

        private void CloseFiles()
        {
            ifsImages.Close();
            brImages.Close();
            ifsLabels.Close();
            brLabels.Close();
        }

        private void GenerateRandomIndex()
        {
            for (int i = 0; i < miniBatchSize; i++)
            {
                randomIndexes[i] = (int)UnityEngine.Random.Range(0, fileDataCount);
            }
        }

        public void ParseImages(int pictureCount)
        {

            // each test image
            for (int di = 0; di < pictureCount; di++)
            {
                for (int i = 0; i < 28; i++)
                {
                    for (int j = 0; j < 28; j++)
                    {
                        byte b = brImages.ReadByte();
                        pixels[i][j] = b;
                        arrayPixels[k] = b;
                        k++;
                    }
                }
                k = 0;

                byte lbl = brLabels.ReadByte();

                allImages[di] = new DigitImage(pixels, lbl, arrayPixels);


            } // each image        

        }

        private void OnApplicationQuit()
        {
            CloseFiles();
        }

        private DigitImage LoadImage(string filePath)
        {
            // Load the image as a Texture2D
            Texture2D texture = new Texture2D(28, 28);
            byte[] fileData = File.ReadAllBytes(filePath);
            texture.LoadImage(fileData); // Load the image data into the texture

            // Prepare the pixel arrays
            byte[][] imagePixels = new byte[28][];
            byte[] arrayPixels = new byte[784];

            // Convert texture pixels to byte arrays
            for (int y = 0; y < 28; y++)
            {
                imagePixels[y] = new byte[28];
                for (int x = 0; x < 28; x++)
                {
                    Color color = texture.GetPixel(x, y);
                    byte pixelValue = (byte)(color.grayscale * 255); // Convert to grayscale
                    imagePixels[y][x] = pixelValue;
                    arrayPixels[y * 28 + x] = pixelValue;
                }
            }

            // Create and return the DigitImage
            byte label = 0; // You can assign a label if needed
            return new DigitImage(imagePixels, label, arrayPixels);
        }

        public int ProcessImage(string filePath)
        {
            Debug.Log("the file being processed is: " + filePath);
            DigitImage digitImage = LoadImage(filePath);
            // Now you can use digitImage as needed, e.g., feed it to the neural network
            neuralNetwork.StartCycle(digitImage);
            return neuralNetwork.GetRecognizedValue();
        }
    }


}

