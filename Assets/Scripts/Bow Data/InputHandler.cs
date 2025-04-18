using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;
using System.IO;


public class InputHandler : MonoBehaviour
{
    [SerializeField] GameObject bowBtns;
    [SerializeField] GameObject inputArea;
    public int index;
    public Slider LRH;
    public TMP_Dropdown handleBrand;
    public TMP_InputField handleNameInput;
    public TextMeshProUGUI handleName;
    public TMP_Dropdown limbBrand;
    public TMP_InputField limbNameInput;
    public TextMeshProUGUI limbName;
    public TMP_Dropdown stringSize;
    public TMP_Dropdown stringStrand;
    public TMP_InputField stringMaterialInput;
    public TextMeshProUGUI stringMaterial;
    public TMP_Dropdown servingSize;
    public TMP_Dropdown servingBrand;
    public TMP_InputField servingMaterialInput;
    public TextMeshProUGUI servingMaterial;
    public TMP_Dropdown plungerBrand;
    public TMP_InputField plungerNameInput;
    public TextMeshProUGUI plungerName;
    public TMP_Dropdown sightBrand;
    public TMP_InputField sightNameInput;
    public TextMeshProUGUI sightName;
    bool RH;
    public TMP_InputField Info;
    public TextMeshProUGUI button0;
    public TextMeshProUGUI button1;
    public TextMeshProUGUI button2;
    public TextMeshProUGUI button3;
    public TextMeshProUGUI button4;
    public TextMeshProUGUI button5;


    List<InputEntry> bow = new List<InputEntry>();
    [SerializeField] string filename;

    void Start()
    {
        inputArea.SetActive(false);
    }
    
    void OnEnable()
    {
        LoadAllButtonName();
        bowBtns.SetActive(true);
    }

    void OnDisable()
    {
        inputArea.SetActive(false);
    }

    public void AddNameToList()
    {
        string bowcontent = File.ReadAllText(Application.persistentDataPath + "/" + filename);
        bow.Clear();
        bow = new();
        bow = JsonConvert.DeserializeObject<List<InputEntry>>(bowcontent);

        if (LRH.value == 1)
        {
            RH = true;
        }
        else RH = false;

        if (index >= bow.Count)
            bow.Add(new InputEntry(RH, handleBrand.options[handleBrand.value].text, handleName.text, limbBrand.options[limbBrand.value].text, limbName.text, stringSize.options[stringSize.value].text, stringStrand.options[stringStrand.value].text, stringMaterial.text, servingSize.options[servingSize.value].text, servingBrand.options[servingBrand.value].text, servingMaterial.text, plungerBrand.options[plungerBrand.value].text, plungerName.text, sightBrand.options[sightBrand.value].text, sightName.text, Info.text));
        else
            bow[index] = new InputEntry(RH, handleBrand.options[handleBrand.value].text, handleName.text, limbBrand.options[limbBrand.value].text, limbName.text, stringSize.options[stringSize.value].text, stringStrand.options[stringStrand.value].text, stringMaterial.text, servingSize.options[servingSize.value].text, servingBrand.options[servingBrand.value].text, servingMaterial.text, plungerBrand.options[plungerBrand.value].text, plungerName.text, sightBrand.options[sightBrand.value].text, sightName.text, Info.text);

        string content = JsonConvert.SerializeObject(bow, Formatting.Indented);
        File.WriteAllText(Application.persistentDataPath + "/" + filename, content);
        Debug.Log(Application.persistentDataPath + "/" + filename);

        LoadAllButtonName();
    }

    // delete data
    public void DeleteBowData() 
    {
        string bowcontent = File.ReadAllText(Application.persistentDataPath + "/" + filename);
        bow.Clear();
        bow = new();
        bow = JsonConvert.DeserializeObject<List<InputEntry>>(bowcontent);
        bow.RemoveAt(index);

        string content = JsonConvert.SerializeObject(bow, Formatting.Indented);
        File.WriteAllText(Application.persistentDataPath + "/" + filename, content);
        Debug.Log(Application.persistentDataPath + "/" + filename);

        LoadAllButtonName();
    }

    public void ChangeIndex(int index)
    {
        this.index = index;
    }

    public void LoadBowData(InputEntry entry)
    {
        if (entry.RH == true)
        {
            LRH.value = 1;
        }
        else LRH.value = 0;
        handleBrand.value = handleBrand.options.FindIndex(option => option.text == entry.handleBrand);
        handleNameInput.text = entry.handleName;
        limbBrand.value = limbBrand.options.FindIndex(option => option.text == entry.limbBrand);
        limbNameInput.text = entry.limbName;
        stringSize.value = stringSize.options.FindIndex(option => option.text == entry.stringSize);
        stringStrand.value = stringStrand.options.FindIndex(option => option.text == entry.stringStrand);
        stringMaterialInput.text = entry.stringMaterial;
        servingSize.value = servingSize.options.FindIndex(option => option.text == entry.servingSize);
        servingBrand.value = servingBrand.options.FindIndex(option => option.text == entry.servingBrand);
        servingMaterialInput.text = entry.servingMaterial;
        plungerBrand.value = plungerBrand.options.FindIndex(option => option.text == entry.plungerBrand);
        plungerNameInput.text = entry.plungerName;
        sightBrand.value = sightBrand.options.FindIndex(option => option.text == entry.sightBrand);
        sightNameInput.text = entry.sightName;
        Info.text = entry.Info;
    }

    public void ResetBowData()
    {
        Info.text = "";
        LRH.value = 1;
        handleBrand.value = 0;
        handleNameInput.text = "";
        limbBrand.value = 0;
        limbNameInput.text = "";
        stringSize.value = 0;
        stringStrand.value = 0;
        stringMaterialInput.text = "";
        servingSize.value = 0;
        servingBrand.value = 0;
        servingMaterialInput.text = "";
        plungerBrand.value = 0;
        plungerNameInput.text = "";
        sightBrand.value = 0;
        sightNameInput.text = "";
    }

    public void LoadNameFromList()
    {
        if (File.Exists(Application.persistentDataPath + "/" + filename))
        {
            string content = File.ReadAllText(Application.persistentDataPath + "/" + filename);
            bow.Clear();
            bow = new();
            bow = JsonConvert.DeserializeObject<List<InputEntry>>(content);

            if (index >= 0 && index < bow.Count)
                LoadBowData(bow[index]);
            else
                ResetBowData();

            Debug.Log("Data loaded from file successfully. " + Application.persistentDataPath + "/" + filename);
        }
        else
        {
            bow.Clear();
            bow = new();
        }

    }

    // load all name to show all option when start
    void LoadAllButtonName()
    {
        // disable all btn at start
        button0.transform.parent.gameObject.SetActive(false);
        button1.transform.parent.gameObject.SetActive(false);
        button2.transform.parent.gameObject.SetActive(false);
        button3.transform.parent.gameObject.SetActive(false);
        button4.transform.parent.gameObject.SetActive(false);
        button5.transform.parent.gameObject.SetActive(false);

        // load all name from file
        if (File.Exists(Application.persistentDataPath + "/" + filename))
        {
            string content = File.ReadAllText(Application.persistentDataPath + "/" + filename);
            bow.Clear();
            bow = new();
            bow = JsonConvert.DeserializeObject<List<InputEntry>>(content);

            int bowNum = bow.Count;
            Debug.Log(bowNum);

            for (int i = 0; i < bowNum; i++)
            {
                switch (i)
                {
                    case 0:
                        {
                            button0.text = $"<size=64><b><u>{bow[i].Info}</u></b></size>\n\n{bow[i].handleName}";
                            button0.transform.parent.gameObject.SetActive(true);
                            break;
                        }
                    case 1:
                        {
                            button1.text = $"<size=64><b><u>{bow[i].Info}</u></b></size>\n\n{bow[i].handleName}";
                            button1.transform.parent.gameObject.SetActive(true);
                            break;
                        }
                    case 2:
                        {
                            button2.text = $"<size=64><b><u>{bow[i].Info}</u></b></size>\n\n{bow[i].handleName}";
                            button2.transform.parent.gameObject.SetActive(true);
                            break;
                        }
                    case 3:
                        {
                            button3.text = $"<size=64><b><u>{bow[i].Info}</u></b></size>\n\n{bow[i].handleName}";
                            button3.transform.parent.gameObject.SetActive(true);
                            break;
                        }
                    case 4:
                        {
                            button4.text = $"<size=64><b><u>{bow[i].Info}</u></b></size>\n\n{bow[i].handleName}";
                            button4.transform.parent.gameObject.SetActive(true);
                            break;
                        }
                    case 5:
                        {
                            button5.text = $"<size=64><b><u>{bow[i].Info}</u></b></size>\n\n{bow[i].handleName}";
                            button5.transform.parent.gameObject.SetActive(true);
                            break;
                        }
                }
            }

            // show add btn
            switch (bowNum)
            {
                case 0: { 
                    button0.transform.parent.gameObject.SetActive(true);
                    button0.text = "+";
                    break;
                }
                case 1: { 
                    button1.transform.parent.gameObject.SetActive(true); 
                    button1.text = "+";
                break; 
                }
                case 2: { 
                    button2.transform.parent.gameObject.SetActive(true); 
                    button2.text = "+";
                break; 
                }
                case 3: { 
                    button3.transform.parent.gameObject.SetActive(true); 
                    button3.text = "+";
                break; 
                }
                case 4: { 
                    button4.transform.parent.gameObject.SetActive(true); 
                    button4.text = "+";
                break; 
                }
                case 5: { 
                    button5.transform.parent.gameObject.SetActive(true); 
                    button5.text = "+";
                break; 
                }
                default: { break; }
            }
            Debug.Log("Data loaded from file successfully. " + Application.persistentDataPath + "/" + filename);
        }
        else
        {
            bow.Clear();
            bow = new();
        }
    }
}
