using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
public class SpawnPointReferences : MonoBehaviour
{
	[Tooltip("The spawn points that are available to the player. Populated at runtime.")]
	[Header("Possible Spawn Points")]
	public Transform[] spawnLocations;
	public SpawnPoint selectedLocation;

	private void Awake()
	{
		// get all spawn points
		var spawnPoints = GetComponentsInChildren<SpawnPoint>();
		var allPoints = from point in spawnPoints
						select point.transform;
		spawnLocations = allPoints.ToArray();
		
	}
	
	public SpawnPoint SelectRandomLocation()
	{
		if (spawnLocations.Length == 0)
		{
			Debug.LogError("No Locations");
			return null;
		}

		var location = spawnLocations[Random.Range(0, spawnLocations.Length)];
		return selectedLocation = location.GetComponent<SpawnPoint>();
	}
}