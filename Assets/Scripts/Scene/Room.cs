using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
	public RoomName RoomName;

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			other.GetComponentInChildren<NavigateAgent>().SetCurrentRoom(this);
		}
	}
}

public enum RoomName
{
	None, Workshop, Music_Room, Tool_Room, Paint_Room
}
