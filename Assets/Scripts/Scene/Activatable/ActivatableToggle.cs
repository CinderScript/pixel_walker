using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActivatableToggle : MonoBehaviour, Activatable
{
	private bool isDefault;

	protected virtual void Awake()
	{
		isDefault = true;
	}

	public void Activate()
	{
		isDefault = !isDefault;

		if (isDefault)
		{
			SetInitialState();
		}
		else
		{
			SetToggledState();
		}
	}

	public abstract void SetInitialState();
	public abstract void SetToggledState();
}