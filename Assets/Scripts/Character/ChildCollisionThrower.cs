using System;
using UnityEngine;

[DisallowMultipleComponent]
public class ChildCollisionThrower : MonoBehaviour
{
	public Action<GameObject, Collision> OnCollisionStayEvent { get; set; }

	private void OnCollisionStay(Collision collision)
	{
		OnCollisionStayEvent(gameObject, collision);
	}
}