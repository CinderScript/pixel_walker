/**
* Project: Pixel Walker
*
* Description: RuntimeGUI is a C# class that handles
* all UI input and outputs. It does so by utilising the
* Gpt3Connection Class to send inputs and recieve outputs from
* Open-AI GPT-3.
* 
* Author: Pixel Walker -
* Maynard, Gregory
* Shubhajeet, Baral
* Do, Khuong
* Nguyen, Thuong
*
* Date: 05-26-2022
*/

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
using UnityEngine.InputSystem;

public class RuntimeGUI : MonoBehaviour
{
	// input system
	PixelWalker_InputActions pixelWalker_InputActions;
    InputAction submitAction;

	#region SCREEN CONTROLS
	// Main UI Elements (Visible at Runtime)
	private VisualElement daveOutGroup;
    private VisualElement daveInGroup;
    private Button menuBtn;
    private Button resetBtn;
    private Button submitBtn;
    private Button cancelBtn;
    private Button exitBtn;
    private TextField userInputField;
    private VisualElement userInputFieldInnerText;
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
    private Button navigationLevelLodeBtn;
    private Button standingPhysicsLevelLoadBtn;

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
	#endregion

	#region LOGIC VARS
	//Variables to store input and current selected action
	private UserInputHandler userInputHandler;
    private EngineType engine;
    GptApiKey keyEncryptor;
    
    //CONSTANTS
    private const int DELAY = 4;
    private const string KEY_FILE = "key.txt";
    private const string PROMPT_FILE = "prompt.JSON";
    private const string KEY_DIRECTORY = "PixelWalker";

    //Variables for saving
    private string key;
    private string debugMessage = "";
    private string keyPath;

    //List used to fill actions radio button group
    List<string> actionlist = new List<string>();

    //Scene GUI connection
    SceneGuiInterface sceneConnection;

    bool closePopUp = false;
	#endregion

	private void Awake()
	{
        pixelWalker_InputActions = new PixelWalker_InputActions();
		submitAction = pixelWalker_InputActions.UI.Submit;
    }

	void OnEnable()
    {
        #region UI REFERENCES
        //Root visual element of the UI Document
        var rootVE = GetComponent<UIDocument>().rootVisualElement;
        var d = GetComponent<UIDocument>();
        sceneConnection = FindObjectOfType(typeof(SceneGuiInterface)) as SceneGuiInterface;

        //Initialize Main UI elements
        menuBtn = rootVE.Q<Button>("menu");
        resetBtn = rootVE.Q<Button>("reset");
        submitBtn = rootVE.Q<Button>("submit");
        cancelBtn = rootVE.Q<Button>("cancel");
        exitBtn = rootVE.Q<Button>("exit-applicaiton");
        userInputField = rootVE.Q<TextField>("user-input");
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
        navigationLevelLodeBtn = rootVE.Q<Button>("level-1");
        standingPhysicsLevelLoadBtn = rootVE.Q<Button>("level-2");

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

        //Initialize engine window elements
        engineWindow = rootVE.Q<VisualElement>("engine-select");
        engineConfirmBtn = rootVE.Q<Button>("confirm-engine");
        engineRadioGroup = rootVE.Q<RadioButtonGroup>("engine-radio-group");
		#endregion

		//Initializes and fills the Action list radio groups witht the appropriate action from uxml
		actionRadioGroup = rootVE.Q<RadioButtonGroup>("action-list");
        foreach (var option in actionRadioGroup.choices)
        {
            actionlist.Add(option.ToLower());
        }

        //Fuctionality of all buttons added here
        submitBtn.clicked += OnSubmitHandler;
        cancelBtn.clicked += sceneConnection.CancelBehavior;
        menuBtn.clicked += OnMainMenuClicked;
        resetBtn.clicked += ReloadScene;
        menuResetBtn.clicked += ReloadScene;
        levelSelectBtn.clicked += OpenSceneMenu;
        apiKeyBtn.clicked += OpenApiInputMenu;
        apiSubmitBtn.clicked += SetApiKey;
        engineMenuBtn.clicked += OpenEngineMenu;
        engineConfirmBtn.clicked += SelectEngine;
        exitBtn.clicked += Application.Quit;

        submitAction.Enable();
        submitAction.performed += OnSubmitKeyPressedHandler;

        //Level select buttons -- refer to scene build order
        navigationLevelLodeBtn.clicked += () => SceneManager.LoadScene(0);
        standingPhysicsLevelLoadBtn.clicked += () => SceneManager.LoadScene(1);

		#region GPT-3 API KEY
		//Initializing directory path to store encrypted key
		keyPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), KEY_DIRECTORY);
        Directory.CreateDirectory(keyPath);
        keyPath = Path.Combine(keyPath, KEY_FILE);

        keyEncryptor = new GptApiKey(keyPath.ToString());
        if (File.Exists(keyPath))
        {
            try
            {
                key = keyEncryptor.GetKeyFromFile();
				//StartCoroutine(DebugOnEnablePopUp(key, 10));
            }
            catch (Exception e)
            {
                var msg = "Could not get the api key from the file.\n\n" + e.Message;
				//StartCoroutine(DebugOnEnablePopUp(msg, 10));
            }

            File.SetAttributes(keyPath, FileAttributes.Hidden);
        }
        else
        {
            OpenApiInputMenu();
        }
		#endregion

	}

	private void OnDisable()
    {
        submitAction.Disable();
    }

    private void Start()
	{
        userInputHandler = new UserInputHandler(sceneConnection.GetPropsList(), key, PROMPT_FILE, engine);

		
		foreach (var item in userInputField.Children())
		{
			if (item.name == "unity-text-input")
			{
				userInputFieldInnerText = item;
			}
		}
		StartCoroutine(KeepInputFieldFocused());
	}

    /// <summary>
    /// Sets the RadioButtonGroup to the behavior 
    /// that is being executed
    /// </summary>
    /// <param name="behavior">the current BehaviorType executed</param>
    public void SetCurrentBehavior(BehaviorType behavior)
    {
        if (behavior == BehaviorType.TurnOn)
        {
            actionRadioGroup.value = 0;
        }
        else if(behavior == BehaviorType.TurnOff)
        {
            actionRadioGroup.value = 1;
        }
        else if(behavior == BehaviorType.Navigate)
        {
            actionRadioGroup.value = 2;
        }
        else
        {
            actionRadioGroup.value = -1;
        }
    }

    /// <summary>
    /// Async method to send user input to GPT-3
    /// Prints command parses to gpt parse window,
    /// and prints question replys to dave's output window. if no API key 
    /// is supplied  the it creates a pop up (refer to CreatePopUp())
    /// to display error, else calls on the User Input Handler to classify 
    /// </summary>
    async void OnSubmitHandler()
    {
        string replyGptWindow = "...";
        string replyDaveWindow = "...";
        GptResponse responce = null;

        try
        {
            // signalable popup
            var msg = "Getting responce from GPT-3...";
            StartCoroutine(CreateSignaledPopup(msg));
            responce = await userInputHandler.GetGptResponce(userInputField.value);
            userInputFieldInnerText.Focus();
            closePopUp = true;
        }
        catch (Exception e)
        {
            var msg = $"An error occured while sending the message to GPT-3.\n\n" +
                      $"Error: {e.Message}";
            var debugMsg = msg + $"\n\nTraceback:{e.StackTrace}";
            Debug.LogWarning(debugMsg);
            StartCoroutine(CreateTimedPopUp(debugMsg, 20));
            return;
        }

        var responceProperties = responce.BehaviorProperties;
        if (responce.Type == InputType.Command)
        {
            replyGptWindow = "\nBehavior:\n\t " + responceProperties.Behavior
                                    + "\nObject:\n\t " + responceProperties.Object
                                    + "\nLocation:\n\t " + responceProperties.Location;

            replyDaveWindow = responce.GeneratedText.ToString();
        }
        else if (responce.Type == InputType.Question)
        {
            replyDaveWindow = responce.GeneratedText.ToString();
        }
        else if (responce.Type == InputType.Conversation)
        {
            replyDaveWindow = responce.GeneratedText.ToString();
        }
        else
        {
            replyGptWindow = responce.GeneratedText.ToString();
        }
        gptParseOutput.value = replyGptWindow;
        daveOutput.value = replyDaveWindow;

		if (responce.Type == InputType.Command)
		{
            SetCurrentBehavior(responce.BehaviorProperties.Behavior);
            var result = await sceneConnection.StartBehavior(responce.BehaviorProperties);

            if (result.Cancelled)
            {
                SetCurrentBehavior(BehaviorType.None);
                var msg = $"Behavior was cancelled while performing {result.Behavior}.";
				await CreateTimedPopUp(msg, DELAY);
			}
            else if (result.Success)
            {
                SetCurrentBehavior(BehaviorType.None);
                Debug.Log( $"{result.Behavior} successfully finished!");
            }
			else
			{
                SetCurrentBehavior(BehaviorType.None);
                var msg = $"I couldn't perform the requested action. {result.Message}";
                await CreateTimedPopUp(msg, DELAY);
            }
        }
	}

	IEnumerator DebugOnEnablePopUp(string msg, float duration)
	{
        yield return new WaitForUpdate();
        yield return new WaitForSeconds(0.5f);
		StartCoroutine(CreateTimedPopUp(msg, duration));
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
            StartCoroutine(CreateTimedPopUp(debugMessage, DELAY));
        }
        else
        {
            var testKey = apiInput.value;
			
            string test = await TestGptResponse(testKey);
            Debug.Log(test);
            if (test != null)
            {
				try
				{
                    if (File.Exists(keyPath))
                    {
                        File.Delete(keyPath);
                    }

                    keyEncryptor.SaveKeyToFile(testKey);

                    key = testKey;
                    File.SetAttributes(keyPath, FileAttributes.Hidden);
                }
				catch (Exception e)
				{
                    var msg = "Could not save the api key to the file.\n\n" + e.Message;

                    await CreateTimedPopUp(msg, DELAY);
                }

            }
            else
            {
                throw new Exception("Key not valid");
            }
        }

    }

    /// <summary>
    /// Method to send a test string to GPT-3 to
    /// verify key validity
    /// always used the Ada tier engine for cost
    /// saving
    /// </summary>
    /// <param name="key">the unencrypted api key</param>
    /// <returns></returns>
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
            userInputHandler = new UserInputHandler(sceneConnection.GetPropsList(), key, PROMPT_FILE, engine);
        }
        catch (Exception)
        {
            debugMessage = "Key not Valid. Enter another one and try again.";
            StartCoroutine(CreateTimedPopUp(debugMessage, DELAY));
            throw;
        }
        ToggleMainUI(true);
        StartCoroutine(CreateTimedPopUp(debugMessage, DELAY));
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
    IEnumerator CreateTimedPopUp(string message, float secondsVisible)
    {
        errorLabel.text = message;
        infoWindow.style.display = DisplayStyle.Flex;
        yield return new WaitForSeconds(secondsVisible);
        infoWindow.style.display = DisplayStyle.None;
    }

	/// <summary>
	/// Creates a pop up window that displays until a close signal is received.
	/// </summary>
	/// <param name="message"> a string variable for the message 
	/// to be displayed on the pop up</param>
	/// <returns>A Couroutine that delays by secondsVisble</returns>
	IEnumerator CreateSignaledPopup(string message)
    {
		closePopUp = false;
		errorLabel.text = message;
        infoWindow.style.display = DisplayStyle.Flex;

		while (!closePopUp)
		{
            yield return null;
		}
		
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
    void SelectEngine()
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


        userInputHandler = new UserInputHandler(sceneConnection.GetPropsList(), key, PROMPT_FILE, engine);
        Debug.Log(engineRadioGroup.value.ToString());
        ToggleMainUI(true);
    }

    async void OnSubmitKeyPressedHandler(InputAction.CallbackContext context)
    {
        OnSubmitHandler();
        await Task.Delay(100);
    }
    IEnumerator KeepInputFieldFocused()
	{
        yield return new WaitForSecondsRealtime(1f);

        while (true)
		{
			if (userInputField.focusController.focusedElement != userInputField)
			{
			    userInputFieldInnerText.Focus();
			}

			yield return new WaitForSecondsRealtime(1f);
        }
	}

	public static bool IsElementFocused(VisualElement visualElement)
	{
		return visualElement.focusController.focusedElement == visualElement;
	}
}