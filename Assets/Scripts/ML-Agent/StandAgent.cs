using System.Collections;
using System.Collections.Generic;

using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

using UnityEngine;

public class StandAgent : Agent
{
	public GameObject Character;
	public GameObject Right_Foot;
	public GameObject Left_Foot;

	private int right_foot_id;
	private int left_foot_id;

	private CharacterController charController;
	private CharacterPose characterPose;
	private Transform pelvis;
	private Transform referenceOrientation;
	private Vector3 startPos;

	private float remainStandingReward = 0.125f;
	private float correctLocationReward = 0.0625f;

	private float TIMER_SECONDS = 0.25f;
	private int ticksPerCountdown;
	private int currentCountdownTicks;

	void Awake()
	{
		charController = Character.GetComponent<CharacterController>();
		right_foot_id = Right_Foot.GetInstanceID();
		left_foot_id = Left_Foot.GetInstanceID();

		ticksPerCountdown = (int)(1 / Time.fixedDeltaTime);
		ticksPerCountdown = ticksPerCountdown / (int)(1 / TIMER_SECONDS);
		currentCountdownTicks = ticksPerCountdown;

		Character.GetComponent<ChildCollisionListener>().OnCollisionEnter += OnCollisionEnterHandler;
		characterPose = new CharacterPose(Character);
		pelvis = charController.Pelvis;
		startPos = pelvis.position;
		referenceOrientation = charController.ReferenceOrientation.transform;
	}

	public override void OnEpisodeBegin()
	{
		characterPose.ApplyPoseTo(Character);
	}
	public override void CollectObservations(VectorSensor sensor)
	{
		charController.AddLimbObservationsTo(sensor);
	}
	public override void OnActionReceived(ActionBuffers actionBuffers)
	{
		// ToDo: Don't request torque values for y and z-axis on hinge joints (only x-axis)
		charController.ProcessActionBuffers(actionBuffers);
	}
	private void OnCollisionEnterHandler(GameObject obj, Collision collision)
	{
		ContactPoint[] contactPoints = new ContactPoint[collision.contactCount];
		collision.GetContacts(contactPoints);

		// for every collision point, check what object collided
		for (int i = 0; i < contactPoints.Length; i++)
		{
			// if the collision was not with the feet, then the character fell
			var id = contactPoints[i].thisCollider.gameObject.GetInstanceID();
			if ( id != right_foot_id &&
				 id != left_foot_id )
			{
				SetReward(-0.5f);
				EndEpisode();
			}
		}
	}

	private void FixedUpdate()
	{
		currentCountdownTicks--;

		if (currentCountdownTicks < 0)
		{
			currentCountdownTicks = ticksPerCountdown;
			AssignRewards();
		}
	}

	private void AssignRewards()
	{
		float reward = 0;
		float distanceFromTarget = Vector3.Distance(startPos, pelvis.position);

		// check for out of bounds
		if (distanceFromTarget > 1)
		{
			SetReward(-1);
			EndEpisode();
		}


		// give reward for standing
		if ( pelvis.position.y > 0.82f   && 
			 pelvis.position.y < 1.1f)
		{
			reward += remainStandingReward;

			// additional correct location reward if already standing
			if (distanceFromTarget < 0.15f)
			{
				reward += correctLocationReward;
			}
		}

		// scale the assigned reward with how well the target rotation is matched.
		// 1.0 when a perfect match - goes to zero if not matching.
		var rotationMatchValue = (Vector3.Dot(pelvis.forward, referenceOrientation.forward) + 1) * .5F;
		reward *= rotationMatchValue;

		AddReward(reward);
	}

	public override void Heuristic(in ActionBuffers actionsOut)
	{

	}
}