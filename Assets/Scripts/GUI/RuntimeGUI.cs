using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using System.Text.RegularExpressions;
//susing InputManager;


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
    string keyString = "";

    Exception ex;

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
        menuBtn.clicked += ToggleMenu;
        resetBtn.clicked += ReloadScene;
        menuResetBtn.clicked += ReloadScene;
        levelSelectBtn.clicked += OpenSceneMenu;
        apiKeyBtn.clicked += OpenApiInputMenu;
        apiSubmitBtn.clicked += SetApiKey;

        levelOneBtn.clicked += () => SceneManager.LoadScene(0);
        levelthreeBtn.clicked += () => SceneManager.LoadScene(1);
        levelTwoBtn.clicked += () => SceneManager.LoadScene(2);

    }

    public void SetCurrentBehavior(string behavior)
    {
        if (!actionlist.Contains(behavior))
        {
            Debug.Log("Error: Action not in list");
        }
        else
        {
            actionRadioGroup.value = actionlist.IndexOf(behavior);
        }
    }

    //Calls upon The InputHandlerFramework's UserInputHandler.cs  to send and parse user input, sends error if API key is missing.
    void ParseGPT3Reply()
    {
        //Prints error if there is no API Key present
        if (keyString == "")
        {
            ex = new Exception("No Key Provided: Menu > API Key to add.");
            StartCoroutine(CreatePopUp(ex.Message, 3));
            Debug.Log("ERROR: NO KEY API PROVIDED");
        }
        else
        {
            UserInputHandler handler = new UserInputHandler(keyString, "TODO: *fileprompts.txt*");
            GptResponse responce = handler.GetGptResponce(userInput.value, out ex);
            Debug.Log(responce.GeneratedText);
            var responceProperties = responce.BehaviorProperties;


            if (responce.Type == InputType.Command)
            {
                gptParseOutput.value = "\nBehavior:\n\t " + responceProperties.Behavior + "\nObject:\n\t " + responceProperties.Object
                            + "\nLocation:\n\t " + responceProperties.Location;
            }
            else if (responce.Type == InputType.Question)
            {
                daveOutput.value = responce.GeneratedText.ToString();
            }
            else
            {
                StartCoroutine(CreatePopUp(ex.Message, 3));
            }
        }
    }

    //Toggles menu on or off(Flex or None) 
    void ToggleMenu()
    {
        if (menuWindow.style.display != DisplayStyle.Flex)
        {
            CloseMenuElements();
            menuWindow.style.display = DisplayStyle.Flex;
            ToggleMainUI(false);
        }
        else
        {
            CloseMenuElements();
            ToggleMainUI(true);
        }
    }

    //Calls Unity.SceneManager to get current scene and loads it when called
    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    //Opens Level Select, does not need a None as ui is disabled on new level load
    //or is closed when the menu button is pressed
    void OpenSceneMenu()
    {
        CloseMenuElements();
        levelSelectWindow.style.display = DisplayStyle.Flex;

    }

    //Opens Input Text Field for API key, 
    //toggles off when other menu element is selected
    //or when api-submit button is pressed
    void OpenApiInputMenu()
    {
        CloseMenuElements();
        apiWindow.style.display = DisplayStyle.Flex;
    }


    //Sets keystring variable to entered the API key
    //Pops up window on excution to say key has been entered
    //Will pop up cautionary message if left empty
    void SetApiKey()
    {
        string tempText;
        keyString = apiInput.value;
        if(apiInput.value == ""){
            ex = new Exception("Field cannot Be left blank.");
            tempText = ex.Message;
        }
        else{
            tempText = "Key has been entered";
            ToggleMainUI(true);
        }
        StartCoroutine(CreatePopUp(tempText, 2));
        //ToggleMainUI(true);

    }

    //Creates a pop up window to display system dialogue
    //in: string 'message' to be displayed as message on pop up
    //in: integer 'secondsVisible' is the number of 
    //    seconds pop up will be displayed
    //out: IEnumerator coroutine object which delays by secondsVisible
    IEnumerator CreatePopUp(string message, int secondsVisible)
    {
        errorLabel.text = message;
        infoWindow.style.display = DisplayStyle.Flex;
        yield return new WaitForSeconds(secondsVisible);
        infoWindow.style.display = DisplayStyle.None;
    }

    //Toggles the main UI visibility on or off
    //in: bool (true for visible and false for invisble);
    void ToggleMainUI(bool toggle)
    {
        if (toggle)
        {
            CloseMenuElements();
            daveInGroup.style.display = DisplayStyle.Flex;
            daveOutGroup.style.display = DisplayStyle.Flex;
            actionRadioGroup.style.display = DisplayStyle.Flex;
        }
        else
        {
            daveInGroup.style.display = DisplayStyle.None;
            daveOutGroup.style.display = DisplayStyle.None;
            actionRadioGroup.style.display = DisplayStyle.None;
        }
    }

    //Closes all non Main UI Elements such as menu elements
    //Popup windows not handled here
    void CloseMenuElements()
    {
        apiWindow.style.display = DisplayStyle.None;
        menuWindow.style.display = DisplayStyle.None;
        levelSelectWindow.style.display = DisplayStyle.None;
    }

}
