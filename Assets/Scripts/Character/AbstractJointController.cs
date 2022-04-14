using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractJointController : MonoBehaviour
{
	public Vector3 MaxJointTorque { get; private set; }

	public abstract void InitializeJoint(Vector3 torque);
}
