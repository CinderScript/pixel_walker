using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PropInfo : MonoBehaviour
{
	[Header("Prop Identification")]
	public string Name;
	public string Description;

	// set at runtime
	[Header("Set At RTime")]
	[Tooltip("This object is assigned at runtime by checking the prop's parent objects")]
	public Room room;

	private void Awake()
	{
		room = GetComponentInParent<Room>();
	}
}