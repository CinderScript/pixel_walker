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
