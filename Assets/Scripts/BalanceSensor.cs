using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalanceSensor : MonoBehaviour
{
    public GameObject Gizmo;

	private List<GameObject> gizmos = new List<GameObject>();

	void Start()
	{
		var centerMass = GetComponent<Rigidbody>().centerOfMass + transform.position;
	}

	private void OnCollisionStay(Collision collision)
	{

		ContactPoint[] contactPoints = new ContactPoint[collision.contactCount];
		collision.GetContacts(contactPoints);
		foreach (var col in contactPoints)
		{
			Debug.Log($"Collision Separation: {col.separation}");
			var go = Instantiate(Gizmo, col.point, Quaternion.identity);
			gizmos.Add(go);
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		foreach (var item in gizmos)
		{
			Destroy(item);
		}
	}
}
