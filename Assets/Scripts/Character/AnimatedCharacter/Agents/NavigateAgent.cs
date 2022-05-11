using Unity.MLAgents;
using Unity.MLAgents.Actuators;

using UnityEngine;

public class NavigateAgent : Agent
{
	public UserInputValues userInputValues;
	public CharacterMovementValues movementValues;

	public override void OnActionReceived(ActionBuffers actions)
	{
		movementValues.forward = actions.DiscreteActions[0];
		movementValues.rotate = actions.DiscreteActions[1];
	}

	public override void Heuristic(in ActionBuffers actionsOut)
	{
		var actions = actionsOut.DiscreteActions;
		actions[0] = userInputValues.forward;
		actions[1] = userInputValues.rotate;
	}

	private void WalkCharacter(int movement)
	{
		movementValues.forward = movement;
	}
	private void RotateCharacter(int rotation)
	{
		movementValues.rotate = rotation;
	}
}