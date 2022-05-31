using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ToggleableAudioSource : ActivatableToggle
{
	private AudioSource source;

	protected override void Awake()
	{
		source = GetComponent<AudioSource>();
		base.Awake();
	}
	public override void triggerOnState()
	{
		source.Play();
	}
	public override void triggerOffState()
	{
		source.Stop();
	}
}
