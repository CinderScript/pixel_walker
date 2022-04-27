using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ChildCollisionListener))]
public class BalanceSensor : MonoBehaviour
{
	public GameObject Gizmo;

	public float TotalMass { get; private set; }
	/// <summary>
	/// Calculates the position denoting the center of mass of this object 
	/// taking into account all child rigidbodies.  This is the is the 
	/// mean location of the distribution of mass in space. See wikipedia 
	/// for the formula used: https://en.wikipedia.org/wiki/Center_of_mass
	/// </summary>
	public Vector3 CenterOfMass { get
		{
			Vector3 weightedPositionSum = Vector3.zero;

			for (int i = 0; i < rigidbodies.Length; i++)
			{
				var rb = rigidbodies[i];
				weightedPositionSum += rb.worldCenterOfMass * rb.mass;
			}
			return weightedPositionSum / TotalMass;
		} 
	}

	private Rigidbody[] rigidbodies;

	void Awake()
	{
		TotalMass = 0;
		rigidbodies = GetComponentsInChildren<Rigidbody>();
		foreach (var rb in rigidbodies)
		{
			TotalMass += rb.mass;
		}

		GetComponent<ChildCollisionListener>().OnCollisionStay += OnCollisionStayHandler;
	}

	void Start()
	{
		Gizmo = Instantiate(Gizmo, CenterOfMass, Quaternion.identity);
	}

	private void Update()
	{
		Gizmo.transform.position = CenterOfMass;
	}

	private void OnCollisionStayHandler(GameObject obj, Collision collision)
	{
		ContactPoint[] contactPoints = new ContactPoint[collision.contactCount];
		collision.GetContacts(contactPoints);
	}
}