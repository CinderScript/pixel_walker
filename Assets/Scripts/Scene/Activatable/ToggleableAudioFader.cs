using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioFader))]
public class ToggleableAudioFader : ActivatableToggle
{
	[SerializeField]
	private float fadeSpeed = 0.75f;

	private AudioFader fader;

	protected override void Awake()
	{
		fader = GetComponent<AudioFader>();
		base.Awake();
		
		var source = GetComponent<AudioSource>();
		if (CurrentState == ActivationState.On)
		{
			source.Play();
		}
		else
		{
			source.Stop();
		}
	}
	public override void triggerOnState()
	{
		fader.FadeIn(fadeSpeed);
	}
	public override void triggerOffState()
	{
		fader.FadeOut(fadeSpeed);
	}
}
