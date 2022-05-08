using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using System.Text.RegularExpressions;


public class RuntimeGUI : MonoBehaviour
{

    private Button menu;
    private Button reset;
    private Button submit;
    private TextField userInput;
    private TextField daveOutput;
    private TextField gptParseOut;

    public RadioButtonGroup actionListGroup;

    string currentAction;
    List<string> actionlist = new List<String>();


    void OnEnable()
    {
        //Root visual element of the UI Document
        var rootVE = GetComponent<UIDocument>().rootVisualElement;

        //Queries visual elements on UI doc and attaches them to variables in script

        //Button Elements
        menu = rootVE.Q<Button>("menu");
        reset = rootVE.Q<Button>("reset");
        submit = rootVE.Q<Button>("submit");
        
        //TextField Elements
        userInput = rootVE.Q<TextField>("user-input");
        daveOutput = rootVE.Q<TextField>("dave-output");
        gptParseOut = rootVE.Q<TextField>("gpt-parsed-words");

        //RadioButtonGroup Element
        actionListGroup = rootVE.Q<RadioButtonGroup>("action-list");

        
        //Loads all radio buttons in teh radiobuttonsgroup t
        foreach (var option in actionListGroup.choices)
        {
            actionlist.Add(option);
        }
        
        Debug.Log(actionlist);
        
        submit.clicked += TestWrite;
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
        //var gptObj = GetComponent<GPTHandler>();
        string commandInput =  userInput.value;
        string reply = GPTHandler.callOpenAI(250, commandInput, "text-davinci-002", 0.7, 1, 0, 0);

        int pFrom = reply.IndexOf("text\":") + "key : ".Length;
        int pTo = reply.IndexOf(", \"index");
        string textReply = reply.Substring(pFrom, pTo - pFrom);

        string formattedTextReply = textReply.Replace("\n","");

        gptParseOut.value = formattedTextReply;
        Debug.Log(reply);
    }
	void OnMainMenuClicked()
	{
		//Debug.Log(actionListGroup.childCount);
	}

    void ReloadScene(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void SetCurrentActionHelper(){
        currentAction = userInput.value;
        SetCurrentBehavior(currentAction);
    }


}
