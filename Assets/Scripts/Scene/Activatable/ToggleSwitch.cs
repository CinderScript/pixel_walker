using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class ToggleSwitch : ActivatableTrigger
{
	[Header("On Activations")]
	[SerializeField]
	private List<ActivatableToggle> Activatables;

	[SerializeField]
	private ActivationState toggleState = ActivationState.Off;
	public ActivationState CurrentState {
		get
		{
			return toggleState;
		}
	}

	public override void TriggerActivatables()
	{
		if (toggleState == ActivationState.On)
		{
			toggleState = ActivationState.Off;
		}
		else
		{
			toggleState = ActivationState.On;
		}

		foreach (var activatable in Activatables)
		{
			activatable.Activate();
		}
	}
}