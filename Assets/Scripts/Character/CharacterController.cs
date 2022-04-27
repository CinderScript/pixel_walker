using System.Collections;
using System.Collections.Generic;

using Unity.MLAgents.Actuators;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
	public const int MAX_LIMB_TORQUE = 1500;

	public int NumberOfJoints => jointControllers.Length;
	public int NumberOfObservations { get; private set; }
	public float TotalTorqueUsed { get; private set; }

	private JointController[] jointControllers;

	private void Awake()
	{
		var joints = GetComponentsInChildren<Joint>();

		// don't add fixed joints that may be found
		var tempList = new List<JointController>();
		for (int i = 0; i < joints.Length; i++)
		{
			if ( joints[i].GetType() != typeof(FixedJoint) ) // no fixed joints
			{
				tempList.Add( joints[i].gameObject.AddComponent<JointController>() );
			}
		}

		jointControllers = tempList.ToArray();
		NumberOfObservations = jointControllers.Length * 4;
	}

	public void ProcessActionBuffers(ActionBuffers actionBuffers)
	{
		// must keep track of force used
		float totalTorque = 0;

		// convert action buffers into a list of vectors
		// representing the amount of torque to apply to each limb
		Vector3[] torques = new Vector3[NumberOfJoints];
		int jointIndex = 0;
		for (int actionIndex = 0; jointIndex < NumberOfJoints; jointIndex++)
		{
			var x = actionBuffers.ContinuousActions[actionIndex++];
			var y = actionBuffers.ContinuousActions[actionIndex++];
			var z = actionBuffers.ContinuousActions[actionIndex++];
			torques[jointIndex] = new Vector3(x, y, z) * MAX_LIMB_TORQUE;
			totalTorque = Mathf.Abs(x) + Mathf.Abs(y) + Mathf.Abs(z);
		}
		TotalTorqueUsed = totalTorque * MAX_LIMB_TORQUE;
		ApplyTorque(torques);

		// complete additional actions
		if (actionBuffers.ContinuousActions.Length > NumberOfJoints * 3)
		{
			// additional continuous actions
		}

		if (actionBuffers.DiscreteActions.Length > 0)
		{
			// additional discrete actions
		}
	}
	public Vector3[] GetLimbObservations()
	{
		// add velocity, angular velocity, position, and rotation 
		// for each joint into an observations list.
		Vector3[] observations = new Vector3[NumberOfObservations];
		for (int jointIndex = 0, obsIndex = 0; jointIndex < NumberOfJoints; jointIndex++)
		{
			observations[obsIndex++] = jointControllers[jointIndex].Rb.velocity;
			observations[obsIndex++] = jointControllers[jointIndex].Rb.angularVelocity;
			observations[obsIndex++] = jointControllers[jointIndex].transform.position;
			observations[obsIndex++] = jointControllers[jointIndex].transform.localRotation.eulerAngles;
		}
		return observations;
	}

	private void ApplyTorque(Vector3[] torques)
	{
		for (int i = 0; i < NumberOfJoints; i++)
		{
			jointControllers[i].ApplyTorque( in torques[i] );
		}
	}
}