using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using System.Text.RegularExpressions;

public class RuntimeGUI : MonoBehaviour
{

    // Main UI Elements (Visible at Runtime)
    private VisualElement daveOutGroup;
    private VisualElement daveInGroup;
    private Button menuBtn;
    private Button resetBtn;
    private Button submit;
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
    

    Exception ex;

    //List used to fill actions radio button group
    List<string> actionlist = new List<String>();

    private UserInputHandler handler; 
    private EngineType engine;

    private const int DELAY = 1;
    private string keyString = "";
    

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
        submit.clicked += SendToGPT;
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

    //Calls upon The InputHandlerFramework's UserInputHandler.cs  to send and parse user input, sends error if API key is missing.
    async void SendToGPT()
    {
        gptParseOutput.value = "Output Here...";
        daveOutput.value = "Output Here...";
        //Prints error if there is no API Key present
        if (keyString == "")
        {
            ex = new Exception("No Key Provided: Menu > API Key to add.");
            StartCoroutine(CreatePopUp(ex.Message, DELAY));
            Debug.Log("ERROR: NO KEY API PROVIDED");
        }
        else
        {
            handler = new UserInputHandler(keyString, "TODO: *fileprompts.txt*", engine);

            GptResponse responce;
            try
			{
                responce = await handler.GetGptResponce(userInput.value);
            }
			catch (Exception)
			{

				throw;
			}
            
            
            Debug.Log(responce.GeneratedText);
            var responceProperties = responce.BehaviorProperties;
            if (responce.Type == InputType.Command)
            {
                gptParseOutput.value = "\nBehavior:\n\t " + responceProperties.Behavior 
                                        + "\nObject:\n\t " + responceProperties.Object
                                        + "\nLocation:\n\t " + responceProperties.Location;
            }
            else if (responce.Type == InputType.Question)
            {
                daveOutput.value = responce.GeneratedText.ToString();
            }
            else
            {
                StartCoroutine(CreatePopUp(ex.Message, DELAY));
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

    void OpenSceneMenu(){
        if(levelSelectWindow.style.display == DisplayStyle.Flex){
            levelSelectWindow.style.display = DisplayStyle.None;
        } else {
            levelSelectWindow.style.display = DisplayStyle.Flex;
            apiWindow.style.display = DisplayStyle.None;
            menuWindow.style.display = DisplayStyle.None;
        }  
    }
    void OpenEngineMenu()
    {
        CloseMenuElements();
        engineWindow.style.display = DisplayStyle.Flex;
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



    //Sets keystring variable to entered the API key
    //Pops up window on excution to say key has been entered
    //Will pop up cautionary message if left empty
    async void SetApiKey()
    {
        string tempText = "";
        keyString = apiInput.value;
        if(apiInput.value == ""){
            ex = new Exception("Field cannot Be left blank.");
            tempText = ex.Message;
        }
        else{
            Debug.Log(keyString);
            engine = EngineType.Ada;
            handler = new UserInputHandler(keyString, "TODO: *fileprompts.txt*", engine);

            GptResponse responce;
            try
			{
                responce = await handler.GetGptResponce("-+-");
            }
            catch (Exception)
			{
				throw;
			}
			
            if (ex != null){
                tempText = "Key not valid. Try again\n";
                Debug.Log(ex.Message);
            }
            else{
                tempText = "Success";
                ToggleMainUI(true);
            }    
        }
        engine = EngineType.Davinci;
        handler = new UserInputHandler(keyString, "TODO: *fileprompts.txt*", engine);
        StartCoroutine(CreatePopUp(tempText, DELAY));
        Debug.Log(ex.Message);
        //ToggleMainUI(true);

    }

    //Creates a pop up window to display system dialogue
    //in: string 'message' to be displayed as message on pop up
    //in: integer 'secondsVisible' is the number of 
    //    seconds pop up will be displayed
    //out: IEnumerator coroutine object which delays by secondsVisible
    IEnumerator CreatePopUp(string message, float secondsVisible)
    {
        errorLabel.text = message;
        infoWindow.style.display = DisplayStyle.Flex;
        yield return new WaitForSeconds(1);
        infoWindow.style.display = DisplayStyle.None;
        daveInGroup.style.display = DisplayStyle.Flex;
        daveOutGroup.style.display = DisplayStyle.Flex;
        actionRadioGroup.style.display = DisplayStyle.Flex;
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
        engineWindow.style.display = DisplayStyle.None;
    }
    

    void SelectEngine(){
        if(engineRadioGroup.value == 1){
            engine = EngineType.Curie;
        }
        else if(engineRadioGroup.value == 2){
            engine = EngineType.Babbage;
        }
        else if(engineRadioGroup.value == 3){
            engine = EngineType.Ada;
        }
        else{
            engine = EngineType.Davinci;
        }

        handler = new UserInputHandler(keyString, "TODO: *fileprompts.txt*", engine);
        Debug.Log(engineRadioGroup.value.ToString());
        ToggleMainUI(true);
    }
         
}
