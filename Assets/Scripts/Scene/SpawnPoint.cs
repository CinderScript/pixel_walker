using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
	public Room room;
		
	private void Awake()
	{
		// Get the room that this spawn point is in
		room = GetComponentInParent<Room>();
	}
}