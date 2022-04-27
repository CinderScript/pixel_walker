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
	public GameObject LocationMarker;

	private int right_foot_id;
	private int left_foot_id;

	private CharacterController cc;
	private CharacterPose characterPose;

	private float remainStandingreward = 0.1f;
	private float countDownTimerTicks;
	private float ticksPerSec;

	void Awake()
	{
		cc = Character.GetComponent<CharacterController>();
		right_foot_id = Right_Foot.GetInstanceID();
		left_foot_id = Left_Foot.GetInstanceID();
		ticksPerSec = 1 / Time.fixedDeltaTime;

		Character.GetComponent<ChildCollisionListener>().OnCollisionStay += OnCollisionStayHandler;
		characterPose = new CharacterPose(Character);
	}

	public override void OnEpisodeBegin()
	{
		characterPose.ApplyPoseTo(Character);
		countDownTimerTicks = ticksPerSec;
	}

	public override void CollectObservations(VectorSensor sensor)
	{
		var observations = cc.GetLimbObservations();
		for (int i = 0; i < cc.NumberOfObservations; i++)
		{
			sensor.AddObservation(observations[i]);
		}
	}

	public override void OnActionReceived(ActionBuffers actionBuffers)
	{
		cc.ProcessActionBuffers(actionBuffers);
	}

	private void OnCollisionStayHandler(GameObject obj, Collision collision)
	{
		ContactPoint[] contactPoints = new ContactPoint[collision.contactCount];
		collision.GetContacts(contactPoints);

		for (int i = 0; i < contactPoints.Length; i++)
		{
			// if the collision was not with the feet, then the character fell
			var id = contactPoints[i].thisCollider.gameObject.GetInstanceID();
			if ( id != right_foot_id &&
				 id != left_foot_id )
			{
				SetReward(-0.3f);
				EndEpisode();
			}
		}
	}

	private void FixedUpdate()
	{
		countDownTimerTicks -= Time.fixedDeltaTime;

		if (LocationMarker.transform.position.y < -0.1)
		{
			SetReward(-0.2f);
			EndEpisode();
		}

		if (countDownTimerTicks == 0)
		{
			RewardAgent();
		}
	}

	private void RewardAgent()
	{
		if (LocationMarker.transform.position.y > 0.75)
			SetReward(remainStandingreward);

		if (remainStandingreward > 0.9999f)
		{
			SetReward(1);
			EndEpisode();
		}
	}

	public override void Heuristic(in ActionBuffers actionsOut)
	{

	}
}