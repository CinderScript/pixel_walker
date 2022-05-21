using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class UserInputValues : MonoBehaviour
{
	[Header("Player Input Values")]
	public int forward;
	public int rotate;
	public Vector2 Look;

	/* * * INVOKED BY UNITY'S PLAYER INPUT CLASS * * * */
	public void OnMove(InputValue value)
	{
		MoveInput(value.Get<Vector2>());
	}

	public void OnLook(InputValue value)
	{
		LookInput(value.Get<Vector2>());
	}

	/// <summary>
	/// Records the player's input values to the class variables
	/// forward and rotate. Converts rotate values from [-1, 0, 1] to
	/// [0, 1, 2] so they can easily be given to ml-agents discrete 
	/// action buffer.
	/// forward is also recorded as values from [0, 1].
	/// </summary>
	/// <param name="inputDirection"></param>
	public void MoveInput(Vector2 inputDirection)
	{
		if (inputDirection.x > 0.01)
			rotate = 1;	// rotate right
		else if (inputDirection.x < -0.01)
			rotate = 2; // rotate left
		else
			rotate = 0;	//no rotation

		if (inputDirection.y > 0.01)
			forward = 1;	// walk forward
		else
			forward = 0;	// don't walk
	} 

	public void LookInput(Vector2 inputLookDirection)
	{
		Look = inputLookDirection;
	}
}