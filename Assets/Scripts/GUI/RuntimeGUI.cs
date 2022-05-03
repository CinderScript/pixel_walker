using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;


public class RuntimeGUI : MonoBehaviour
{

    private Button menu;
    private Button reset;

    private Button submit;

    private TextField userInput;

    private TextField daveOutput;

    private TextField gptParseOut;

    public RadioButtonGroup actionListGroup;


    string action;
    public RadioButton currentAction;
    List<string> actionlist = new List<String>();



    void OnEnable()
    {
        var rootVE = GetComponent<UIDocument>().rootVisualElement;

        menu = rootVE.Q<Button>("menu");
        reset = rootVE.Q<Button>("reset");
        submit = rootVE.Q<Button>("submit");

        userInput = rootVE.Q<TextField>("user-input");
        daveOutput = rootVE.Q<TextField>("dave-output");
        gptParseOut = rootVE.Q<TextField>("gpt-parsed-words");

        actionListGroup = rootVE.Q<RadioButtonGroup>("action-list");

        foreach (var option in actionListGroup.choices)
        {
            actionlist.Add(option);
        }
        
        Debug.Log(actionlist);
        


        submit.clicked += TestRadioGroup;
        menu.clicked += OnMainMenuClicked;
        reset.clicked += ReloadScene;



    }

    public void SetCurrentBehavior(string behavior)
	{
       if(!actionlist.Contains(behavior)){
           Debug.Log("Error: Action not in list"); 
       }
       else{ 
           actionListGroup.value = actionlist.IndexOf(behavior);
       }
	}

    void TestWrite(){
        string command =  userInput.value;
        userInput.value = "";
        gptParseOut.value = command;
        Debug.Log("Button Has Been Pressed");
    }
	void OnMainMenuClicked()
	{
		Debug.Log(actionListGroup.childCount);
	}

    void ReloadScene(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void TestRadioGroup(){
        action = userInput.value;
        SetCurrentBehavior(action);
    }


}
