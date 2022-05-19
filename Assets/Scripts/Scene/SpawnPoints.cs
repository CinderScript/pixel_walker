using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoints : MonoBehaviour
{
	public List<Transform> SpawnLocations;
	public Transform SelectedLocation;

	private void Awake()
	{
		foreach (Transform child in transform)
		{
			SpawnLocations.Add(child);
		}
	}

	public Transform SelectRandomLocation()
	{
		if (SpawnLocations.Count == 0)
		{
			Debug.LogError("No Locations");
			return null;
		}

		var location = SpawnLocations[Random.Range(0, SpawnLocations.Count)];
		return SelectedLocation = location;
	}
}