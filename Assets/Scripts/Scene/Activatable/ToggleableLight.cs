using UnityEngine;

public class ToggleableLight : ActivatableToggle
{
	private bool initialState;
	private new Light light;

	protected override void Awake()
	{
		base.Awake();
		initialState = gameObject.activeSelf;
		light = GetComponent<Light>();
	}
	public override void SetToggledState()
	{
		light.enabled = !initialState;
	}
	public override void SetInitialState()
	{
		light.enabled = initialState;
	}
}
