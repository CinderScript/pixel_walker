using System;
using System.Collections.Generic;
using UnityEngine;

public class ChildCollisionListener : MonoBehaviour
{
	public bool DetectInterChildCollisions = false;
	public Action<GameObject, Collision> OnCollisionStay { get; set; }

	private Dictionary<int, string> childColliders = new Dictionary<int, string>();
	private HashSet<int> childIDs = new HashSet<int>();
    void Awake()
    {
        var rBodies = GetComponentsInChildren<Rigidbody>();
		foreach (var rb in rBodies)
		{
			var childListener = rb.gameObject.AddComponent<ChildCollisionThrower>();
			childListener.OnCollisionStayEvent += CollisionStayHandler;
			
			var collisionObjectID = rb.gameObject.GetInstanceID();

			childIDs.Add(collisionObjectID);
			childColliders[collisionObjectID] = rb.name;
		}
    }

	private void CollisionStayHandler(GameObject reportingObject, Collision collision)
	{
		var collisionObjectID = collision.gameObject.GetInstanceID();
		var objName = reportingObject.name;
		
		bool isChild = childIDs.Contains( collisionObjectID );
		
		if ( DetectInterChildCollisions )
		{
			OnCollisionStay(reportingObject, collision);
		}
		else if ( !isChild )
		{
			OnCollisionStay(reportingObject, collision);
		}
	}
}