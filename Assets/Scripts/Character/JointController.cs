using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointController : MonoBehaviour
{
	public Rigidbody Rb
	{
		get
		{
			return rb;
		}
	}
	private Rigidbody rb;
	private Joint joint;

	void Awake()
	{
		rb = GetComponent<Rigidbody>();
		joint = rb.GetComponent<Joint>();
	}

	public void ApplyTorque(in Vector3 torque)
	{
		rb.AddRelativeTorque( Vector3.Scale( torque, joint.axis ) );
	}
}