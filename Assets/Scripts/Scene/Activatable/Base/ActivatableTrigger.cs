using UnityEngine;

public abstract class ActivatableTrigger : MonoBehaviour
{
	public abstract void TriggerActivatables();

	private void OnTriggerEnter(Collider other)
	{
		// check if this is the player's hand IK target
		if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
		{
			var agent = other.GetComponentInParent<CharacterRoot>();
			var activationAgent = agent.GetComponentInChildren<ActivateAgent>();
			activationAgent.SetTriggerActivated();
			TriggerActivatables();
		}
	}
}
