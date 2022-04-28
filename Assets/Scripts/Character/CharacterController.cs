using System.Collections.Generic;

using UnityEngine;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System;

public class CharacterController : MonoBehaviour
{
	public const int MAX_LIMB_TORQUE = 1500;

	public int NumberOfJoints => jointControllers.Length;
	public int NumberOfObservations { get; private set; }
	public float TotalTorqueUsed { get; private set; }

	private JointController[] jointControllers;
	private Vector3 MAX_MIN_VELOCITY_ESTIMATE = new Vector3(40, 40, 40);
	private Vector3 MAX_MIN_ROT_VELOCITY_ESTIMATE = new Vector3(200, 200, 200);
	private Vector3 MAX_MIN_AGENT_WORLD_SIZE = new Vector3(30, 30, 30);

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
	public void AddLimbObservationsTo(VectorSensor sensor)
	{
		// add velocity, angular velocity, position, and rotation 
		// for each joint into an observations list.
		for (int jointIndex = 0; jointIndex < NumberOfJoints; jointIndex++)
		{
			var jointVelocity = jointControllers[jointIndex].Rb.velocity;
			var jointAngularVelocity = jointControllers[jointIndex].Rb.angularVelocity;
			var jointPosition = jointControllers[jointIndex].transform.localPosition;
			var jointRotation = jointControllers[jointIndex].transform.localRotation.eulerAngles;

			// NORMALIZE DATA
			// when falling, max velocity was 2, so make max 40
			// when falling, max angular velocity was 10, so make max 200
			// making world space a max of -30 to 30 units for the agent
			CharacterController.NormalizeVectorValues(ref jointVelocity, MAX_MIN_VELOCITY_ESTIMATE);
			CharacterController.NormalizeVectorValues(ref jointAngularVelocity, MAX_MIN_ROT_VELOCITY_ESTIMATE);
			CharacterController.NormalizeVectorValues(ref jointPosition, MAX_MIN_AGENT_WORLD_SIZE);
			jointRotation = jointRotation / 180f - Vector3.one;    //normalized [-1, 1]

			Debug.Assert(jointVelocity.x < 1);
			Debug.Assert(jointVelocity.y < 1);
			Debug.Assert(jointVelocity.z < 1);
			Debug.Assert(jointVelocity.x > -1);
			Debug.Assert(jointVelocity.y > -1);
			Debug.Assert(jointVelocity.z > -1);
			Debug.Assert(jointAngularVelocity.x < 1);
			Debug.Assert(jointAngularVelocity.y < 1);
			Debug.Assert(jointAngularVelocity.z < 1);
			Debug.Assert(jointAngularVelocity.x > -1);
			Debug.Assert(jointAngularVelocity.y > -1);
			Debug.Assert(jointAngularVelocity.z > -1);
			Debug.Assert(jointPosition.x < 1);
			Debug.Assert(jointPosition.y < 1);
			Debug.Assert(jointPosition.z < 1);
			Debug.Assert(jointPosition.x > -1);
			Debug.Assert(jointPosition.y > -1);
			Debug.Assert(jointPosition.z > -1);

			sensor.AddObservation(jointVelocity);
			sensor.AddObservation(jointAngularVelocity);        
			sensor.AddObservation(jointPosition);
			sensor.AddObservation(jointRotation);
		}
	}

	private void ApplyTorque(Vector3[] torques)
	{
		for (int i = 0; i < NumberOfJoints; i++)
		{
			jointControllers[i].ApplyTorque( in torques[i] );
		}
	}

	private static float Normalize(float value, float min, float max)
	{
		return (value - min) / (max - min) * 2 - 1;
	}

	/// <summary>
	/// Converts the given vector to a vector with values between -1 and 1.
	/// vector3.x is used as the min and max for each vector axis.
	/// </summary>
	/// <param name="vector">vector to normalize</param>
	/// <param name="symmetricalMinMax">vector denoting the values</param>
	private static void NormalizeVectorValues(ref Vector3 vector, in Vector3 symmetricalMinMax)
	{
		//     (vector - min) / (max - min) * 2 - 1					
		//   = (vector + symmetricalMinMax) / (max - min) * 2 - 1	// - min = max if symmetric
		//	 = (vector + symmetricalMinMax) / (2 * max) * 2 - 1		// max - (-max) = 2 * max
		//	 = (vector + symmetricalMinMax) / max - 1				// 2s cancel out
		vector = (vector + symmetricalMinMax) / (symmetricalMinMax.x) - Vector3.one;
	}
}