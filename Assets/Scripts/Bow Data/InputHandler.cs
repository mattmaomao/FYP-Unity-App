using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class InputHandler : MonoBehaviour
{
    TMP_Dropdown handleBrand; 
    InputField handleName;
    TMP_Dropdown limbBrand;
    InputField limbName;
    TMP_Dropdown stringSize;
    TMP_Dropdown stringStrand;
    InputField stringMaterial;
    TMP_Dropdown servingSize;
    TMP_Dropdown servingBrand;
    InputField servingMaterial;
    TMP_Dropdown plungerBrand;
    InputField plungerName;
    TMP_Dropdown sightBrand;
    InputField sightName;

    List<InputEntry> bow = new List<InputEntry> ();
    [SerializeField] string filename;
    public void AddNameToList()
    {
        bow.Add(new InputEntry(handleBrand.options[handleBrand.value].text, handleName.text, limbBrand.options[limbBrand.value].text, limbName.text, stringSize.options[stringSize.value].text, stringStrand.options[stringStrand.value].text, stringMaterial.text, servingSize.options[servingSize.value].text, servingBrand.options[servingBrand.value].text, servingMaterial.text, plungerBrand.options[plungerBrand.value].text, plungerName.text, sightBrand.options[sightBrand.value].text, sightName.text));
        FileHandler.SaveToJSON<InputEntry>(bow, filename);
    }
}
