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
    public TextMeshProUGUI limbName;
    public TMP_Dropdown stringSize;
    public TMP_Dropdown stringStrand;
    public TextMeshProUGUI stringMaterial;
    public TMP_Dropdown servingSize;
    public TMP_Dropdown servingBrand;
    public TextMeshProUGUI servingMaterial;
    public TMP_Dropdown plungerBrand;
    public TextMeshProUGUI plungerName;
    public TMP_Dropdown sightBrand;
    public TextMeshProUGUI sightName;
    bool RH;

    List<InputEntry> bow = new List<InputEntry> ();
    [SerializeField] string filename;

    public void AddNameToList()
    {
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

    public void LoadNameFromList()
    {
        if (File.Exists(Application.persistentDataPath + "/" + filename))
        {
            string content = File.ReadAllText(Application.persistentDataPath + "/" + filename);
            bow.Clear();
            bow = new();
            bow = JsonConvert.DeserializeObject<List<InputEntry>>(content);

            InputEntry entry = bow[0];
            index = entry.index;
            
            if (entry.RH == true)
            {
                LRH.value = 1;
            } else LRH.value = 0;
            handleBrand.value = handleBrand.options.FindIndex(option => option.text == entry.handleBrand);
            handleNameInput.text = entry.handleName;
            handleName.text = "HI";
            Debug.Log(entry.handleName);
            limbBrand.value = limbBrand.options.FindIndex(option => option.text == entry.limbBrand);
            limbName.text = entry.limbName;
            stringSize.value = stringSize.options.FindIndex(option => option.text == entry.stringSize);
            stringStrand.value = stringStrand.options.FindIndex(option => option.text == entry.stringStrand);
            stringMaterial.text = entry.stringMaterial;
            servingSize.value = servingSize.options.FindIndex(option => option.text == entry.servingSize);
            servingBrand.value = servingBrand.options.FindIndex(option => option.text == entry.servingBrand);
            servingMaterial.text = entry.servingMaterial;
            plungerBrand.value = plungerBrand.options.FindIndex(option => option.text == entry.plungerBrand);
            plungerName.text = entry.plungerName;
            sightBrand.value = sightBrand.options.FindIndex(option => option.text == entry.sightBrand);
            sightName.text = entry.sightName;

            Debug.Log("Data loaded from file successfully. " + Application.persistentDataPath + "/" + filename);
        }
        else
        {
            bow.Clear();
            bow = new();
        }

    }
}
