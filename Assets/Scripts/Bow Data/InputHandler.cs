using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;
using System.IO;


public class InputHandler : MonoBehaviour
{
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

    List<InputEntry> bow = new List<InputEntry> ();
    [SerializeField] string filename;

    public void AddNameToList()
    {
        string bowcontent = File.ReadAllText(Application.persistentDataPath + "/" + filename);
        bow.Clear();
        bow = new();
        bow = JsonConvert.DeserializeObject<List<InputEntry>>(bowcontent);
        int bowNum = bow.Count;
        for (int i = 0; i < bowNum; i++)
        {
            if (bow[i].index == index)
            {
                bow.RemoveAt(i);
                Debug.Log("Remove" + i);
                break;
            }
        }

        if (LRH.value == 1)
        {
            RH = true;
        } else RH = false;
        bow.Add(new InputEntry(index, RH, handleBrand.options[handleBrand.value].text, handleName.text, limbBrand.options[limbBrand.value].text, limbName.text, stringSize.options[stringSize.value].text, stringStrand.options[stringStrand.value].text, stringMaterial.text, servingSize.options[servingSize.value].text, servingBrand.options[servingBrand.value].text, servingMaterial.text, plungerBrand.options[plungerBrand.value].text, plungerName.text, sightBrand.options[sightBrand.value].text, sightName.text));
        string content = JsonConvert.SerializeObject(bow, Formatting.Indented);
        File.WriteAllText(Application.persistentDataPath + "/" + filename, content);
        Debug.Log(Application.persistentDataPath + "/" + filename);
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
    }

    public void ResetBowData()
    {
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

            int bowNum = bow.Count;
            Debug.Log(bowNum);
            for (int i = 0; i < bowNum; i++)
            {
                if (bow[i].index == index)
                {
                    LoadBowData(bow[i]);
                    break;
                }
                else
                {
                    ResetBowData();
                }
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
