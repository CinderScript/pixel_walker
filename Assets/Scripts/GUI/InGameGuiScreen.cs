using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InGameGuiScreen : MonoBehaviour
{
	/*
	 * Each GUI element can be set by finding them using
	 * a VisualElement query from the root in void Start, 
	 * or by setting each in the inspector.
	 */

	[Header("GUI Buttons")]
	public Button MainMenu;
	public Button ReloadEnvironment;
	public Button Submit;

	// Each behavior Dave can run
	[Header("GUI Radio Button Group")]
	public RadioButton Stand;
	public RadioButton GoTo;
	public RadioButton Open;
	public RadioButton PickUp;
	public RadioButton Fetch;

	[Header("User Input Field")]
	public TextField UserInput;

	[Header("GUI Display Text")]
	public Label Gpt3ParseText;
	public Label MessageFromAgent;

	void Start()
	{
		// connect button event handlers to the relevant methods
		var root = GetComponent<UIDocument>().rootVisualElement;

		MainMenu.clicked += OnMainMenuClicked;

		/*
		 * Need to connect rest of buttons
		 */

	}

	void OnMainMenuClicked()
	{
		throw new NotImplementedException();
	}

	void OnReloadEnvironmentClicked()
	{
		throw new NotImplementedException();
	}

	void OnSubmitClicked()
	{
		throw new NotImplementedException();
	}

	public void SetCurrentBehavior(string behavior)
	{
		switch (behavior)
		{
			case "Stand":
				Stand.value = true;
				break;
			case "GoTo":
				GoTo.value = true;
				break;
			case "Open":
				Open.value = true;
				break;
			case "PickUp":
				PickUp.value = true;
				break;
			case "Fetch":
				Fetch.value = true;
				break;
			default:
				break;
		}
	}
}
