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
}
