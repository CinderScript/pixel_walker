using UnityEngine;

public class ToggleableLight : ActivatableToggle
{
	private new Light light;

	protected override void Awake()
	{
		light = GetComponent<Light>();
		base.Awake();
	}
	public override void triggerOnState()
	{
		light.enabled = true;
	}
	public override void triggerOffState()
	{
		light.enabled = false;
	}
}
