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

	/// <summary>
	/// Returns the scene prop with the name given. If a location is specified,
	/// then this location is prioritized if multiple objects have the same name.
	/// </summary>
	/// <param name="name"></param>
	/// <param name="location"></param>
	/// <returns></returns>
	public PropInfo GetProp(string name, string location = "")
	{
		name = name.Replace("_", " ");
		location = location.Replace("_", " ");
		var room = GetRoom(location);

		// if there is no room, then find the prop
		if (!room)
		{
			return Props.FirstOrDefault(prop => prop.Name.ToLower() == name.ToLower());
		}
		else
		{
			// get all objects that match
			var propsMatched = Props.Where(prop => prop.Name.ToLower() == name.ToLower());

			// if there is at least one, set this to be returned if a room cannot be matched
			PropInfo returnValue = null;
			if (propsMatched.Count() > 0)
			{
				returnValue = propsMatched.First();
			}

			foreach (var prop in propsMatched)
			{
				if (prop.room == room)
				{
					return prop;
				}
			}

			// if no room was matched, return name match if any
			return returnValue;
		}

	}
	public Room GetRoom(string name)
	{
		name = name.Replace("_", " ");
		return Rooms.FirstOrDefault(room => RoomNameToString(room).ToLower() == name.ToLower());
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