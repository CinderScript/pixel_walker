using UnityEngine;

public class HandTarget : MonoBehaviour
{
	[Header("Hand of Agent")]
	public Transform hand;

	// this objects RB
	private Rigidbody targetRb;

	private void Awake()
	{
		targetRb = GetComponent<Rigidbody>();
	}

	private void FixedUpdate()
	{

		// point targetRB in direction the hand is pointing
		targetRb.rotation = Quaternion.LookRotation(hand.forward, hand.up);

		targetRb.angularVelocity = Vector3.zero;
	}
}
