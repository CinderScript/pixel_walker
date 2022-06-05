using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActivatableToggle : MonoBehaviour, IActivatable
{
	[SerializeField]
	[Header("Initial Toggle State")]
	[Tooltip("Sets initial state to either be On or Off")]
	protected ActivationState initialState = ActivationState.Off;

	public ActivationState CurrentState { get; private set; }

	protected virtual void Awake()
	{
		CurrentState = initialState;
		if (initialState == ActivationState.On)
		{
			triggerOnState();
		}
		else
		{
			triggerOffState();
		}
	}

	public void Activate()
	{
		if (CurrentState == ActivationState.Off)
		{
			triggerOnState();
			CurrentState = ActivationState.On;
		}
		else
		{
			triggerOffState();
			CurrentState = ActivationState.Off;
		}
	}

	public abstract void triggerOffState();
	public abstract void triggerOnState();
}