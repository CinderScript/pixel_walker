using System.Collections;
using System.Collections.Generic;

using Unity.MLAgents;

using UnityEngine;

public class BehaviorController : MonoBehaviour
{
	[Header("Behaviors")]
	public GameObject WalkTo;
	public GameObject Open;
	public GameObject PickUpObject;
	public GameObject PlaceObject;

	public Agent SelectBehavior()
	{
		return WalkTo.GetComponent<Agent>();
	}
}
