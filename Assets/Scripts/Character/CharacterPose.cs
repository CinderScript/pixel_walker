using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

public class CharacterPose
{
	public Dictionary<string, Vector3> Positions { get; }
	public Dictionary<string, Quaternion> Rotations { get; }

	public CharacterPose()
	{
		Positions = new Dictionary<string, Vector3>();
		Rotations = new Dictionary<string, Quaternion>();
	}
}