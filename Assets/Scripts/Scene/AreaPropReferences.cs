using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class AreaPropReferences : MonoBehaviour
{
	[Header("Scene Objects")]
	public PropInfo[] Props;
    public GameObject SelectedProp;

	[Header("Scene Rooms")]
	public Room[] Rooms;

	private void Awake()
	{
		// area containing props is the parent of this object
		Props = GetComponentsInChildren<PropInfo>();
		Rooms = GetComponentsInChildren<Room>();
	}

	public GameObject SelectRandomProp()
	{
		if (Props.Length == 0)
		{
			Debug.LogError("No props found");
			return null;
		}

		var prop = Props[Random.Range(0, Props.Length)];
		return SelectedProp = prop.gameObject;
	}

	public GameObject GetProp(string name)
	{
		name = name.Replace("_", " ");
		return Props.FirstOrDefault(prop => prop.Name.ToLower() == name.ToLower())?.gameObject;
	}
	public GameObject GetRoom(string name)
	{
		name = name.Replace("_", " ");
		return Rooms.FirstOrDefault(room => RoomNameToString(room).ToLower() == name.ToLower())?.gameObject;
	}
	private string RoomNameToString(Room room)
	{
		return room.Name.ToString().Replace("_", " ");
	}

	public string GetAllPropNames()
	{
		string[] names = Props.Select(prop => prop.Name).ToArray();
		for (int i = 0; i < names.Length; i++)
		{
			names[i] = names[i].Replace(' ', '_');
		}
		return string.Join(", ", names);
	}
}