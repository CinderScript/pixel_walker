using UnityEngine;

public class ToggleableMaterial : ActivatableToggle
{
	[Header("Toggleable Material")]
	public Material material;

	private Material originalMaterial;

	protected override void Awake()
	{
		base.Awake();
		originalMaterial = GetComponent<Renderer>().material;
	}
	public override void SetToggledState()
	{
		GetComponent<Renderer>().material = material;
	}
	public override void SetInitialState()
	{
		GetComponent<Renderer>().material = originalMaterial;
	}
}
