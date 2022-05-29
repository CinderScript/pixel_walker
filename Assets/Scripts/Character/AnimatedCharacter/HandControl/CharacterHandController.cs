using UnityEngine;

public class CharacterHandController : MonoBehaviour
{
	private const float HAND_SPEED = 1f;

	[Header("Dot product constraints")]
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

		
		if (input.handForwardMovement == 1)
		{
			currentVelocity += bodyReference.forward * HAND_SPEED;
		}
		else if (input.handForwardMovement == 2)
		{
			currentVelocity += bodyReference.forward * -HAND_SPEED;
		}

		if (input.handSideMovement == 1)
		{
			currentVelocity += bodyReference.right * HAND_SPEED;
		}
		else if (input.handSideMovement == 2)
		{
			currentVelocity += bodyReference.right * -HAND_SPEED;
		}

		if (input.handVerticalMovement == 1)
		{
			currentVelocity += bodyReference.up * HAND_SPEED;
		}
		else if (input.handVerticalMovement == 2)
		{
			currentVelocity += bodyReference.up * -HAND_SPEED;
		}

		
		targetRb.velocity = currentVelocity;
		targetRb.angularVelocity = Vector3.zero;
	}
}
