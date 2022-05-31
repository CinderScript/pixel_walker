/**
 *	Project:		Pixel Walker
 *	
 *	Description:	ReferenceOrientation can be given a ReferenceBase that it will 
 *					use to set its position and then rotate to face the given 
 *					target. Used to provide a reference for Agent observation data.
 *					
 *					When training agents in reinforcement learning, it is 
 *					often best to not provide global rotation and position data, 
 *					but to always provide observation based on a common reference.
 *					
 *	Author:			Pixel Walker -
 *						Maynard, Gregory
 *						Shubhajeet, Baral
 *						Do, Khuong
 *						Nguyen, Thuong						
 *					
 *	Date:			05-30-2022
 */

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
