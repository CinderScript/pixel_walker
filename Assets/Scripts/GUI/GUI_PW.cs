using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GUI_PW : MonoBehaviour
{

    private Button reloadScene;
    private TextField inputStr;

    private TextField outputStr;
    void OnEnable(){
        var rootVE = GetComponent<UIDocument>().rootVisualElement;

        inputStr = rootVE.Q<TextField>("input-box");
        outputStr = rootVE.Q<TextField>("gpt-string-out");
        reloadScene = rootVE.Q<Button>("reload-scene");
        reloadScene.text = "Command";

        reloadScene.RegisterCallback<ClickEvent>(ev => TestWrite());


    }

    void TestWrite(){
        string command =  inputStr.value;
        inputStr.value = "";
        outputStr.value = command;
        Debug.Log("Button Has Been Pressed");

    }

}
