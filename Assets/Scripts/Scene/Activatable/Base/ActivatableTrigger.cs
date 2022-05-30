using UnityEngine;

public abstract class ActivatableTrigger : MonoBehaviour
{
	public abstract void TriggerActivatables();

	private void OnTriggerEnter(Collider other)
	{
		TriggerActivatables();
	}
}
