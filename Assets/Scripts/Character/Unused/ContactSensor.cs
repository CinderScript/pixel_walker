using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContactSensor : MonoBehaviour
{
	[SerializeField]
	private bool ContactDetected = false;
	public float SeparationDistanceThreashold = 0.004f;
	public bool IsContactDetected => ContactDetected;

	private void OnCollisionStay(Collision collision)
	{
        ContactPoint[] contactPoints = new ContactPoint[collision.contactCount];
        collision.GetContacts(contactPoints);

		float minDistance = float.MaxValue;
		for (int i = 0; i < contactPoints.Length; i++)
		{
			if (contactPoints[i].separation < minDistance)
			{
				minDistance = contactPoints[i].separation;
			}
		}

		if (minDistance < SeparationDistanceThreashold)
			ContactDetected = true;
		else
			ContactDetected = false;

	}
}
