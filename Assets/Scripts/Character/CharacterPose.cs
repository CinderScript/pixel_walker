using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

public class CharacterPose
{
	private Dictionary<int, Vector3> Positions { get; }
	private Dictionary<int, Quaternion> Rotations { get; }

	public CharacterPose(GameObject character)
	{
		Positions = new Dictionary<int, Vector3>();
		Rotations = new Dictionary<int, Quaternion>();

		// Remember this pose
		var parts = character.GetComponentsInChildren<Transform>();
		foreach (var part in parts)
		{
			saveBodyPartsPose(part);
		}
	}

	public void ApplyPoseTo(GameObject character)
	{
		var parts = character.GetComponentsInChildren<Transform>();
		foreach (var part in parts)
		{
			applyPoseToPart(part);
		}
	}

	/// <summary>
	/// Adds a child object of the character to the pose.
	/// Uses a rigidbody for position and rotation if 
	/// one exists, otherwise uses the transform.
	/// </summary>
	/// <param name="bodyPart">Character's child object to add.</param>
	private void saveBodyPartsPose(Transform bodyPart)
	{
		var id = bodyPart.gameObject.GetInstanceID();

		Positions[id] = bodyPart.transform.position;
		Rotations[id] = bodyPart.transform.rotation;
	}

	private void applyPoseToPart(Transform bodyPart)
	{
		var id = bodyPart.gameObject.GetInstanceID();
		var rb = bodyPart.GetComponent<Rigidbody>();

		if (rb)
		{
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
		}

		bodyPart.transform.position = Positions[id];
		bodyPart.transform.rotation = Rotations[id];
	}
}