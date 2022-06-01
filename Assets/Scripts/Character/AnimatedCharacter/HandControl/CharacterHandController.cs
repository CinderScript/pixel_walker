/**
 *	Project:		Pixel Walker
 *	
 *	Description:	CharacterHandController provides a means of controlling 
 *					the agents hand by moving the hand's inverse kinimatics 
 *					target. This target has a rigidbody and collider so that
 *					it can interact with the scene and manipulate objects.
 *					
 *					This controller reads the values in the CharacterMovementValues
 *					object and applies the movements to the hand's IK target, 
 *					within the constraints defined using dot products of the direction
 *					the agent is facing and the vector pointing to the agent's hand's
 *					inverse kinimatic target.
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

/// <summary>
/// CharacterHandController provides a means of controlling 
/// the agents hand by moving the hand's inverse kinimatics 
/// target. This target has a rigidbody and collider so that
/// it can interact with the scene and manipulate objects.
///					
/// This controller reads the values in the CharacterMovementValues
/// object and applies the movements to the hand's IK target, 
/// within the constraints defined using dot products of the direction
/// the agent is facing and the vector pointing to the agent's hand's
/// inverse kinimatic target.
/// </summary>
public class CharacterHandController : MonoBehaviour
{
	private const float HAND_SPEED = 1f;

	[Header("Dot product constraints")]
	private float leftDotConstraint = 0.12f;
	private float rightDotConstraint = 0.62f;

	private float forwardDotConstraint = 0.66f;
	private float backwardDotConstraint = 0.08f;

	private float upDotConstraint = 1.8f;
	private float downDotConstraint = 0.8f;

	public float horizontalDot;
	public float depthDot;
	public float verticalDot;

	private CharacterMovementValues input;
    private Rigidbody targetRb;
	private Transform bodyReference;

    // Start is called before the first frame update
    void Start()
    {
		input = GetComponent<CharacterMovementValues>();
        targetRb = GetComponentInChildren<HandTarget>().GetComponent<Rigidbody>();
		bodyReference = GetComponentInParent<CharacterController>().transform;
	}

    // Update is called once per frame
    void FixedUpdate()
    {
        MoveHand();
    }

	void MoveHand()
	{
		Vector3 currentVelocity = Vector3.zero;
		
		Vector3 dirFromReferenceToTarget = targetRb.position - bodyReference.position;

		// transform.forward is the current agent direction
		// dot product of agent's right direction and target direction
		horizontalDot = Vector3.Dot(dirFromReferenceToTarget, transform.right);
		depthDot = Vector3.Dot(dirFromReferenceToTarget, transform.forward);
		verticalDot = Vector3.Dot(dirFromReferenceToTarget, transform.up);


		if (input.handForwardMovement == 1)
		{
			if (depthDot < forwardDotConstraint)
			{
				currentVelocity += bodyReference.forward * HAND_SPEED;
			}
		}
		else if (input.handForwardMovement == 2)
		{
			if (depthDot > backwardDotConstraint)
			{
				currentVelocity += bodyReference.forward * -HAND_SPEED;
			}
		}

		if (input.handSideMovement == 1)
		{
			if (horizontalDot < rightDotConstraint)
			{
				currentVelocity += bodyReference.right * HAND_SPEED;
			}
		}
		else if (input.handSideMovement == 2)
		{
			if (horizontalDot > leftDotConstraint)
			{
				currentVelocity += bodyReference.right * -HAND_SPEED;
			}
		}

		if (input.handVerticalMovement == 1)
		{
			if (verticalDot < upDotConstraint)
			{
				currentVelocity += bodyReference.up * HAND_SPEED;
			}
		}
		else if (input.handVerticalMovement == 2)
		{
			if (verticalDot > downDotConstraint)
			{
				currentVelocity += bodyReference.up * -HAND_SPEED;
			}
		}

		
		targetRb.velocity = currentVelocity;
		targetRb.angularVelocity = Vector3.zero;
	}
}
