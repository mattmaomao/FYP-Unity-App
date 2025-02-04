using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[Serializable]
public class InputEntry : MonoBehaviour
{
    public string handleBrand;
    public string handleName;
    public string limbBrand;
    public string limbName;
    public string stringSize;
    public string stringStrand;
    public string stringMaterial;
    public string servingSize;
    public string servingBrand;
    public string servingMaterial;
    public string plungerBrand;
    public string plungerName;
    public string sightBrand;
    public string sightName;

    public InputEntry (string handleBrand, string handleName, string limbBrand, string limbName, string stringSize, string stringStrand, string stringMaterial, string servingSize, string servingBrand, string servingMaterial, string plungerBrand, string plungerName, string sightBrand, string sightName)
    {
        this.handleBrand = handleBrand;
        this.handleName = handleName;
        this.limbBrand = limbBrand;
        this.limbName = limbName;
        this.stringSize = stringSize;
        this.stringStrand = stringStrand;
        this.stringMaterial = stringMaterial;
        this.servingSize = servingSize;
        this.servingBrand = servingBrand;
        this.servingMaterial = servingMaterial;
        this.plungerBrand = plungerBrand;
        this.plungerName = plungerName;
        this.sightBrand = sightBrand;
        this.sightName = sightName;

    }
}
