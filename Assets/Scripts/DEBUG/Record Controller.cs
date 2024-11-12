using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class RecordController
{
    public static string filePath = "record.txt";

    public static void AppendTextToFile(string text)
    {
        using StreamWriter writer = File.AppendText(filePath);
        writer.WriteLine(text);
    }
}