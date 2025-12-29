using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class ConsoleManager : MonoBehaviour
{

    public TMP_InputField consoleText;

    private Dictionary<string, Func<string, bool>> nameCommandDict;

    void Awake()
    {

        nameCommandDict = new Dictionary<string, Func<string, bool>>
        {
            { "Log", TestLog },
            { "Equip", EquipItem },
            { "Quit", (cmd) => { Application.Quit(); return true; } } // 람다식 예시
        };

        consoleText.gameObject.SetActive(false);
    }

    void Start()
    {
        consoleText.onSubmit.AddListener(CommandInput);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            ToggleConsole();
        }
    }

    public void ToggleConsole()
    {
        bool isActive = !consoleText.gameObject.activeInHierarchy;
        consoleText.gameObject.SetActive(isActive);

        if (isActive)
        {
            Time.timeScale = 0;
            consoleText.ActivateInputField();
        }
        else
        {
            Time.timeScale = 1;
            consoleText.text = "";
        }
    }

    public void CommandInput(string text)
    {
        // 엔터 쳤을 때 다시 포커스 유지할지, 끌지 결정 (여기선 유지 후 초기화)
        consoleText.text = "";
        consoleText.ActivateInputField(); // 연속 입력 편의성

        if (string.IsNullOrWhiteSpace(text)) return; // 빈 값 방어

        var res = CommandProcess(text);
        Debug.Log($"Command result : {res}");
    }

    public bool CommandProcess(string text)
    {
        // 공백으로 나눌 때, 빈 공백 제거 옵션 추가
        var parts = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 0) return false;

        string command = parts[0];

        if (nameCommandDict.ContainsKey(command))
        {
            return nameCommandDict[command](text);
        }
        else
        {
            Debug.LogWarning($"Unknown command: {command}");
            return false;
        }
    }


    public bool TestLog(string command)
    {
        var split = command.Split();
        StringBuilder stringBuilder = new StringBuilder();

        for (int i = 1; i < split.Length; i++)
        {
            stringBuilder.Append(split[i]);
            stringBuilder.Append(" ");
        }

        Debug.Log($"[Console Log] {stringBuilder.ToString()}");
        return true;
    }

    public bool EquipItem(string command)
    {
        var split = command.Split();
        if(split.Length < 2 || split.Length >2)
        {
            Debug.LogWarning("This is a command that cannot be performed.");
            return false;
        }
        var itemName = split[1];
        AdvancedEquipmentController.Instance.EquipItem(itemName);
        return true;
    }
}