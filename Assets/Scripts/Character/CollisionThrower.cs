using System;
using UnityEngine;

[DisallowMultipleComponent]
public class CollisionThrower : MonoBehaviour
{
	public Action<GameObject, Collision> OnCollisionEnterEvent { get; set; }
	public Action<GameObject, Collision> OnCollisionExitEvent { get; set; }
	public Action<GameObject, ControllerColliderHit> OnCharacterCollision { get; set; }

	private void OnCollisionEnter(Collision collision)
	{
		OnCollisionEnterEvent?.Invoke(gameObject, collision);
	}
	private void OnCollisionExit(Collision collision)
	{
		OnCollisionExitEvent?.Invoke(gameObject, collision);
	}

	void OnControllerColliderHit(ControllerColliderHit hit)
	{
		OnCharacterCollision(gameObject, hit);
	}
}