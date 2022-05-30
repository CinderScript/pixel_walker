using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActivatableToggle : MonoBehaviour, Activatable
{
	protected bool isToggled { get; private set; }

	protected virtual void Awake()
	{
		isToggled = false;
	}

	public void Activate()
	{
		isToggled = !isToggled;

		if (isToggled)
		{
			SetToggledState();
		}
		else
		{
			SetInitialState();
		}
	}

	public abstract void SetInitialState();
	public abstract void SetToggledState();
}