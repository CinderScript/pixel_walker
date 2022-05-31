using System;

using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class UserInputValues : MonoBehaviour
{
	[Header("Player Body Input Values")]
	public int forward;
	public int rotate;
	public Vector2 Look;

	[Header("Player Hand Input Values")]
	public int handForward;
	public int handSide;
	public int handVertical;

	/* * * INVOKED BY UNITY'S PLAYER INPUT CLASS * * * */
	public void OnMove(InputValue value)
	{
		MoveInput(value.Get<Vector2>());
	}

	public void OnLook(InputValue value)
	{
		LookInput(value.Get<Vector2>());
	}

	public void OnHand(InputValue value)
	{
		HandMoveInput(value.Get<Vector3>());
	}

	/* * * UPDATE INPUT VALUES * * * */
	
	/// <summary>
	/// Records the player's input values to the class variables
	/// forward and rotate. Converts rotate values from [-1, 0, 1] to
	/// [0, 1, 2] so they can easily be given to ml-agents discrete 
	/// action buffer.
	/// forward is also recorded as values from [0, 1].
	/// </summary>
	/// <param name="inputVector"></param>
	public void MoveInput(Vector2 inputVector)
	{
		if (inputVector.x > 0.01)
			rotate = 1;	// rotate right
		else if (inputVector.x < -0.01)
			rotate = 2; // rotate left
		else
			rotate = 0;	//no rotation

		if (inputVector.y > 0.01)
			forward = 1;	// walk forward
		else
			forward = 0;	// don't walk
	} 

	public void LookInput(Vector2 inputLookDirection)
	{
		Look = inputLookDirection;
	}

	private void HandMoveInput(Vector3 inputVector)
	{
		if (inputVector.x > 0.01)
			handSide = 1; // right
		else if (inputVector.x < -0.01)
			handSide = 2; // left
		else
			handSide = 0; // still

		if (inputVector.y > 0.01)
			handVertical = 1; // up
		else if (inputVector.y < -0.01)
			handVertical = 2; // down
		else
			handVertical = 0; // still

		if (inputVector.z > 0.01)
			handForward = 1; // forward
		else if (inputVector.z < -0.01)
			handForward = 2; // backward
		else
			handForward = 0; // still
	}
}