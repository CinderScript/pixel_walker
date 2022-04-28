using System;
using UnityEngine;

[DisallowMultipleComponent]
public class ChildCollisionThrower : MonoBehaviour
{
	public Action<GameObject, Collision> OnCollisionEnterEvent { get; set; }
	public Action<GameObject, Collision> OnCollisionExitEvent { get; set; }

	private void OnCollisionEnter(Collision collision)
	{
		OnCollisionEnterEvent(gameObject, collision);
	}
	private void OnCollisionExit(Collision collision)
	{
		OnCollisionExitEvent(gameObject, collision);
	}
}