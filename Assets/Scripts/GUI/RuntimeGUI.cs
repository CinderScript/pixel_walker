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
<<<<<<< HEAD
    private Button resetInMenu;
=======
>>>>>>> master
    private Button submit;
    private TextField userInput;
    private TextField daveOutput;
    private TextField gptParseOut;
<<<<<<< HEAD
    private TextField apiInput;

    private Button sceneSelect;
    private Button apiKey;
    private Button levelA;
    private Button levelB;
    private Button levelC;

    private Button apiSubmit;

    private VisualElement menuWindow;
    private VisualElement levelSelectWindow;
    private VisualElement apiWindow;
    private VisualElement infoWindow;

    private Label errorLabel;

=======
>>>>>>> master

    public RadioButtonGroup actionListGroup;

    string currentAction;
    List<string> actionlist = new List<String>();


<<<<<<< HEAD


=======
>>>>>>> master
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
<<<<<<< HEAD
        
        menuWindow = rootVE.Q<VisualElement>("menu-pane");
        sceneSelect = rootVE.Q<Button>("scene");
        resetInMenu = rootVE.Q<Button>("reset-in-menu");
        apiKey = rootVE.Q<Button>("insert-key");

        levelSelectWindow = rootVE.Q<VisualElement>("level-select");
        levelA = rootVE.Q<Button>("level-1");
        levelB = rootVE.Q<Button>("level-2");
        levelC = rootVE.Q<Button>("level-3");

        apiWindow = rootVE.Q<VisualElement>("api-menu");
        apiSubmit = rootVE.Q<Button>("api-submit");
        apiInput = rootVE.Q<TextField>("api-input-field");

        infoWindow = rootVE.Q<VisualElement>("info-window");
        errorLabel = rootVE.Q<Label>("error-label");

=======
>>>>>>> master

        //RadioButtonGroup Element
        actionListGroup = rootVE.Q<RadioButtonGroup>("action-list");

        
        //Loads all radio buttons in teh radiobuttonsgroup t
        foreach (var option in actionListGroup.choices)
        {
            actionlist.Add(option);
        }
        
        Debug.Log(actionlist);
        
<<<<<<< HEAD
        submit.clicked += ParseGPT3Reply;
        menu.clicked += OnMainMenuClicked;       
        reset.clicked += ReloadScene;
        resetInMenu.clicked += ReloadScene;
        sceneSelect.clicked += OpenSceneMenu;
        apiKey.clicked += OpenApiInputMenu;
        apiSubmit.clicked += SetApiKey;

        levelA.clicked += () => SceneManager.LoadScene(0);
        levelC.clicked += () => SceneManager.LoadScene(1);
        levelB.clicked += () => SceneManager.LoadScene(2);
=======
        submit.clicked += TestWrite;
        menu.clicked += OnMainMenuClicked;
        reset.clicked += ReloadScene;
>>>>>>> master

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

<<<<<<< HEAD
    void ParseGPT3Reply(){
        if(GPTHandler.keyString == ""){
            errorLabel.text = "ERROR: NO KEY API PROVIDED. \n Submit one to Menu > API key.";
            infoWindow.style.display = DisplayStyle.Flex;
            Debug.Log("ERROR: NO KEY API PROVIDED");
        }
        else{
            //var gptObj = GetComponent<GPTHandler>();
            string commandInput =  userInput.value;
            string reply = GPTHandler.callOpenAI(250, commandInput, "text-davinci-002", 0.7, 1, 0, 0);
            Debug.Log(reply);
            gptParseOut.value = reply;
        }
        
    }
	void OnMainMenuClicked()
	{
        infoWindow.style.display = DisplayStyle.None;
		if(menuWindow.style.display != DisplayStyle.Flex){
            menuWindow.style.display = DisplayStyle.Flex;
            apiWindow.style.display = DisplayStyle.None;
            levelSelectWindow.style.display = DisplayStyle.None;
        } else {
            menuWindow.style.display = DisplayStyle.None;
        }
=======
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
>>>>>>> master
	}

    void ReloadScene(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void SetCurrentActionHelper(){
        currentAction = userInput.value;
        SetCurrentBehavior(currentAction);
    }

<<<<<<< HEAD
    void OpenSceneMenu(){
        if(levelSelectWindow.style.display == DisplayStyle.Flex){
            levelSelectWindow.style.display = DisplayStyle.None;
        } else {
            levelSelectWindow.style.display = DisplayStyle.Flex;
            apiWindow.style.display = DisplayStyle.None;
            menuWindow.style.display = DisplayStyle.None;
        }  
    }

    void OpenApiInputMenu(){
        if(apiWindow.style.display != DisplayStyle.Flex){
            apiWindow.style.display = DisplayStyle.Flex;
            menuWindow.style.display = DisplayStyle.None;
            levelSelectWindow.style.display = DisplayStyle.None;
        } else {
            apiWindow.style.display = DisplayStyle.None;
        }  
    }

    void SetApiKey(){
        GPTHandler.keyString = apiInput.value;
        apiWindow.style.display = DisplayStyle.None;
    }
=======
>>>>>>> master

}
