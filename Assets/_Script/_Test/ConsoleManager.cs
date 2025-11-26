using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ConsoleManager : MonoBehaviour
{
    public TMP_InputField consoleText;
    public static Dictionary<string, Func<string,  bool>> nameCommandDict = new Dictionary<string, Func<string, bool>>
        (

        ); 
    public void CommandInput()
    {
        var text = consoleText.text;
        consoleText.text = "";
        var res  = CommandProceess(text);
        Debug.Log($"command result : {res}");
    }
    public bool CommandProceess(string text)
    {

        var command = text.Split()[0];
        if(nameCommandDict.ContainsKey(command))
        {
            nameCommandDict[command](text);
            return true;
        }
        else return false;
    }

}
