using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Activatable
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