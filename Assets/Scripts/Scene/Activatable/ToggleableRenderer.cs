using UnityEngine;

public class ToggleableRenderer : ActivatableToggle
{
	[Header("Initial Object State")]
	[Tooltip("Sets initial state to either be Visible or Not Visible")]
	[SerializeField]
	private bool isVisible = false;
	private new MeshRenderer renderer;

	protected override void Awake()
	{
		renderer = GetComponent<MeshRenderer>();
		base.Awake();
	}

	public override void triggerOnState()
	{
		renderer.forceRenderingOff = false;
	}
	public override void triggerOffState()
	{
		renderer.forceRenderingOff = true;
	}
}