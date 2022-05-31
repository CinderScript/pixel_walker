/**
 *	Project:		Pixel Walker
 *	
 *	Description:	Agents and the HandController need collision information. 
 *					ChildRbCollisionListener finds child objects with colliders 
 *					and exposes events that Agents and controllers can subscribe.
 *					
 *	Author:			Pixel Walker -
 *						Maynard, Gregory
 *						Shubhajeet, Baral
 *						Do, Khuong
 *						Nguyen, Thuong						
 *					
 *	Date:			05-30-2022
 */

using System;
using System.Collections.Generic;
using UnityEngine;

public class ChildRbCollisionListener : MonoBehaviour
{
	public bool DetectInterChildCollisions = false;
	public Action<GameObject, Collision> OnCollisionEnter { get; set; }
	public Action<GameObject, Collision> OnCollisionExit { get; set; }

	private Dictionary<int, string> childColliders = new Dictionary<int, string>();
	private HashSet<int> childIDs = new HashSet<int>();
    void Awake()
    {
        var rBodies = GetComponentsInChildren<Rigidbody>();
		foreach (var rb in rBodies)
		{
			var childListener = rb.gameObject.AddComponent<CollisionThrower>();
			childListener.OnCollisionEnterEvent += OnCollisionEnterHandler;
			childListener.OnCollisionExitEvent += OnCollisionExitHandler;

			var collisionObjectID = rb.gameObject.GetInstanceID();

			childIDs.Add(collisionObjectID);
			childColliders[collisionObjectID] = rb.name;
		}
    }

	private void OnCollisionEnterHandler(GameObject reportingObject, Collision collision)
	{
		if (OnCollisionEnter != null)
		{
			var collisionObjectID = collision.gameObject.GetInstanceID();
			bool isChild = childIDs.Contains(collisionObjectID);

			if (DetectInterChildCollisions)
			{
				OnCollisionEnter(reportingObject, collision);
			}
			else if (!isChild)
			{
				OnCollisionEnter(reportingObject, collision);
			}
		}
	}

	private void OnCollisionExitHandler(GameObject reportingObject, Collision collision)
	{
		if (OnCollisionExit != null)
		{
			var collisionObjectID = collision.gameObject.GetInstanceID();
			bool isChild = childIDs.Contains(collisionObjectID);

			if (DetectInterChildCollisions)
			{
				OnCollisionExit(reportingObject, collision);
			}
			else if (!isChild)
			{
				OnCollisionExit(reportingObject, collision);
			}
		}
	}
}