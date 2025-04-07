using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MyUINNResults : MonoBehaviour
{

    [SerializeField] GameObject recognizedValueUI;

    TextMeshProUGUI recognizedValue;
    MyNeuralNetwork neuralNetwork;

    bool started;

    public void CustomStart()
    {
        started = true;
        neuralNetwork = gameObject.GetComponent<MyNeuralNetwork>();
        recognizedValue = recognizedValueUI.GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (!started)
            return;

        recognizedValue.text = neuralNetwork.GetRecognizedValue().ToString();
    }
}
