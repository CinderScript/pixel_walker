using System.Collections.Generic;

using UnityEngine;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class CharacterController : MonoBehaviour
{
	public ReferenceOrientation ReferenceOrientation;
	public Transform Pelvis;

	/// <summary>
	/// Scaler for limb torque value given by ml-agents (a value
	/// between -1 and 1). Max torque will be 1 * this value.
	/// </summary>
	public const int MAX_LIMB_TORQUE = 1500;

	public int NumberOfJoints => jointControllers.Length;
	public int NumberOfObservations { get; private set; }
	public float TotalTorqueUsed { get; private set; }
	public float AveLimbVelocitySqr => aveLimbVelocitySqr;

	private JointDriver[] jointControllers;
	private Vector3 MAX_MIN_VELOCITY_ESTIMATE = new Vector3(40, 40, 40);
	private Vector3 MAX_MIN_ROT_VELOCITY_ESTIMATE = new Vector3(200, 200, 200);
	private Vector3 MAX_MIN_AGENT_WORLD_SIZE = new Vector3(30, 30, 30);

	private float LIMB_VELOCITY_CHECK_RESOLUTION = 0.2f;
	private int limbVelocityTicksPerCountdown;
	private int currentAveLimbVelocityCountdownTicks;
	private float limbVelocitySqrSum;
	private float aveLimbVelocitySqr;
	private Rigidbody[] rbodies;

	private void Awake()
	{
		var joints = GetComponentsInChildren<Joint>();

		// don't add fixed joints that may be found
		var tempList = new List<JointDriver>();
		for (int i = 0; i < joints.Length; i++)
		{
			if ( joints[i].GetType() != typeof(FixedJoint) ) // no fixed joints
			{
				tempList.Add( joints[i].gameObject.AddComponent<JointDriver>() );
			}
		}

		jointControllers = tempList.ToArray();
		NumberOfObservations = jointControllers.Length * 4;

		// SETTUP LIMB VELOCITY CHECKING
		var timerTicksPerSec = (1 / Time.fixedDeltaTime);
		limbVelocityTicksPerCountdown =
			Mathf.RoundToInt(timerTicksPerSec * LIMB_VELOCITY_CHECK_RESOLUTION);
		
		currentAveLimbVelocityCountdownTicks = limbVelocityTicksPerCountdown;

		rbodies = GetComponentsInChildren<Rigidbody>();
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
		ReferenceOrientation.UpdateOrientation();

		// add velocity, angular velocity, position, and rotation 
		// for each joint into an observations list.
		for (int jointIndex = 0; jointIndex < NumberOfJoints; jointIndex++)
		{
			// POSITION
			var jointPosition = jointControllers[jointIndex].transform.position;
			jointPosition = ReferenceOrientation.transform.InverseTransformDirection(jointPosition - Pelvis.position);
			
			// ROTATION
			var jointRotation = jointControllers[jointIndex].transform.localRotation.eulerAngles;
			
			// VELOCITY
			var jointVelocity = jointControllers[jointIndex].Rb.velocity;
			jointVelocity = ReferenceOrientation.transform.InverseTransformDirection(jointVelocity);

			// ANGULAR VELOCITY
			var jointAngularVelocity = jointControllers[jointIndex].Rb.angularVelocity;
			jointAngularVelocity = ReferenceOrientation.transform.InverseTransformDirection(jointAngularVelocity);


			// NORMALIZE DATA
			// when falling, max velocity was 2, so make max 40
			// when falling, max angular velocity was 10, so make max 200
			// making world space a max of -30 to 30 units for the agent
			NormalizeVectorValues(ref jointVelocity, MAX_MIN_VELOCITY_ESTIMATE);
			NormalizeVectorValues(ref jointAngularVelocity, MAX_MIN_ROT_VELOCITY_ESTIMATE);
			NormalizeVectorValues(ref jointPosition, MAX_MIN_AGENT_WORLD_SIZE);
			jointRotation = jointRotation / 720f - Vector3.one;    //normalized [-1, 1]

			sensor.AddObservation(jointVelocity);
			sensor.AddObservation(jointAngularVelocity);        
			sensor.AddObservation(jointPosition);
			sensor.AddObservation(jointRotation);
		}

		// Pelvis data
		var pelvisPosition = ReferenceOrientation.transform.InverseTransformDirection(Pelvis.transform.position);
		NormalizeVectorValues(ref pelvisPosition, MAX_MIN_AGENT_WORLD_SIZE);

		var pelvisRotation = ReferenceOrientation.transform.InverseTransformDirection(Pelvis.transform.eulerAngles);
		pelvisRotation = pelvisRotation / 720f - Vector3.one;    //normalized [-1, 1]

		sensor.AddObservation(pelvisPosition);
		sensor.AddObservation(pelvisRotation);

		// Pelvis and Orientation rotation difference
		var deltaRotation = Quaternion.FromToRotation(Pelvis.forward, ReferenceOrientation.transform.forward);
		sensor.AddObservation(deltaRotation.eulerAngles / 720f - Vector3.one);
	}

	private void ApplyTorque(Vector3[] torques)
	{
		for (int i = 0; i < NumberOfJoints; i++)
		{
			jointControllers[i].ApplyRelativeTorque( in torques[i] );
		}
	}

	private void UpdateAveLimbVelocitySqr()
	{
		// LIMB VELOCITY AVERAGE		
		if (currentAveLimbVelocityCountdownTicks > 0)
		{
			currentAveLimbVelocityCountdownTicks--;

			// get sum of all velocities this frame
			for (int i = 0; i < rbodies.Length; i++)
			{
				limbVelocitySqrSum += Mathf.Abs(rbodies[i].velocity.sqrMagnitude);
			}
		}
		else // finished - calculate new average
		{
			aveLimbVelocitySqr = limbVelocitySqrSum / limbVelocityTicksPerCountdown;
			aveLimbVelocitySqr /= rbodies.Length;

			// settup for next average
			currentAveLimbVelocityCountdownTicks = limbVelocityTicksPerCountdown;
			limbVelocitySqrSum = 0;
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

	private void FixedUpdate()
	{
		UpdateAveLimbVelocitySqr();
	}
}