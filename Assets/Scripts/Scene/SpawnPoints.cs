using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoints : MonoBehaviour
{
	public Transform[] SpawnLocations;
	public Transform SelectedLocation;

	private void Awake()
	{
		SpawnLocations = GetComponentsInChildren<Transform>();
	}

	public Transform SelectRandomLocation()
	{
		if (SpawnLocations.Length == 0)
		{
			Debug.LogError("No Locations");
			return null;
		}

		var location = SpawnLocations[Random.Range(0, SpawnLocations.Length)];
		return SelectedLocation = location;
	}
}