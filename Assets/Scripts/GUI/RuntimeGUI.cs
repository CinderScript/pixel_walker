using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using System.Text.RegularExpressions;


public class RuntimeGUI : MonoBehaviour
{

    // Main UI Elements (Always Visible)
    private VisualElement daveOutGroup;
    private VisualElement daveInGroup;
    private Button menuBtn;
    private Button resetBtn;
    private Button submit;
    private TextField userInput;
    private TextField daveOutput;
    private TextField gptParseOutput;
    private TextField apiInput;
    public RadioButtonGroup actionRadioGroup;


    //Main Menu screen elements (Visible in Main Menu)
    private VisualElement menuWindow;
    private Button levelSelectBtn;
    private Button menuResetBtn;
    private Button apiKeyBtn;

    //Level Select screen elements
    private VisualElement levelSelectWindow;
    private Button levelOneBtn;
    private Button levelTwoBtn;
    private Button levelthreeBtn;

    //API Key screen elements
    private VisualElement apiWindow;
    private Button apiSubmitBtn;

   
    //Elements for error pop up screen
    private VisualElement infoWindow;
    private Label errorLabel;


    //Variables to store input and current selected action
    string currentAction;
    string commandInput;

    //List used to fill actions radio button group
    List<string> actionlist = new List<String>();

    void OnEnable()
    {
        //Root visual element of the UI Document
        var rootVE = GetComponent<UIDocument>().rootVisualElement;


        //Initialize Main UI elements
        menuBtn = rootVE.Q<Button>("menu");
        resetBtn = rootVE.Q<Button>("reset");
        submit = rootVE.Q<Button>("submit");
        userInput = rootVE.Q<TextField>("user-input");
        daveOutput = rootVE.Q<TextField>("dave-text-out");
        gptParseOutput = rootVE.Q<TextField>("gpt-parsed-words");
        
        //Initialize Menu window elements
        menuWindow = rootVE.Q<VisualElement>("menu-pane");
        levelSelectBtn = rootVE.Q<Button>("scene");
        menuResetBtn = rootVE.Q<Button>("reset-in-menu");
        apiKeyBtn = rootVE.Q<Button>("insert-key");

        //Initialize Level Select window elements
        levelSelectWindow = rootVE.Q<VisualElement>("level-select");
        levelOneBtn = rootVE.Q<Button>("level-1");
        levelTwoBtn = rootVE.Q<Button>("level-2");
        levelthreeBtn = rootVE.Q<Button>("level-3");

        //Initialize API key window elements
        apiWindow = rootVE.Q<VisualElement>("api-menu");
        apiSubmitBtn = rootVE.Q<Button>("api-submit");
        apiInput = rootVE.Q<TextField>("api-input-field");
        daveOutGroup = rootVE.Q<VisualElement>("dave-in");
        daveInGroup = rootVE.Q<VisualElement>("dave-out");

        //Initialize error window elements
        infoWindow = rootVE.Q<VisualElement>("info-window");
        errorLabel = rootVE.Q<Label>("error-label");


        //Initializes and fills the Action list radio groups witht the appropriate action from uxml
        actionRadioGroup = rootVE.Q<RadioButtonGroup>("action-list");
        foreach (var option in actionRadioGroup.choices)
        {
            actionlist.Add(option);
        }
        
        Debug.Log(actionRadioGroup.choices.ToString()); // prints action list to console
        
        //Fuctionality of all buttons added here
        submit.clicked += ParseGPT3Reply;
        menuBtn.clicked += OnMainMenuClicked;       
        resetBtn.clicked += ReloadScene;
        menuResetBtn.clicked += ReloadScene;
        levelSelectBtn.clicked += OpenSceneMenu;
        apiKeyBtn.clicked += OpenApiInputMenu;
        apiSubmitBtn.clicked += () => StartCoroutine(SetApiKey());

        levelOneBtn.clicked += () => SceneManager.LoadScene(0);
        levelthreeBtn.clicked += () => SceneManager.LoadScene(1);
        levelTwoBtn.clicked += () => SceneManager.LoadScene(2);

    }

    public void SetCurrentBehavior(string behavior)
	{
       if(!actionlist.Contains(behavior)){
           Debug.Log("Error: Action not in list"); 
       }
       else{ 
           actionRadioGroup.value = actionlist.IndexOf(behavior);
       }
	}

    //Calls upon GPTHandler.cs to send and parse user input, sends error if API key is missing.
    void ParseGPT3Reply(){
        //Prints error if there is no API Key present
        if(GPTHandler.keyString == ""){
            errorLabel.text = "ERROR: NO KEY API PROVIDED. \n Submit one to Menu > API key.";
            infoWindow.style.display = DisplayStyle.Flex;
            Debug.Log("ERROR: NO KEY API PROVIDED");
        }
        else{
            Tuple<string,string> parsedReply = PromptClassifier.ClassifyString(userInput.value);
            Debug.Log(parsedReply);
            gptParseOutput.value = parsedReply.Item1;
            daveOutput.value = parsedReply.Item2;
            string convertedToStr = parsedReply.Item2;
            if (gptParseOutput.value == "Command"){
                int Pos1 = convertedToStr.IndexOf("behavior: ") + "behavior: ".Length;
                int Pos2 = convertedToStr.IndexOf(",");
                string finalStr = convertedToStr.Substring(Pos1, Pos2-Pos1);
                Debug.Log(finalStr);
                SetCurrentBehavior(finalStr.Trim());
            }
            else if (gptParseOutput.value == "ERROR"){
                errorLabel.text ="ERROR: " + parsedReply.Item2;
                infoWindow.style.display = DisplayStyle.Flex;
            }
        }  
    }
    
    //Determines what happens when menu ui button is clicked
    //Turns display  attribute for the 'menu-pane' ve in 
    //RuntimeUI.uxml on and off (Flex or None).
	void OnMainMenuClicked()
	{
        infoWindow.style.display = DisplayStyle.None;
		if(menuWindow.style.display != DisplayStyle.Flex){
            menuWindow.style.display = DisplayStyle.Flex;
            apiWindow.style.display = DisplayStyle.None;
            levelSelectWindow.style.display = DisplayStyle.None;
            daveInGroup.style.display = DisplayStyle.None;
            daveOutGroup.style.display = DisplayStyle.None;
            actionRadioGroup.style.display = DisplayStyle.None;
        } else {
            menuWindow.style.display = DisplayStyle.None;
            daveInGroup.style.display = DisplayStyle.Flex;
            daveOutGroup.style.display = DisplayStyle.Flex;
            actionRadioGroup.style.display = DisplayStyle.Flex;
        }
	}

    //Calls Unity.SceneManager to get current scene and loads it on execution
    void ReloadScene(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void SetCurrentActionHelper(){
        currentAction = userInput.value;
        SetCurrentBehavior(currentAction);
    }

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

    IEnumerator SetApiKey(){
        GPTHandler.keyString = apiInput.value;
        apiWindow.style.display = DisplayStyle.None;
        errorLabel.text = "Key has been set.";
        infoWindow.style.display = DisplayStyle.Flex;
        yield return new WaitForSeconds(1);
        infoWindow.style.display = DisplayStyle.None;
        daveInGroup.style.display = DisplayStyle.Flex;
        daveOutGroup.style.display = DisplayStyle.Flex;
        actionRadioGroup.style.display = DisplayStyle.Flex;

    }

}
