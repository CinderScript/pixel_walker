using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Security.AccessControl;
using System.Threading.Tasks;

public class RuntimeGUI : MonoBehaviour
{

    // Main UI Elements (Visible at Runtime)
    private VisualElement daveOutGroup;
    private VisualElement daveInGroup;
    private Button menuBtn;
    private Button resetBtn;
    private Button submitBtn;
    private Button cancelBtn;
    private TextField userInput;
    private TextField daveOutput;
    private TextField gptParseOutput;
    private TextField apiInput;
    private RadioButtonGroup actionRadioGroup;


    //Main Menu screen elements (Invisible at Runtime)
    private VisualElement menuWindow;
    private Button levelSelectBtn;
    private Button menuResetBtn;
    private Button apiKeyBtn;
    private Button engineMenuBtn;

    //Level Select screen elements (Invisible at Runtime)
    private VisualElement levelSelectWindow;
    private Button levelOneBtn;
    private Button levelTwoBtn;
    private Button levelthreeBtn;

    //API Key screen elements (Invisible at Runtime)
    private VisualElement apiWindow;
    private Button apiSubmitBtn;


    //Elements for GPT-3 Engine Selection Window (Invisible at Runtime)
    private VisualElement engineWindow;
    private RadioButtonGroup engineRadioGroup;
    private Button engineConfirmBtn;



    //Elements for error pop up screen (Invisible at Runtime)
    private VisualElement infoWindow;
    private Label errorLabel;


    //Variables to store input and current selected action
    private UserInputHandler handler;
    private EngineType engine;
    private const string PROMPT_FILE = "prompt.JSON"; 




    //CONSTANTS
    private const int DELAY = 1;
    private const string FILEPATH = "key.txt";

    //Variables for saving
    private string debugMessage = "";
    private string keyPath;


    //List used to fill actions radio button group
    List<string> actionlist = new List<String>();




    async void OnEnable()
    {

        keyPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/" + FILEPATH;

        if (File.Exists(keyPath))
        {
            //File.SetAttributes(keyPath, FileAttributes.Hidden);
            string key = await LoadApiKey(keyPath);
            handler = new UserInputHandler(key, PROMPT_FILE, engine);
        }


        //Root visual element of the UI Document
        var rootVE = GetComponent<UIDocument>().rootVisualElement;


        //Initialize Main UI elements
        menuBtn = rootVE.Q<Button>("menu");
        resetBtn = rootVE.Q<Button>("reset");
        submitBtn = rootVE.Q<Button>("submit");
        cancelBtn = rootVE.Q<Button>("cancel");
        userInput = rootVE.Q<TextField>("user-input");
        daveOutput = rootVE.Q<TextField>("dave-text-out");
        gptParseOutput = rootVE.Q<TextField>("gpt-parsed-words");

        //Initialize Menu window elements
        menuWindow = rootVE.Q<VisualElement>("menu-pane");
        levelSelectBtn = rootVE.Q<Button>("scene");
        menuResetBtn = rootVE.Q<Button>("reset-in-menu");
        apiKeyBtn = rootVE.Q<Button>("insert-key");
        engineMenuBtn = rootVE.Q<Button>("engine-select-menu");

        //Initialize Level Select window elements
        levelSelectWindow = rootVE.Q<VisualElement>("level-select");
        levelOneBtn = rootVE.Q<Button>("level-1");
        levelTwoBtn = rootVE.Q<Button>("level-2");
        levelthreeBtn = rootVE.Q<Button>("level-3");

        //Initialize API key window elements
        apiWindow = rootVE.Q<VisualElement>("api-menu");
        apiSubmitBtn = rootVE.Q<Button>("api-submit");
        apiInput = rootVE.Q<TextField>("api-input-field");
        apiInput.isPasswordField = true;
        apiInput.maskChar = '*';


        //Main Input and Output UI Elements 
        daveOutGroup = rootVE.Q<VisualElement>("dave-in");
        daveInGroup = rootVE.Q<VisualElement>("dave-out");

        //Initialize error window elements
        infoWindow = rootVE.Q<VisualElement>("info-window");
        errorLabel = rootVE.Q<Label>("error-label");

        engineWindow = rootVE.Q<VisualElement>("engine-select");
        engineConfirmBtn = rootVE.Q<Button>("confirm-engine");
        engineRadioGroup = rootVE.Q<RadioButtonGroup>("engine-radio-group");

        //Initializes and fills the Action list radio groups witht the appropriate action from uxml
        actionRadioGroup = rootVE.Q<RadioButtonGroup>("action-list");
        foreach (var option in actionRadioGroup.choices)
        {
            actionlist.Add(option);
        }

        Debug.Log(actionRadioGroup.choices.ToString()); // prints action list to console

        //Fuctionality of all buttons added here
        submitBtn.clicked += SendToGPT;
        menuBtn.clicked += OnMainMenuClicked;
        resetBtn.clicked += ReloadScene;
        menuResetBtn.clicked += ReloadScene;
        levelSelectBtn.clicked += OpenSceneMenu;
        apiKeyBtn.clicked += OpenApiInputMenu;
        apiSubmitBtn.clicked += SetApiKey;
        engineMenuBtn.clicked += OpenEngineMenu;
        engineConfirmBtn.clicked += SelectEngine;

        //Level select buttons -- refer to scene build order
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

    /// <summary>
    /// Async method to send user input to GPT-3
    /// Prints command parses to gpt parse window,
    /// and prints question replys to dave's output window. if no API key 
    /// is supplied  the it creates a pop up (refer to CreatePopUp())
    /// to display error, else calls on the User Input Handler to classify 
    /// </summary>
    async void SendToGPT()
    {
        string replyForGptWindow = "...";
        string replyForDaveWindow = "...";
        string key = "";
        if (!File.Exists(keyPath))
        {
            debugMessage = "No API key on record";
            StartCoroutine(CreatePopUp(debugMessage, DELAY));
            Debug.Log("ERROR: NO KEY API PROVIDED");
        }
        else if (File.Exists(keyPath))
        {
            key = await LoadApiKey(keyPath);
            //string testHandle = await TestGptResponse(key);
            GptResponse responce;
            try
            {
                responce = await handler.GetGptResponce(userInput.value);
            }

            catch (Exception){throw;}

            var responceProperties = responce.BehaviorProperties;
            if (responce.Type == InputType.Command)
            {
                replyForGptWindow = "\nBehavior:\n\t " + responceProperties.Behavior
                                        + "\nObject:\n\t " + responceProperties.Object
                                        + "\nLocation:\n\t " + responceProperties.Location;

                replyForDaveWindow = responce.GeneratedText.ToString();
            }
            else if (responce.Type == InputType.Question)
            {
                replyForDaveWindow = responce.GeneratedText.ToString();
            }
            else if (responce.Type == InputType.Conversation)
            {
                replyForDaveWindow = responce.GeneratedText.ToString();
            }
            else
            {
                replyForGptWindow = responce.GeneratedText.ToString();
            }
            gptParseOutput.value = replyForGptWindow;
            daveOutput.value = replyForDaveWindow;
        }
    }

    /// <summary>
    /// Determines events when Menu button is clicked.
    /// Toggles between MainUI elements and Main Menu elements
    /// </summary>
    void OnMainMenuClicked()
    {
        infoWindow.style.display = DisplayStyle.None;
        if (menuWindow.style.display == DisplayStyle.Flex)
        {
            ToggleMainUI(true);
        }
        else
        {
            ToggleMainUI(false);
            menuWindow.style.display = DisplayStyle.Flex;

        }
    }

    /// <summary>
    /// Calls Unity.SceneManager to get current scene and loads it on click.
    /// </summary>
    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Toggles menu window for selecting between scenes/levels
    /// </summary>
    void OpenSceneMenu()
    {
        if (levelSelectWindow.style.display == DisplayStyle.Flex)
        {
            ToggleMainUI(true);
        }
        else
        {
            ToggleMainUI(false);
            levelSelectWindow.style.display = DisplayStyle.Flex;
        }
    }

    /// <summary>
    ///Toggles Menu window for changing GPT-3 engine
    ///closes all other menu elements on click.
    /// </summary>
    void OpenEngineMenu()
    {
        ToggleMainUI(false);
        engineWindow.style.display = DisplayStyle.Flex;
    }

    /// <summary>
    ///Toggles Menu window Inputing API key.
    ///closes all other menu elements on click.
    /// </summary>
    void OpenApiInputMenu()
    {
        if (apiWindow.style.display == DisplayStyle.Flex)
        {
            ToggleMainUI(true);
        }
        else
        {
            ToggleMainUI(false);
            apiWindow.style.display = DisplayStyle.Flex;
        }
    }

    /// <summary>
    /// Async method for setting API key.
    /// Has a Gpt3Connection object to test authenticity/
    /// validity of api key. Sends a small test string to GPT-3
    /// using the ADA text engine. If there is a reply then a pop
    /// up is show displaying a successful validation. Else the pop up 
    /// displays an error message.
    /// </summary>
    async void SetApiKey()
    {
        if (apiInput.value == "")
        {
            debugMessage = "Field cannot be left blank";
            StartCoroutine(CreatePopUp(debugMessage, DELAY));
        }
        else
        {
            string key = apiInput.value;
            string test = await TestGptResponse(key);
            Debug.Log(test);
            if(test != null){
                SaveApiKey(key, keyPath);
            }else{
                throw new Exception("Key not valid");
            }
        }

    }

    async Task<string> TestGptResponse(string key)
    {
        string testResponse;
        engine = EngineType.Ada;
        Gpt3Connection testConnection = new Gpt3Connection(key, engine); ;
        try
        {
            testResponse = await testConnection.GenerateText("Say one word {stop}");
            debugMessage = "Validation Successful!";
            engine = EngineType.Davinci;
            handler = new UserInputHandler(key, PROMPT_FILE, engine);
        }
        catch (Exception)
        {
            debugMessage = "Key not Valid. Enter another one and try again.";
            StartCoroutine(CreatePopUp(debugMessage, DELAY));
            throw;
        }
        ToggleMainUI(true);
        StartCoroutine(CreatePopUp(debugMessage, DELAY));
        return testResponse;

    }

    /// <summary>
    /// Creates a pop up window that displays a message for a
    /// defined amount of seconds. 
    /// </summary>
    /// <param name="message"> a string variable for the message 
    /// to be displayed on the pop up</param>
    /// <param name="secondsVisible"> The number of seconds the pop up will be displayed</param>
    /// <returns>A Couroutine that delays by secondsVisble</returns>
    IEnumerator CreatePopUp(string message, float secondsVisible)
    {
        errorLabel.text = message;
        infoWindow.style.display = DisplayStyle.Flex;
        yield return new WaitForSeconds(secondsVisible);
        infoWindow.style.display = DisplayStyle.None;
    }

    /// <summary>
    /// Toogles Main UI element visibility.
    /// </summary>
    /// <param name="toggle"> bool variable true for visible and
    /// false for other wise</param>
    void ToggleMainUI(bool toggle)
    {
        CloseMenuElements();
        if (toggle)
        {
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

    /// <summary>
    ///Closes all non Main UI Elements such as menu elements.
    ///Popup windows are not handled here.
    /// </summary>
    void CloseMenuElements()
    {
        apiWindow.style.display = DisplayStyle.None;
        menuWindow.style.display = DisplayStyle.None;
        levelSelectWindow.style.display = DisplayStyle.None;
        engineWindow.style.display = DisplayStyle.None;

    }

    /// <summary>
    /// Sets the GPT-3 Text engine being used.
    /// Selected via radio buttons.
    /// Defaults to Davinci
    /// </summary>
    async void SelectEngine()
    {
        if (engineRadioGroup.value == 1)
        {
            engine = EngineType.Curie;
        }
        else if (engineRadioGroup.value == 2)
        {
            engine = EngineType.Babbage;
        }
        else if (engineRadioGroup.value == 3)
        {
            engine = EngineType.Ada;
        }
        else
        {
            engine = EngineType.Davinci;
        }

        string key = await LoadApiKey(keyPath);
        handler = new UserInputHandler(key, PROMPT_FILE, engine);
        Debug.Log(engineRadioGroup.value.ToString());
        ToggleMainUI(true);
    }
    async void SaveApiKey(string key, string path)
    {
        try
        {
            await File.WriteAllTextAsync(path, key);
        }
        catch (Exception)
        {
            throw;
        }
        //ApiCrypt.AddEncryption(path);
        Debug.Log(path);
    }

    async Task<string> LoadApiKey(string path)
    {
        //ApiCrypt.RemoveEncryption(path);
        try
        {
            
            string key = await File.ReadAllTextAsync(path);
            //ApiCrypt.AddEncryption(path);
            return key;
        }
        catch (Exception) { throw; }
    }



}
