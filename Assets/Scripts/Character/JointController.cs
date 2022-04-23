using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointController : MonoBehaviour
{
	public string Name { get; }
	private readonly Rigidbody rb;
	private readonly Joint joint;
	private readonly Vector3 torqueAxisScaler;
	
	public float AverageLimbVelocity { get; private set; }

	public void AddTorque(float x, float y, float z)
	{
		rb.AddRelativeTorque(x * torqueAxisScaler.x, y * torqueAxisScaler.y, z * torqueAxisScaler.z);
	}

	public Vector3 TorqueScaler() {
		return joint.axis;
	}
}