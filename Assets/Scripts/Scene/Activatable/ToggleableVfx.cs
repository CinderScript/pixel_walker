using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ToggleableVfx : ActivatableToggle
{
	private VisualEffect effect;

	protected override void Awake()
	{
		effect = GetComponent<VisualEffect>();
		base.Awake();
	}
	public override void triggerOnState()
	{
		effect.Play();
	}
	public override void triggerOffState()
	{
		effect.Stop();
	}
}
