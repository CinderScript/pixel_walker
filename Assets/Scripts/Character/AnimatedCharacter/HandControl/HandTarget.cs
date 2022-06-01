/**
 *	Project:		Pixel Walker
 *	
 *	Description:	HandTarget is attached to the Agent's hand's
 *					inverse kinimatic target as a means of giving 
 *					the CharacterHandController a fast lookup of this
 *					object in the scene.
 *					
 *					In a future update, this script will also control the
 *					rotation of the rigidbody to match the rotation of the 
 *					agent's hand so that interactions with the IK target
 *					match the interactions the hand would make.
 *					
 *	Author:			Pixel Walker -
 *						Maynard, Gregory
 *						Shubhajeet, Baral
 *						Do, Khuong
 *						Nguyen, Thuong						
 *					
 *	Date:			05-30-2022
 */

using UnityEngine;

public class HandTarget : MonoBehaviour
{
	// for use with physical hand that matches rotation of target
	// not in use when using IK to target finger

	//[Header("Hand of Agent")]
	//public Transform hand;

	//// this objects RB
	//private Rigidbody targetRb;

	//private void Awake()
	//{
	//	targetRb = GetComponent<Rigidbody>();
	//}

	//private void FixedUpdate()
	//{
	//	//Debug.DrawRay(hand.position, -hand.up, Color.green);
	//	//Debug.DrawRay(targetRb.position, -targetRb.transform.up, Color.red);

	//	// point targetRB in direction the hand is pointing
	//	targetRb.rotation = Quaternion.LookRotation(hand.forward, hand.up);
	//	targetRb.angularVelocity = Vector3.zero;
	//}
}
