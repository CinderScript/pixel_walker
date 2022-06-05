using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActivatable
{
	void Activate();
}

public enum ActivationState
{
	On,
	Off,
	State_One,
	State_Two,
	State_Three
}