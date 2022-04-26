using System;
using UnityEngine;

public class ChildCollisionThrower : MonoBehaviour
{
	public GameObject Test;
	public Action<GameObject, Collision> OnCollisionStayEvent { get; set; }

	private void OnCollisionStay(Collision collision)
	{
		OnCollisionStayEvent(gameObject, collision);
	}
}