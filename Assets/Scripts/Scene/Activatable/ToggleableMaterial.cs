using UnityEngine;

public class ToggleableMaterial : ActivatableToggle
{
	[SerializeField]
	[Header("Toggleable Material")]
	private Material toggledMaterial;

	private Material originalMaterial;

	protected override void Awake()
	{
		originalMaterial = GetComponent<Renderer>().material;
		base.Awake();
	}
	public override void triggerOnState()
	{
		GetComponent<Renderer>().material = toggledMaterial;
	}
	public override void triggerOffState()
	{
		GetComponent<Renderer>().material = originalMaterial;
	}
}
