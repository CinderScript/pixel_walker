using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReferenceOrientation : MonoBehaviour
{

	public Transform ReferenceBase;

	public void UpdateOrientation(Transform target)
	{
		var direction = target.position - ReferenceBase.position;
		var position = new Vector3( ReferenceBase.position.x, 0, ReferenceBase.position.z);

		if (direction == Vector3.zero)
			transform.SetPositionAndRotation(ReferenceBase.position, Quaternion.identity);
		else
			transform.SetPositionAndRotation(ReferenceBase.position, Quaternion.LookRotation(direction));
	}
	public void UpdateOrientation()
	{
		transform.position = new Vector3(ReferenceBase.position.x, 0, ReferenceBase.position.z);
	}

}
