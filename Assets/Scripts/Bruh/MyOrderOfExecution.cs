using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using test;
using TMPro;

public class MyOrderOfExecution : MonoBehaviour {

    [SerializeField] GameObject ScreenControlsObj;

    MyScreenControls screenControls;
    MyUbyteParser ubyteParser;
    MyNeuralNetwork neuralNetwork;
    MyUINNResults UINNResults;

    [SerializeField] TextMeshProUGUI debugText;


    private void Awake()
    {
        screenControls = ScreenControlsObj.GetComponent<MyScreenControls>();
        ubyteParser = gameObject.GetComponent<MyUbyteParser>();
        neuralNetwork = gameObject.GetComponent<MyNeuralNetwork>();
        UINNResults = gameObject.GetComponent<MyUINNResults>();

        StartCoroutine(StartUp());
    }

    IEnumerator StartUp()
    {
        debugText.text = "Starting up...\n";
        debugText.text += "init nn\n";
        neuralNetwork.InitializeNNVariables();
        yield return null;
        debugText.text += "nn load num\n";
        neuralNetwork.LoadNumbersFromfile();
        yield return null;
        debugText.text += "screen controls\n";
        screenControls.CustomStart();
        yield return null;
        debugText.text += "ubyte parser\n";
        ubyteParser.CustomStart();
        yield return null;
        debugText.text += "nn result\n";
        UINNResults.CustomStart();
    }
}
