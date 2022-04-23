using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorSelector : MonoBehaviour
{
	[Header("Behaviors")]
	public GameObject Stand;
	public GameObject WalkTo;
	public GameObject Open;
	public GameObject PickUpObject;
	public GameObject PlaceObject;

	public GameObject SelectBehavior()
	{

		return GameObject.FindGameObjectWithTag("Stand");
	}
}