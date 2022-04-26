using System;
using System.Collections.Generic;
using UnityEngine;

public class ChildCollisionListener : MonoBehaviour
{
	public bool DetectInterChildCollisions = false;
	public Action<GameObject, Collision> OnCollisionStay { get; set; }

	private HashSet<int> childIDs = new HashSet<int>();
    void Awake()
    {
        var rBodies = GetComponentsInChildren<Rigidbody>();
		foreach (var rb in rBodies)
		{
			var childListener = rb.gameObject.AddComponent<ChildCollisionThrower>();
			childListener.OnCollisionStayEvent += CollisionStayHandler;
			childIDs.Add(rb.gameObject.GetInstanceID());
		}
    }

	private void CollisionStayHandler(GameObject obj, Collision collision)
	{
		bool isChild = childIDs.Contains( obj.GetInstanceID() );
		if (DetectInterChildCollisions)
		{
			OnCollisionStay(obj, collision);
		}
		else if ( !isChild )
		{
			OnCollisionStay(obj, collision);
		}
	}
}