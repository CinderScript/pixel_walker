using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScale : MonoBehaviour
{
    public float TimeScaleMultiplier = 1f;

	private void Awake()
	{
		Time.timeScale *= TimeScaleMultiplier;
		Time.fixedDeltaTime *= TimeScaleMultiplier;
	}
}
