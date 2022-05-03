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

    public RadioButton stand;
	public RadioButton goTo;
	public RadioButton open;
	public RadioButton pickUp;
	public RadioButton fetch;



    void OnEnable()
    {
        var rootVE = GetComponent<UIDocument>().rootVisualElement;

        menu = rootVE.Q<Button>("menu");
        reset = rootVE.Q<Button>("reset");
        submit = rootVE.Q<Button>("submit");

        userInput = rootVE.Q<TextField>("user-input");
        daveOutput = rootVE.Q<TextField>("dave-output");
        gptParseOut = rootVE.Q<TextField>("gpt-parsed-words");

        stand = rootVE.Q<RadioButton>("stand");
        goTo = rootVE.Q<RadioButton>("go-to");
        open = rootVE.Q<RadioButton>("open");
        pickUp = rootVE.Q<RadioButton>("pick-up");
        fetch = rootVE.Q<RadioButton>("fetch");

        submit.clicked += TestWrite;
        menu.clicked += OnMainMenuClicked;
        reset.clicked += ReloadScene;


    }

    public void SetCurrentBehavior(string behavior)
	{
		switch (behavior)
		{
			case "Stand":
				stand.value = true;
				break;
			case "GoTo":
				goTo.value = true;
				break;
			case "Open":
				open.value = true;
				break;
			case "PickUp":
				pickUp.value = true;
				break;
			case "Fetch":
				fetch.value = true;
				break;
			default:
				break;
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
		throw new NotImplementedException();
	}

    void ReloadScene(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


}
