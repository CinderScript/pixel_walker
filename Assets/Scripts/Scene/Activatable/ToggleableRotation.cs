using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleableRotation : ActivatableToggle
{
	public float targetSpeed = 750f;
	public float acceleration = 300f;
	public Vector3 rotationAxis = new Vector3(0,1,0);

	private float currentSpeed = 0;
	private Coroutine startRotation;
	private Coroutine stopRotation;
	private bool isStillRunning = false;

	public override void triggerOnState()
	{
		if (isStillRunning)
		{
			StopCoroutine(stopRotation);
		}
		startRotation = StartCoroutine(StartRotation());
	}
	public override void triggerOffState()
	{
		if (isStillRunning)
		{
			StopCoroutine(startRotation);
		}
		stopRotation = StartCoroutine(StopRotation());
	}

	IEnumerator StartRotation()
	{
		isStillRunning = true;
		
		// accelerate
		while (currentSpeed < targetSpeed)
		{
			currentSpeed += acceleration * Time.deltaTime;
			transform.Rotate(rotationAxis, currentSpeed * Time.fixedDeltaTime);
			yield return new WaitForFixedUpdate();
		}

		while (true)
		{
			transform.Rotate(rotationAxis, targetSpeed * Time.deltaTime);
			yield return new WaitForFixedUpdate();
		}
	}
	IEnumerator StopRotation()
	{
		// accelerate
		while (currentSpeed > 0)
		{
			currentSpeed -= acceleration * Time.deltaTime;
			transform.Rotate(rotationAxis, currentSpeed * Time.fixedDeltaTime);
			yield return new WaitForFixedUpdate();
		}

		currentSpeed = 0;
		isStillRunning = false;
	}
}
