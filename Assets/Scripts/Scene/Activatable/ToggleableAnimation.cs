using System.Collections;

using UnityEngine;

public class ToggleableAnimation : ActivatableToggle
{
	private new Animation animation;

	protected override void Awake()
	{
		animation = gameObject.GetComponent<Animation>();
		base.Awake();

		// animation doesn't turn on/off on scene load, so wait a little
		StartCoroutine(SetStartingState());
	}
	IEnumerator SetStartingState()
	{
		yield return new WaitForFixedUpdate();
		yield return new WaitForSeconds(1);
		if (CurrentState == ActivationState.On)
		{
			triggerOnState();
		}
		else
		{
			triggerOffState();
		}
	}

	public override void triggerOffState()
	{
		animation.Stop();
	}
	public override void triggerOnState()
	{
		animation.Play();
	}
}